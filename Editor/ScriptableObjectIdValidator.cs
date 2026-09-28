using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Calluna.Core.Editor
{
    /// <summary>
    /// Finds <see cref="ScriptableObjectId"/> assets whose id is empty or used by another asset of the
    /// same type - either breaks lookups by id, e.g. of <see cref="Id{TDefinition}"/> keys or saved ids.
    /// Run it from the menu Calluna > Diagnostics, or guard a project with an edit mode test:
    /// <code>Assert.IsEmpty(ScriptableObjectIdValidator.FindProblems());</code>
    /// The checks themselves live in <see cref="ScriptableObjectIdValidation"/>.
    /// </summary>
    public static class ScriptableObjectIdValidator
    {
        /// <summary>One message per empty or duplicated id, naming the asset paths involved.</summary>
        public static IReadOnlyList<string> FindProblems()
        {
            List<(ScriptableObjectId asset, string path)> assets = AssetDatabase
                .FindAssets($"t:{nameof(ScriptableObjectId)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Distinct()
                .SelectMany(path => AssetDatabase.LoadAllAssetsAtPath(path)
                    .OfType<ScriptableObjectId>()
                    .Select(asset => (asset, path)))
                .ToList();
            return ScriptableObjectIdValidation.FindProblems(assets);
        }

        [MenuItem("Calluna/Diagnostics/Validate ScriptableObject Ids")]
        private static void ValidateFromMenu()
        {
            IReadOnlyList<string> problems = FindProblems();
            if (problems.Count == 0)
            {
                Debug.Log("[ScriptableObjectIdValidator] All ids are set and unique per type.");
                return;
            }
            foreach (string problem in problems)
                Debug.LogWarning($"[ScriptableObjectIdValidator] {problem}");
        }
    }
}
