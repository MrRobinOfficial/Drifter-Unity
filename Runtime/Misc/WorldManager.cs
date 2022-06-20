using UnityEngine;

namespace Drifter.Misc
{
    [AddComponentMenu("Tools/Drifter/Misc/World Manager [Misc]"), DisallowMultipleComponent]
    public class WorldManager : MonoBehaviour
    {
        private static WorldManager _instance;

        public static WorldManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("Drifter [Runtime]");
                    _instance = obj.AddComponent<WorldManager>();

                    DontDestroyOnLoad(obj);
                }

                return _instance;
            }
        }

        [field: SerializeField] public float AirDensity { get; set; } = 1.225f;
    }
}