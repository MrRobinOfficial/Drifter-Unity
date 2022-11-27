using UnityEngine;

namespace Drifter
{
    public interface IHookable
    {
        public void Connect(Transform transform, bool overrideConnection);

        public void Disconnect();
    }
}