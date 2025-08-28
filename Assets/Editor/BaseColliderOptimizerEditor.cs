#if UNITY_EDITOR
using System.Linq;
using UnityColliderOptimizer.Core;
using UnityColliderOptimizer.Core.P;
using UnityColliderOptimizer.Services;
using UnityEditor;
using UnityEngine;

namespace UnityColliderOptimizer.E
{
    public class BaseColliderOptimizerEditor<T> : Editor where T : Object, IColliderOptimizer
    {
        protected IAssetPersistence _persistance;
        protected virtual void OnEnable() => _persistance = new AssetPersistenceEditor();

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Collider Optimization", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Optimize"))
                {
                    foreach (var o in targets.Cast<T>())
                    {
                        Undo.RecordObject((Object)o, "Optimize");
                        o.Optimize();
                        EditorUtility.SetDirty((Object)o);
                    }
                }

                if (GUILayout.Button("Reset"))
                {
                    foreach (var o in targets.Cast<T>())
                    {
                        Undo.RecordObject((Object)o, "Reset");
                        o.Reset();
                        EditorUtility.SetDirty((Object)o);
                    }
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Capture Base"))
                {
                    foreach (var o in targets.Cast<T>())
                    {
                        Undo.RecordObject((Object)o, "Capture Base");
                        o.Capture();
                        EditorUtility.SetDirty((Object)o);
                    }
                }

                if (GUILayout.Button("Save Result"))
                {
                    foreach (var o in targets.Cast<T>())
                    {
                        var saved = o.GetSaved();
                        if (saved == null) continue;

                        if (saved is Mesh m)
                            _persistance.SaveMeshAsset(m, "OptimizedCollider");
                        else if (saved is OptimizedPathData pd)
                            _persistance.SavePathDataAsset(pd, "OptimizedPath");
                    }
                }
            }
        }
    }
}
#endif
