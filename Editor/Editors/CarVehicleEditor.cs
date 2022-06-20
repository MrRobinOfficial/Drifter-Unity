using Drifter.Vehicles;
using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Drifter.Editor.Editors
{
    //[CustomEditor(typeof(CarVehicle), editorForChildClasses: true)]
    //[CanEditMultipleObjects]
    //public class CarVehicleEditor : NaughtyInspector
    //{
    //    private const string UXML_PATH = "UI/CarInspectorUI";
    //    private const string USS_PATH = "UI/CarInspectorUI";

    //    //public override VisualElement CreateInspectorGUI()
    //    //{
    //    //    var root = new VisualElement();

    //    //    var asset = Resources.Load<VisualTreeAsset>(UXML_PATH);

    //    //    root = asset.CloneTree();

    //    //    var styleSheet = Resources.Load<StyleSheet>(USS_PATH);
    //    //    root.styleSheets.Add(styleSheet);

    //    //    var IMGUI = new IMGUIContainer(() =>
    //    //    {
    //    //        var editor = CreateEditor(target);
    //    //        editor.DrawDefaultInspector();
    //    //    });

    //    //    var container = root.Q("container");
    //    //    container.Add(IMGUI);

    //    //    return root;
    //    //}

    //    //public override void OnInspectorGUI() => base.OnInspectorGUI();
    //}
}