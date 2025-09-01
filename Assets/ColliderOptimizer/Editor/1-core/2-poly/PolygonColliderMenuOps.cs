#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ColliderOptimizer.Utils;

namespace ColliderOptimizer.Core.P
{
    sealed class PolygonColliderMenuOps : IColliderMenuOps<PolygonCollider2D>
    {
        public void Optimize(PolygonCollider2D __pc)
        {
            var p = OptSettings.PolyParams;
            var authoring = PolyOptHelpers.BuildAuthoringPaths(__pc);
            var simplified = PolyOptHelpers.SimplifyPolys(authoring, p);
            Undo.RecordObject(__pc, "Optimize PolygonCollider2D");
            PolyOptHelpers.WritePaths(__pc, simplified);
            EditorUtility.SetDirty(__pc);
        }
        public void Reset(PolygonCollider2D __pc)
        {
            var authoring = PolyOptHelpers.BuildAuthoringPaths(__pc);
            Undo.RecordObject(__pc, "Reset PolygonCollider2D");
            PolyOptHelpers.WritePaths(__pc, authoring);
            EditorUtility.SetDirty(__pc);
        }
        public void Save(PolygonCollider2D __pc)
        {
            var data = ScriptableObject.CreateInstance<PathData>();
            var cur = PolyOptHelpers.ReadPaths(__pc);
            data.Paths = cur.ConvertAll(p => new Path2D { Pts = p.ToArray() });

            var path = EditorUtility.SaveFilePanelInProject("Save Collider Paths", __pc.name + "-coll-path", "asset", "Pick a location");
            if (string.IsNullOrEmpty(path)) { Object.DestroyImmediate(data); return; }
            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(data);
        }
        public void Load(PolygonCollider2D __pc)
        {
            var data = OptEditorHelpers.LoadAssetViaPanel<PathData>("Pick PathData asset", "asset");
            if (!data || data.Paths == null) return;

            var arr = new List<List<Vector2>>(data.Paths.Count);
            foreach (var p in data.Paths)
                arr.Add(new List<Vector2>(p?.Pts ?? System.Array.Empty<Vector2>()));

            Undo.RecordObject(__pc, "Load Saved PolygonCollider2D");
            PolyOptHelpers.WritePaths(__pc, arr);
            EditorUtility.SetDirty(__pc);
        }
    }
}
#endif