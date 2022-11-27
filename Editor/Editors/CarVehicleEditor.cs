using Drifter.Vehicles;
using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Drifter.Editor.Editors
{
    [CustomEditor(typeof(CarVehicle), editorForChildClasses: true), CanEditMultipleObjects]
    public class CarVehicleEditor : BaseVehicleEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}