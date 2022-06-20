using Drifter.Extensions;
using NaughtyAttributes;
using UnityEngine;

namespace Drifter.Misc
{
    [AddComponentMenu("Tools/Drifter/Misc/Physics Surface [Misc]"), DisallowMultipleComponent]
    public class PhysicsSurface : MonoBehaviour
    {
        [SerializeField] SurfaceType m_Type = default;
        [SerializeField, ShowIf(nameof(m_Type), SurfaceType.Custom), MinMaxSlider(0f, 1f)] Vector2 m_KineticFriction = new(0f, 1f);
        [SerializeField, ShowIf(nameof(m_Type), SurfaceType.Custom), MinMaxSlider(0f, 1f)] Vector2 m_StaticFriction = new(0f, 1f);

        public float GetFriction(bool isStatic)
        {
            var kineticFriction = (m_StaticFriction.x, m_StaticFriction.y);
            var staticFriction = (m_StaticFriction.x, m_StaticFriction.y);
            var friction = DrifterExtensions.GetFrictionSurface(isStatic, m_Type, staticFriction, kineticFriction);
            return Random.Range(friction.min, friction.max);
        }
    }
}