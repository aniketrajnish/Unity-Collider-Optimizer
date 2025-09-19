#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using ColliderOptimizer.Utils;
using ColliderOptimizer.Gltfpack;

namespace ColliderOptimizer.Core.M
{
    sealed class MeshColliderMenuOps : IColliderMenuOps<MeshCollider>
    {
        public void Optimize(MeshCollider __mc)
        {
            var go = __mc.gameObject;
            var mf = go.GetComponent<MeshFilter>();
            var src = (mf && mf.sharedMesh) ? mf.sharedMesh : __mc.sharedMesh;
            if (!src) { Debug.LogWarning("no source mesh found", __mc); return; }

            var p = OptSettings.MeshParams;
            Undo.RecordObject(__mc, "Optimize MeshCollider");
            MeshOptHelpers.ResetMesh(__mc, src, p);

            float keep = 1f - Mathf.Clamp01(p.ContractionFactor);

            Mesh simplified = MeshSimplifyGateway.SimplifyWithGltfpack(
                src, keep, p.RecalcNormals,
                "Assets/ColliderOptimizer/Editor/5-opt-out",
                p.Aggressive, p.Permissive);

            if (!simplified) simplified = MeshOptHelpers.CloneMesh(src);

            if (p.Convex && simplified.triangles != null && simplified.triangles.Length / 3 > 255)
                Debug.LogWarning("Convex MeshCollider may be auto-reduced (>255 tris)", __mc);

            MeshOptHelpers.ApplyMesh(__mc, simplified, p);
            EditorUtility.SetDirty(__mc);
        }
        public void Reset(MeshCollider __mc)
        {
            var mf = __mc.GetComponent<MeshFilter>();
            var src = (mf && mf.sharedMesh) ? mf.sharedMesh : __mc.sharedMesh;
            if (!src) { Debug.LogWarning("no authoring mesh found", __mc); return; }
            var p = OptSettings.MeshParams;
            Undo.RecordObject(__mc, "Reset MeshCollider");
            MeshOptHelpers.ResetMesh(__mc, src, p);
            EditorUtility.SetDirty(__mc);
        }
        public void Save(MeshCollider __mc)
        {
            var mesh = __mc.sharedMesh;
            if (!mesh) { Debug.LogWarning("no collider mesh to save", __mc); return; }
            var path = EditorUtility.SaveFilePanelInProject("Save Collider Mesh", __mc.name + "-coll-mesh", "asset", "Pick a location");
            if (string.IsNullOrEmpty(path)) return;
            var clone = Object.Instantiate(mesh);
            AssetDatabase.CreateAsset(clone, path);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(clone);
        }
        public void Load(MeshCollider __mc)
        {
            var picked = OptEditorHelpers.LoadAssetViaPanel<Mesh>("Pick Mesh asset", "asset");
            if (!picked) return;
            var p = OptSettings.MeshParams;
            Undo.RecordObject(__mc, "Load Saved MeshCollider");
            MeshOptHelpers.ApplyMesh(__mc, picked, p);
            EditorUtility.SetDirty(__mc);
        }
    }
}
#endif
