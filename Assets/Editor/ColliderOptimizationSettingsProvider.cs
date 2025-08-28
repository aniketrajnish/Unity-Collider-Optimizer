#if UNITY_EDITOR
using UnityColliderOptimizer.Core;
using UnityEditor;
using UnityEngine.UIElements;
namespace UnityColliderOptimizer.E
{
    public class GlobalColliderOptimizationSettingsProvider : SettingsProvider
    {
        SerializedObject _so;
        public GlobalColliderOptimizationSettingsProvider(string path, SettingsScope scope) : base(path, scope) { }
        public static bool IsSettingsAvailable() => OptimizationSettings.Instance != null;

        [SettingsProvider]
        public static SettingsProvider Create()
        {
            var provider = new GlobalColliderOptimizationSettingsProvider("Project/Collider Optimizer", SettingsScope.Project);
            return provider;
        }
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _so = new SerializedObject(OptimizationSettings.Instance);
        }
        public override void OnGUI(string searchContext)
        {
            if (_so == null) return;
            _so.Update();
            EditorGUILayout.PropertyField(_so.FindProperty("DefaultMOP"), true);
            EditorGUILayout.PropertyField(_so.FindProperty("DefaultPOP"), true);
            _so.ApplyModifiedProperties();
        }
    }
}
#endif
