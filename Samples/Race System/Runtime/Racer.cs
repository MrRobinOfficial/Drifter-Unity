using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Drifter.Samples.RaceSystem
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class Racer : MonoBehaviour
    {
        public event UnityAction OnLapCompleted;

        public bool IsFinished { get; private set; } = false;

        private List<Checkpoint> _checkpoints = new();
        private BaseVehicle _vehicle = default;
        private AudioSource _sfxSource = default;

        public void Init(BaseVehicle vehicle)
        {
            _sfxSource = GetComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
            _sfxSource.outputAudioMixerGroup = RaceManager.Instance.AudioMixerGroup;

            RaceManager.OnRaceEnded += OnRaceEnded;

            _checkpoints.Clear();
            _lapCount = 0;
            IsFinished = false;
            _lapTimes = new(RaceManager.Instance.LapCount);

            hideFlags = HideFlags.NotEditable;
            _vehicle = vehicle;

            Disable();
        }

        private void OnRaceEnded()
        {
            RaceManager.OnRaceEnded -= OnRaceEnded;

            if (!IsFinished)
                print("DNF!");
        }

        public void AddCheckpoint(Checkpoint ctx)
        {
            if (_checkpoints.Contains(ctx))
            {
                if (ctx.IsFinale)
                    HandleFinishline();

                return;
            }

            if (_checkpoints.Count > ctx.Index + 1)
            {
                Debug.LogError("Wrong checkpoint! Turn back!");
                return;
            }

            _checkpoints.Add(ctx);

            _sfxSource.PlayOneShot(RaceManager.Instance.CheckpointSFX);

            void HandleFinishline()
            {
                _checkpoints.Clear();

                var timer = RaceManager.Instance.Timer;
                var sumTime = _lapCount > 0 ? _lapTimes.Sum(x => x.TotalSeconds) : 0f;

                var lapTime = TimeSpan.FromSeconds(timer - sumTime);
                _lapTimes.Add(lapTime);

                OnLapCompleted?.Invoke();

                _lapCount++;

                if (_lapCount >= RaceManager.Instance.LapCount)
                {
                    _lapCount = RaceManager.Instance.LapCount; // Clamped

                    var totalTime = new TimeSpan(_lapTimes.Sum(x => x.Ticks));
                    var bestTime = new TimeSpan(_lapTimes.Min(x => x.Ticks));

                    print("You have finish!");
                    print($"Total Time: {totalTime.AsString()}");
                    print($"Best Time: {bestTime.AsString()}");

                    IsFinished = true;

                    Disable();
                }
            }
        }

        private int _lapCount = 0;
        private List<TimeSpan> _lapTimes = new();

        public IReadOnlyList<TimeSpan> LapTimes => _lapTimes;

        public void Enable() => _vehicle.IsDriveable = true;

        public void Disable() => _vehicle.IsDriveable = false;
    }
}