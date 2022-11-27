#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using Drifter.Components;

namespace Drifter.Extensions
{
    public static partial class DrifterEditorExtensions
    {
        private static void DrawWireCylinder(Vector3 pos, Vector3 dir, float radius, float height)
        {
            var halfHeight = height * 0.5f;
            var quaternion = Quaternion.LookRotation(dir, new Vector3(0f - dir.y, dir.x, 0f));

            Gizmos.color = Color.cyan;

            Gizmos.DrawLine(pos + quaternion * new Vector3(radius, 0f, halfHeight), 
                pos + quaternion * new Vector3(radius, 0f, 0f - halfHeight));

            Gizmos.DrawLine(pos + quaternion * new Vector3(0f - radius, 0f, halfHeight), 
                pos + quaternion * new Vector3(0f - radius, 0f, 0f - halfHeight));

            Gizmos.DrawLine(pos + quaternion * new Vector3(0f, radius, halfHeight), 
                pos + quaternion * new Vector3(0f, radius, 0f - halfHeight));

            Gizmos.DrawLine(pos + quaternion * new Vector3(0f, 0f - radius, halfHeight), 
                pos + quaternion * new Vector3(0f, 0f - radius, 0f - halfHeight));

            Gizmos.color = Color.green;

            for (var theta = 0f; theta < Mathf.PI * 2f; theta += 0.1f)
            {
                var from = pos + quaternion * new Vector3(Mathf.Sin(theta) * radius, 
                    Mathf.Cos(theta) * radius, halfHeight);

                var to = pos + quaternion * new Vector3(Mathf.Sin(theta + 0.1f) * radius, 
                    Mathf.Cos(theta + 0.1f) * radius, halfHeight);

                Gizmos.DrawLine(from, to);

                var from2 = pos + quaternion * new Vector3(Mathf.Sin(theta) * radius, 
                    Mathf.Cos(theta) * radius, 0f - halfHeight);

                var to2 = pos + quaternion * new Vector3(Mathf.Sin(theta + 0.1f) * radius, 
                    Mathf.Cos(theta + 0.1f) * radius, 0f - halfHeight);

                Gizmos.DrawLine(from2, to2);
            }
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy | GizmoType.Active)]
        private static void DrawWheelGizmo(WheelBehaviour wheel, GizmoType gizmoTyp)
        {
            const float THICKNESS = 1.5f;

            // DRAW DISC //
            DrawWireCylinder(wheel.transform.position, wheel.transform.right, wheel.Radius, wheel.Width);
        }

        //void DrawDisc()
        //{
        //    Handles.color = Color.green;
        //    Handles.DrawWireDisc(wheel.GetWorldPosition(), wheel.transform.right, wheel.Radius, THICKNESS);
        //}

        //void DrawDropLine()
        //{
        //    //Handles.color = Color.magenta;

        //    //var startPoint = GetOffsetPosition();
        //    //var endPoint = GetWorldPositon();

        //    //Handles.DrawLine(startPoint, endPoint);
        //}

        //void DrawCenterLine()
        //{
        //    return;

        //    if (wheel.Visual == null)
        //        return;

        //    var visual = wheel.Visual;
        //    Handles.color = Color.yellow;

        //    var length = wheel.Radius * 0.8f;
        //    var dir = visual.forward;

        //    var startPoint = wheel.GetWorldPosition() - dir * length;
        //    var endPoint = wheel.GetWorldPosition() + dir * length;

        //    Handles.DrawLine(startPoint, endPoint, THICKNESS);
        //}

        //void DrawWidthLine()
        //{
        //    Handles.color = Color.cyan;

        //    var length = wheel.Width;
        //    var dir = wheel.transform.right;

        //    var startPoint = wheel.GetWorldPosition() - dir * length;
        //    var endPoint = wheel.GetWorldPosition() + dir * length;

        //    Handles.DrawLine(startPoint, endPoint, THICKNESS);
        //}
    }
}

#endif