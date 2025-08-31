#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityColliderOptimizer.Utils;
namespace UnityColliderOptimizer.UI
{
    static class PolygonColliderOptContextMenu
    {
        const int K_PRIO = 5050;
        static PolyOptParams PolyDefaults => new PolyOptParams
        {
            Mode = ToleranceMode.World,
            Tolerance = 0.01f,
            PerPathScaleByBounds = false
        };
        [MenuItem("CONTEXT/PolygonCollider2D/Optimize Collider", false, K_PRIO + 0)]
        static void Optimize(MenuCommand cmd)
        {
            var pc = (PolygonCollider2D)cmd.context;
            var authoring = PolyOptHelpers.BuildAuthoringPaths(pc);
            var simplified = PolyOptHelpers.SimplifyPolys(authoring, PolyDefaults);
            Undo.RecordObject(pc, "Optimize PolygonCollider2D");
            PolyOptHelpers.WritePaths(pc, simplified);
            EditorUtility.SetDirty(pc);
        }
        [MenuItem("CONTEXT/PolygonCollider2D/Reset Collider", false, K_PRIO + 10)]
        static void Reset(MenuCommand __cmd)
        {
            var pc = (PolygonCollider2D)__cmd.context;
            var authoring = PolyOptHelpers.BuildAuthoringPaths(pc);
            Undo.RecordObject(pc, "Reset PolygonCollider2D");
            PolyOptHelpers.WritePaths(pc, authoring);
            EditorUtility.SetDirty(pc);
        }
        [MenuItem("CONTEXT/PolygonCollider2D/Save Collider", false, K_PRIO + 20)]
        static void Save(MenuCommand __cmd)
        {
            var pc = (PolygonCollider2D)__cmd.context;
            var data = ScriptableObject.CreateInstance<PathData>();
            var cur = PolyOptHelpers.ReadPaths(pc);
            data.Paths = cur.ConvertAll(p => new Path2D { Pts = p.ToArray() });

            var path = EditorUtility.SaveFilePanelInProject("Save Collider Paths", pc.name + "_CollPaths", "asset", "Pick a location");
            if (string.IsNullOrEmpty(path)) { Object.DestroyImmediate(data); return; }
            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(data);
        }

        [MenuItem("CONTEXT/PolygonCollider2D/Load Collider", false, K_PRIO + 30)]
        static void Load(MenuCommand __cmd)
        {
            var pc = (PolygonCollider2D)__cmd.context;
            var data = ColliderOptEditorUtils.LoadAssetViaPanel<PathData>("Pick PathData asset", "asset");
            if (!data || data.Paths == null) return;

            var arr = new List<List<Vector2>>(data.Paths.Count);
            foreach (var p in data.Paths)
                arr.Add(new List<Vector2>(p?.Pts ?? System.Array.Empty<Vector2>()));

            Undo.RecordObject(pc, "Load Saved PolygonCollider2D");
            PolyOptHelpers.WritePaths(pc, arr);
            EditorUtility.SetDirty(pc);
        }
    }
}
#endif
