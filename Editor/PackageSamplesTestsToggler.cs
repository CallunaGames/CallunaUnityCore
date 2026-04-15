using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class PackageSamplesTestsToggler : EditorWindow
    {
        private class Pkg
        {
            public string Name; // folder name under Packages/ (e.g., com.calluna.scenemanagement)
            public string Path; // absolute path
            public bool HasPackageJson; // is this a real package
            public bool SamplesVisible; // Samples exists (not hidden)
            public bool SamplesHidden; // Samples~ exists
            public bool TestsVisible; // Tests exists
            public bool TestsHidden; // Tests~ exists
        }

        private Vector2 _scroll;
        private List<Pkg> _packages = new List<Pkg>();
        private DateTime _lastScan = DateTime.MinValue;

        [MenuItem("Calluna/Packages/Samples and Tests Toggler")]
        public static void Open()
        {
            var win = GetWindow<PackageSamplesTestsToggler>("Packages: Samples/Tests");
            win.minSize = new Vector2(640, 360);
            win.Rescan();
            win.Show();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Rescan Packages"))
                Rescan();

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Hide ALL (add ~)", GUILayout.Height(24)))
                    ToggleAll(_packages, hide: true);

                if (GUILayout.Button("Show ALL (remove ~)", GUILayout.Height(24)))
                    ToggleAll(_packages, hide: false);
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "This tool renames 'Samples'↔'Samples~' and 'Tests'↔'Tests~' inside local packages in /Packages.\n" +
                "Unity hides folders with '~' in the Package Manager view.", MessageType.Info);

            EditorGUILayout.Space();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (var p in _packages)
            {
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    EditorGUILayout.LabelField(p.Name, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Path:", p.Path);
                    if (!p.HasPackageJson)
                    {
                        EditorGUILayout.HelpBox("No package.json found. Skipping (not a UPM package).",
                            MessageType.Warning);
                        continue;
                    }

                    DrawRow("Samples", p.SamplesVisible, p.SamplesHidden, () =>
                    {
                        if (p.SamplesVisible) ToggleFolder(p.Path, "Samples", hide: true);
                        else if (p.SamplesHidden) ToggleFolder(p.Path, "Samples", hide: false);
                    });

                    DrawRow("Tests", p.TestsVisible, p.TestsHidden, () =>
                    {
                        if (p.TestsVisible) ToggleFolder(p.Path, "Tests", hide: true);
                        else if (p.TestsHidden) ToggleFolder(p.Path, "Tests", hide: false);
                    });
                }
            }

            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField($"Last scan: {_lastScan:yyyy-MM-dd HH:mm:ss}", EditorStyles.miniLabel);
        }

        private void DrawRow(string label, bool visible, bool hidden, Action toggleAction)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var state = visible ? "Visible" : hidden ? "Hidden (~)" : "Not present";
                EditorGUILayout.LabelField(label + ":", GUILayout.Width(80));
                EditorGUILayout.LabelField(state);

                GUILayout.FlexibleSpace();

                using (new EditorGUI.DisabledScope(!(visible || hidden)))
                {
                    var btn = visible ? $"Hide {label} (add ~)" :
                        hidden ? $"Show {label} (remove ~)" : $"No {label}";
                    if (GUILayout.Button(btn, GUILayout.Width(200)))
                    {
                        toggleAction?.Invoke();
                        Rescan();
                    }
                }
            }
        }

        private void Rescan()
        {
            _packages = ScanPackages();
            _lastScan = DateTime.Now;
            Repaint();
        }

        // ------- Core logic -------

        private static List<Pkg> ScanPackages()
        {
            var list = new List<Pkg>();
            var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            var packagesRoot = Path.Combine(projectRoot, "Packages");

            if (!Directory.Exists(packagesRoot))
            {
                Debug.LogWarning("No /Packages folder found.");
                return list;
            }

            foreach (var dir in Directory.GetDirectories(packagesRoot))
            {
                var name = Path.GetFileName(dir);
                // skip Unity's lock/meta dirs
                if (name.StartsWith("."))
                    continue;

                var pkg = new Pkg
                {
                    Name = name,
                    Path = dir,
                    HasPackageJson = File.Exists(Path.Combine(dir, "package.json"))
                };

                pkg.SamplesVisible = Directory.Exists(Path.Combine(dir, "Samples"));
                pkg.SamplesHidden = Directory.Exists(Path.Combine(dir, "Samples~"));
                pkg.TestsVisible = Directory.Exists(Path.Combine(dir, "Tests"));
                pkg.TestsHidden = Directory.Exists(Path.Combine(dir, "Tests~"));

                list.Add(pkg);
            }

            // Put real packages (with package.json) first in the UI
            return list.OrderByDescending(p => p.HasPackageJson).ThenBy(p => p.Name).ToList();
        }

        private static void ToggleAll(IEnumerable<Pkg> pkgs, bool hide)
        {
            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (var p in pkgs.Where(p => p.HasPackageJson))
                {
                    ToggleFolder(p.Path, "Samples", hide);
                    ToggleFolder(p.Path, "Tests", hide);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }

            var action = hide ? "Hidden (~) " : "Shown ";
            Debug.Log($"{action} Samples/Tests for all local packages in /Packages.");
        }

        private static void ToggleFolder(string packagePath, string baseName, bool hide)
        {
            // baseName is "Samples" or "Tests"
            var visible = Path.Combine(packagePath, baseName);
            var hidden = Path.Combine(packagePath, baseName + "~");

            try
            {
                if (hide)
                {
                    if (Directory.Exists(visible))
                    {
                        MoveDirWithMeta(visible, hidden);
                        Debug.Log($"Renamed: {visible} -> {hidden}");
                    }
                }
                else
                {
                    if (Directory.Exists(hidden))
                    {
                        MoveDirWithMeta(hidden, visible);
                        Debug.Log($"Renamed: {hidden} -> {visible}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to toggle '{baseName}' in package '{packagePath}': {ex.Message}");
            }
            finally
            {
                AssetDatabase.Refresh();
            }
        }

        private static void MoveDirWithMeta(string src, string dst)
        {
            // Ensure destination doesn’t exist
            if (Directory.Exists(dst))
            {
                throw new IOException($"Destination already exists: {dst}");
            }

            FileUtil.MoveFileOrDirectory(src, dst);

            var srcMeta = src + ".meta";
            var dstMeta = dst + ".meta";

            // Never leave a .meta file for a ~ folder — Unity ignores ~ folders entirely,
            // so a Foo~.meta with no visible Foo~ causes "meta file exists but folder can’t be found" warnings.
            if (dst.EndsWith("~"))
            {
                // Hiding: delete the .meta rather than renaming it to Foo~.meta
                if (File.Exists(srcMeta))
                    FileUtil.DeleteFileOrDirectory(srcMeta);
            }
            else
            {
                // Showing: move .meta if FileUtil didn’t handle it; delete any leftover ~.meta
                if (File.Exists(srcMeta) && !File.Exists(dstMeta))
                    FileUtil.MoveFileOrDirectory(srcMeta, dstMeta);
                else if (File.Exists(srcMeta))
                    FileUtil.DeleteFileOrDirectory(srcMeta);
            }
        }
    }
}