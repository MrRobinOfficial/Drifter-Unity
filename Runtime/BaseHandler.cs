using System.Collections;
using UnityEngine;

namespace Drifter
{
    public abstract class BaseHandler : MonoBehaviour
    {
        protected virtual void Awake() => hideFlags = HideFlags.NotEditable;
    }
}