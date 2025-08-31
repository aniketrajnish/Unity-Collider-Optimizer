#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using ColliderOptimizer.Gltfpack;
using ColliderOptimizer.Utils;
namespace ColliderOptimizer.UI
{
    static class MeshColliderOptContextMenu
    {
        const int K_PRIO = 5050;
        static MeshOptParams MeshDefaults => new MeshOptParams
        {
            ContractionFactor = 0.25f,
            RecalcNormals = true,
            BakeScale = true,
            Convex = false
        };
        [MenuItem("CONTEXT/MeshCollider/Optimize Collider", false, K_PRIO + 0)]
        static void Optimize(MenuCommand cmd)
        {
            var mc = (MeshCollider)cmd.context;
            var go = mc.gameObject;
            var mf = go.GetComponent<MeshFilter>();
            var src = mf && mf.sharedMesh ? mf.sharedMesh : mc.sharedMesh;
            if (!src) { Debug.LogWarning("No source mesh found."); return; }

            Undo.RecordObject(mc, "Optimize MeshCollider");
            MeshOptHelpers.ResetMeshTo(mc, src, MeshDefaults);

            float keep = 1f - Mathf.Clamp01(MeshDefaults.ContractionFactor);
            var bakedOrSrc = (MeshDefaults.BakeScale && go.transform.lossyScale != Vector3.one)
                ? MeshOptHelpers.BakeScaledCopy(src, go.transform.lossyScale)
                : src;

            Mesh simplified = null;
            try
            {
                simplified = MeshSimplifyGateway.SimplifyWithGltfpack(
                    bakedOrSrc, keep, MeshDefaults.RecalcNormals, "Assets/ColliderOptimizer/Editor/4-opt-out");
            }
            finally
            {
                if (!ReferenceEquals(bakedOrSrc, src)) Object.DestroyImmediate(bakedOrSrc);
            }
            if (!simplified) simplified = MeshOptHelpers.CloneMesh(src);

            if (MeshDefaults.Convex && simplified.triangles != null && simplified.triangles.Length / 3 > 255)
                Debug.LogWarning("Convex MeshCollider may be auto-reduced (>255 tris)", mc);

            MeshOptHelpers.ApplyMesh(mc, simplified, MeshDefaults);
            EditorUtility.SetDirty(mc);
        }
        [MenuItem("CONTEXT/MeshCollider/Reset Collider", false, K_PRIO + 10)]
        static void Reset(MenuCommand __cmd)
        {
            var mc = (MeshCollider)__cmd.context;
            var mf = mc.GetComponent<MeshFilter>();
            var src = mf && mf.sharedMesh ? mf.sharedMesh : mc.sharedMesh;
            if (!src) { Debug.LogWarning("No authoring mesh found."); return; }

            Undo.RecordObject(mc, "Reset MeshCollider");
            MeshOptHelpers.ResetMeshTo(mc, src, MeshDefaults);
            EditorUtility.SetDirty(mc);
        }
        [MenuItem("CONTEXT/MeshCollider/Save Collider", false, K_PRIO + 20)]
        static void Save(MenuCommand __cmd)
        {
            var mc = (MeshCollider)__cmd.context;
            var mesh = mc.sharedMesh;
            if (!mesh) { Debug.LogWarning("No collider mesh to save."); return; }

            var path = EditorUtility.SaveFilePanelInProject("Save Collider Mesh", mc.name + "_CollMesh", "asset", "Pick a location");
            if (string.IsNullOrEmpty(path)) return;

            var clone = Object.Instantiate(mesh);
            AssetDatabase.CreateAsset(clone, path);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(clone);
        }
        [MenuItem("CONTEXT/MeshCollider/Load Collider", false, K_PRIO + 30)]
        static void Load(MenuCommand __cmd)
        {
            var mc = (MeshCollider)__cmd.context;
            var picked = ColliderOptEditorUtils.LoadAssetViaPanel<Mesh>("Pick Mesh asset", "asset");
            if (!picked) return;

            Undo.RecordObject(mc, "Load Saved MeshCollider");
            MeshOptHelpers.ApplyMesh(mc, picked, MeshDefaults);
            EditorUtility.SetDirty(mc);
        }
    }
}
#endif