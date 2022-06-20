using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityTime = UnityEngine.Time;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Drifter.Samples.RaceSystem
{
#if UNITY_EDITOR
    [CustomEditor(typeof(RaceManager))]
    public class RaceManagerEditor : UnityEditor.Editor
    {
        public override bool RequiresConstantRepaint() => Application.isPlaying;

        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                base.OnInspectorGUI();
                return;
            }

            var style1 = new GUIStyle(EditorStyles.label)
            {
                normal = new GUIStyleState
                {
                    textColor = Color.yellow,
                },
                fontStyle = FontStyle.Bold,
                fontSize = 14,
            };

            var manager = (RaceManager)target;

            EditorGUI.BeginDisabledGroup(disabled: true);

            for (int i = 0; i < manager.Racers.Count; i++)
                DrawRacer(manager.Racers[i]);

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            void DrawRacer(Racer racer)
            {
                EditorGUILayout.LabelField($"{racer.name}", style1);
                EditorGUI.indentLevel++;
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"Is Finished: {racer.IsFinished}");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Lap Times:");
                EditorGUI.indentLevel++;

                for (int i = 0; i < racer.LapTimes.Count; i++)
                {
                    var lapTime = racer.LapTimes[i];

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField($"Index={i}, {lapTime.AsString()}");
                }

                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
        }
    }
#endif

    public class RaceManager : MonoBehaviour
    {
        public static event UnityAction OnRaceStarted;
        public static event UnityAction OnRaceEnded;

        public static RaceManager Instance { get; private set; } = null;

        [field: Header("References")]
        [field: SerializeField] public Transform CheckpointArray { get; private set; } = default;
        [SerializeField] BaseVehicle[] m_RacerArray = default;

        [Header("General Settings")]
        [SerializeField, Tooltip("DontDestroyOnLoad")] bool m_DontDestroy = true;
        [field: SerializeField] public int LapCount { get; private set; } = 3;

        [field: Header("SFX Settings")]
        [field: SerializeField] public UnityEngine.Audio.AudioMixerGroup AudioMixerGroup { get; private set; } = default;
        [field: SerializeField] public AudioClip CheckpointSFX { get; private set; } = default;

        private Checkpoint[] _checkpoints = default;
        private List<Racer> _racers = new(capacity: 10);

        public IReadOnlyList<Racer> Racers => _racers;

        /// <summary>
        /// Add racer into competition
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool TryAddRacer(BaseVehicle vehicle, out Racer racer)
        {
            if (InProgress)
            {
                racer = default;
                return false;
            }

            racer = vehicle.gameObject.AddComponent<Racer>();
            racer.Init(vehicle);
            _racers.Add(racer);

            return racer != null;
        }

        private void OnDrawGizmos()
        {
            if (CheckpointArray == null)
                return;

            const float THICKNESS = 2f;

            var lastChild = CheckpointArray.GetChild(0);

            for (int i = 0; i < CheckpointArray.childCount; i++)
            {
                var child = CheckpointArray.GetChild(i);

                Handles.color = i == 0 ? Color.red : Color.green;
                Handles.DrawLine(lastChild.position, child.position, THICKNESS);

                lastChild = child;
            }
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            if (m_DontDestroy)
                DontDestroyOnLoad(gameObject);

            _checkpoints = new Checkpoint[CheckpointArray.childCount];

            for (int i = 0; i < _checkpoints.Length; i++)
            {
                var obj = CheckpointArray.GetChild(i).gameObject.AddComponent<Checkpoint>();

                obj.Init(i);

                _checkpoints[i] = obj;
            }
        }

        private void OnEnable()
        {
            for (int i = 0; i < m_RacerArray.Length; i++)
                TryAddRacer(m_RacerArray[i], out _);
        }

        private void OnDisable()
        {

        }

        [ShowNativeProperty] public bool InProgress { get; private set; } = false;
        [ShowNativeProperty] public double Timer { get; private set; } = default;

        [ContextMenu("Manager/Start Race")]
        public async void StartRace()
        {
            if (InProgress)
                return;

            InProgress = true;

            const int SECONDS = 3;

            //for (int i = SECONDS - 1; i >= 0; i--)
            //{
            //    print($"{i + 1}");
            //    await UniTask.Delay(1000);
            //}

            print("GO!");

            for (int i = 0; i < _racers.Count; i++)
                _racers[i].Enable();

            OnRaceStarted?.Invoke();
        }

        [ContextMenu("Manager/End Race")]
        public async void EndRace()
        {
            if (!InProgress)
                return;

            InProgress = false;

            for (int i = 0; i < _racers.Count; i++)
                _racers[i].Disable();

            for (var i = 0; i < _checkpoints.Length; i++)
                Destroy(_checkpoints[i]);

            for (var i = 0; i < _racers.Count; i++)
                Destroy(_racers[i]);

            //await UniTask.Delay(500);

            OnRaceEnded?.Invoke();
        }

        private void Update()
        {
            if (Keyboard.current[Key.F1].wasPressedThisFrame && !InProgress)
                StartRace();

            if (Keyboard.current[Key.F2].wasPressedThisFrame && InProgress)
                EndRace();

            if (InProgress)
                UpdateRace();
        }

        private void UpdateRace()
        {
            Timer += UnityTime.deltaTime;
        }
    }
}