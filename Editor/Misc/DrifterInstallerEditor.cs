using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using PackageManager = UnityEditor.PackageManager;

namespace Drifter.Editor.Misc
{
    public class DrifterInstallerEditor : EditorWindow
    {
        private static readonly HashSet<(string displayName, string name, 
            string version, System.Action onInstall)> PACKAGES = new()
        {
            ("Newtonsoft Json", "com.unity.nuget.newtonsoft-json", "3.0.2", 
                new(InstallNewtonsoftJson)),

            ("Naughty Attributes", "com.unity.naughty.attributes", "2.1.4", 
                new(InstallNaughtyAttributes)),
        };

        private static void InstallNewtonsoftJson() => 
            Debug.Log("Installing Newtonsoft Json");

        private static void InstallNaughtyAttributes() => 
            Debug.Log("Installing Naughty Attributes");

        private static readonly Vector2Int MIN_SIZE = new()
        {
            x = 500,
            y = 400,
        };

        [MenuItem("Tools/Drifter/Help/Open Installer")]
        [Shortcut("drifter-installer", 
            defaultKeyCode: KeyCode.F10, defaultShortcutModifiers: ShortcutModifiers.Control)]
        public static void Init()
        {
            var window = GetWindow<DrifterInstallerEditor>();
            window.titleContent = new GUIContent("Drifter Installer");
            window.minSize = MIN_SIZE;
            window.ShowModalUtility();
        }

        // AssetDatabase.ImportPackage(package, false);

        private const string k_AssetPath = "UI/Installer";

        private void CreateGUI()
        {
            var root = new VisualElement();

            var treeAsset = Resources.Load<VisualTreeAsset>(k_AssetPath);
            root = treeAsset.CloneTree();

            var packageContainer = root.Q("packageContainer");

            for (int i = 0; i < packageContainer.childCount; i++)
                packageContainer.RemoveAt(i);

            foreach (var package in PACKAGES)
                packageContainer.Add(CreatePackageElement(package.displayName, package.name, package.version, IsPackageInstalled(package.name, package.version), package.onInstall));

            rootVisualElement.Add(root);
        }

        private bool IsPackageInstalled(string name, string version) => false;

        private VisualElement CreatePackageElement(string displayName, string name, string version, bool isInstalled, System.Action onInstall)
        {
            var root = new VisualElement();

            root.style.flexDirection = FlexDirection.Row;
            root.style.flexWrap = Wrap.Wrap;
            root.style.justifyContent = Justify.SpaceBetween;
            root.style.alignItems = Align.Center;

            var displayNameLabel = new Label(displayName)
            {
                name = "displayName",
            };

            displayNameLabel.AddToClassList("label");

            var nameLabel = new Label(name)
            {
                name = "name",
            };

            nameLabel.AddToClassList("label");

            var versionLabel = new Label(version)
            {
                name = "versionLabel",
            };

            versionLabel.AddToClassList("label");

            var installBtn = new Button(onInstall)
            {
                name = "installBtn",
                text = isInstalled ? "Installed" : "Not Installed",
            };

            installBtn.AddToClassList("label");
            installBtn.AddToClassList(isInstalled ? "installButton" : "notInstallButton");

            root.Add(displayNameLabel);
            root.Add(nameLabel);
            root.Add(versionLabel);
            root.Add(installBtn);

            root.AddToClassList("package");

            return root;
        }
    }
}