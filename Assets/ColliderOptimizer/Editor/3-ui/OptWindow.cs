#if UNITY_EDITOR
using ColliderOptimizer.Core;
using UnityEditor;
using UnityEngine;

public class OptWindow : EditorWindow
{
    const string K_TABKEY = "CO_OptWindowTab";
    const string K_MESHNERDKEY = "CO_OptWindowMeshNerd";
    const string K_POLYNERDKEY = "CO_OptWindowPolyNerd";
    const string K_ICONPATH = "Assets/ColliderOptimizer/Assets/2-sprite/logo.png";

    int _tab; bool _meshNerd, _polyNerd; Texture2D _icon;
    MeshOptPreset _meshPreset; PolyOptPreset _polyPreset;

    [MenuItem("Tools/Collider Optimizer")]
    static void Open()
    {
        var win = GetWindow<OptWindow>("Collider Optimizer");
        win.ApplyIcon();
    }
    void OnEnable()
    {
        _tab = EditorPrefs.GetInt(K_TABKEY, 0);
        _meshNerd = EditorPrefs.GetBool(K_MESHNERDKEY, false);
        _polyNerd = EditorPrefs.GetBool(K_POLYNERDKEY, false);

        _meshPreset = OptSettings.GetActiveMeshPreset();
        _polyPreset = OptSettings.GetActivePolyPreset();

        ApplyIcon();
    }
    void ApplyIcon()
    {
        if (!_icon) _icon = AssetDatabase.LoadAssetAtPath<Texture2D>(K_ICONPATH);
        titleContent = new GUIContent("Collider Optimizer", _icon);
        Repaint();
    }
    void OnGUI()
    {
        var labels = new[] { "Mesh", "Polygon" };
        var newTab = GUILayout.Toolbar(_tab, labels);
        if (newTab != _tab) { _tab = newTab; EditorPrefs.SetInt(K_TABKEY, _tab); }
        EditorGUILayout.Space(6);

        if (_tab == 0) DrawMeshTab();
        else DrawPolyTab();
    }
    void DrawMeshTab()
    {
        EditorGUI.BeginChangeCheck();
        _meshPreset = (MeshOptPreset)EditorGUILayout.ObjectField("Preset", _meshPreset, typeof(MeshOptPreset), false);
        if (EditorGUI.EndChangeCheck())
            OptSettings.SetActiveMeshPreset(_meshPreset);

        var state = OptProjectState.instance;
        var mp = (_meshPreset ? _meshPreset.Params : state.MeshParams);

        EditorGUILayout.Space(6);
        GUILayout.Label("Params", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        mp.ContractionFactor = EditorGUILayout.Slider("Contraction", mp.ContractionFactor, 0f, 1f);

        _meshNerd = EditorGUILayout.Foldout(_meshNerd, "Are you a nerd?", true);
        if (_meshNerd)
        {
            EditorGUI.indentLevel++;
            mp.RecalcNormals = EditorGUILayout.Toggle("Recalc Normals", mp.RecalcNormals);
            mp.BakeScale = EditorGUILayout.Toggle("Bake Scale", mp.BakeScale);
            mp.Convex = EditorGUILayout.Toggle("Convex", mp.Convex);
            mp.Aggressive = EditorGUILayout.Toggle(new GUIContent("Aggressive (-sa)"), mp.Aggressive);
            mp.Permissive = EditorGUILayout.Toggle(new GUIContent("Permissive (-sp)"), mp.Permissive);
            mp.LossyScale = EditorGUILayout.Vector3Field(new GUIContent("Override Lossy Scale"), mp.LossyScale);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Use Selected Transform Scale") && Selection.activeTransform)
                    mp.LossyScale = Selection.activeTransform.lossyScale;
                if (GUILayout.Button("Clear Override"))
                    mp.LossyScale = Vector3.one;
            }
            EditorGUI.indentLevel--;
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (_meshPreset) EditorUtility.SetDirty(_meshPreset);
            else OptProjectState.instance.SaveProjectSettings(true);
        }
        EditorPrefs.SetBool(K_MESHNERDKEY, _meshNerd);
    }
    void DrawPolyTab()
    {
        EditorGUI.BeginChangeCheck();
        _polyPreset = (PolyOptPreset)EditorGUILayout.ObjectField("Preset", _polyPreset, typeof(PolyOptPreset), false);
        if (EditorGUI.EndChangeCheck())
        {
            OptSettings.SetActivePolyPreset(_polyPreset);
        }

        var state = OptProjectState.instance;
        var pp = (_polyPreset ? _polyPreset.Params : state.PolyParams);

        EditorGUILayout.Space(6);
        GUILayout.Label("Params", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        pp.Tolerance = EditorGUILayout.FloatField("Tolerance", pp.Tolerance);

        _polyNerd = EditorGUILayout.Foldout(_polyNerd, "Are you a nerd?", true);
        if (_polyNerd)
        {
            EditorGUI.indentLevel++;
            pp.Mode = (ToleranceMode)EditorGUILayout.EnumPopup("Tolerance Mode", pp.Mode);
            pp.PerPathScaleByBounds = EditorGUILayout.Toggle("Scale By Bounds", pp.PerPathScaleByBounds);
            EditorGUI.indentLevel--;
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (_polyPreset) EditorUtility.SetDirty(_polyPreset);
            else OptProjectState.instance.SaveProjectSettings(true);
        }
        EditorPrefs.SetBool(K_POLYNERDKEY, _polyNerd);
    }
}
#endif