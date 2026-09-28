using UnityEditor;

namespace Calluna.Core.Editor
{
    /// <summary>Toggles <see cref="ObservableDiagnostics.LogUnchangedValues"/>, remembered per machine.</summary>
    public static class ObservableDiagnosticsMenu
    {
        private const string MenuPath = "Calluna/Diagnostics/Log Unchanged Observable Values";
        private const string PrefKey = "Calluna.ObservableDiagnostics.LogUnchangedValues";

        [InitializeOnLoadMethod]
        private static void ApplySavedSetting() =>
            ObservableDiagnostics.LogUnchangedValues = EditorPrefs.GetBool(PrefKey, false);

        [MenuItem(MenuPath)]
        private static void Toggle()
        {
            bool enabled = !EditorPrefs.GetBool(PrefKey, false);
            EditorPrefs.SetBool(PrefKey, enabled);
            ObservableDiagnostics.LogUnchangedValues = enabled;
        }

        [MenuItem(MenuPath, true)]
        private static bool ToggleValidate()
        {
            Menu.SetChecked(MenuPath, EditorPrefs.GetBool(PrefKey, false));
            return true;
        }
    }
}
