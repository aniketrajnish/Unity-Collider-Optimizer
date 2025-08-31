#if UNITY_EDITOR
using System.Linq;
using Codice.Client.BaseCommands.Changelist;
using UnityColliderOptimizer.Core;
using UnityColliderOptimizer.Core.P;
using UnityColliderOptimizer.Services;
using UnityEditor;
using UnityEngine;

namespace UnityColliderOptimizer.E
{
    public class BaseColliderOptimizerEditor<T> : Editor where T : Object, IColliderOptimizer
    {
        protected IAssetPersistence _persistence;
        protected virtual void OnEnable() => _persistence = new IAssetPersistenceEditor();
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var row = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight + 4f);
            const int cols = 4;
            const float gap = 4f;
            float w = (row.width - gap * (cols - 1)) / cols;
            float h = row.height;

            var rOptimize = new Rect(row.x, row.y, w, h);
            var rReset = new Rect(rOptimize.xMax + gap, row.y, w, h);
            var rSave = new Rect(rReset.xMax + gap, row.y, w, h);
            var rLoad = new Rect(rSave.xMax + gap, row.y, w, h);

            if (GUI.Button(rOptimize, "Optimize"))
            {
                foreach (var o in targets.Cast<T>())
                {
                    Undo.RecordObject((Object)o, "Optimize");
                    o.Optimize();
                    EditorUtility.SetDirty((Object)o);
                }
            }

            if (GUI.Button(rReset, "Reset"))
            {
                foreach (var o in targets.Cast<T>())
                {
                    Undo.RecordObject((Object)o, "Reset");
                    o.Reset();
                    EditorUtility.SetDirty((Object)o);
                }
            }

            if (GUI.Button(rSave, "Save"))
            {
                foreach (var o in targets.Cast<T>())
                {
                    var comp = o as Component;
                    if (!comp) continue;

                    var mc = comp.GetComponent<MeshCollider>();
                    if (mc != null && mc.sharedMesh != null)
                    {
                        _persistence.SaveMeshAsset(mc.sharedMesh, comp.name + "_CollMesh");
                        continue;
                    }

                    var pc = comp.GetComponent<PolygonCollider2D>();
                    if (pc != null)
                    {
                        var data = CreateInstance<PathData>();
                        var paths = new System.Collections.Generic.List<Path2D>(pc.pathCount);
                        for (int i = 0; i < pc.pathCount; i++)
                        {
                            paths.Add(new Path2D { Pts = pc.GetPath(i) });
                        }
                        data.Paths = paths;
                        _persistence.SavePathDataAsset(data, comp.name + "_CollPaths");
                    }
                }
            }

            if (GUI.Button(rLoad, "Load"))
                EditorGUIUtility.ShowObjectPicker<Object>(null, false, "", 0);

            if (Event.current.commandName == "ObjectSelectorClosed")
            {
                var picked = EditorGUIUtility.GetObjectPickerObject();
                if (picked != null)
                {
                    foreach (var o in targets.Cast<T>())
                    {
                        Undo.RecordObject((Object)o, "Load Collider");
                        o.LoadSaved(picked);
                        EditorUtility.SetDirty((Object)o);
                    }
                }
                Repaint();
            }
        }

    }
}
#endif
