using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using UnityEngine;
namespace Umeshu.USystem.Time
{
    public sealed class TimeManager : GameSystem<TimeManager>
    {

        #region Game Element Methods

        protected override void SystemFirstInitialize()
        {
            // Init threads
            foreach (TimeThread _item in UfEnum.GetEnumArray<TimeThread>())
            {
                GetThread(_item);
            }

            for (int _i = 0; _i < DEPENDENCIES.GetLength(0); _i++)
            {
                TimeThread _parent = DEPENDENCIES[_i, 0];
                TimeThread _child = DEPENDENCIES[_i, 1];
                GetThread(_child).SetParent(GetThread(_parent));
            }
            GetThread(TimeThread.Difficulty).SetSpeedMultiplier(1f);
        }
        protected override void SystemEnableAndReset() { }
        protected override void SystemPlay() { }
        protected override void SystemUpdate() { }

        #endregion


        public static float DeltaTime => GetDeltaTime(0);
        public static float Time => GetTime(0);
        public static float RealTime => UnityEngine.Time.time;
        public static float RealDeltaTime => UnityEngine.Time.deltaTime;
        public static float RealUnscaledDeltaTime => UnityEngine.Time.deltaTime;
        public static float RealUnscaledTime => UnityEngine.Time.unscaledTime;

        public static UEvent<float> onTimeSpeedChange = new();

        public static bool inSlowedTime = false;
        public static bool TimePaused => Instance?.Paused ?? true;

        public static float TimeSpeedMultiplier
        {
            get => TimePaused ? 0 : speedMultiplier;
            private set
            {
                speedMultiplier = value;
                onTimeSpeedChange?.Invoke(speedMultiplier);
            }
        }
        private static float speedMultiplier = 1;

        public static Dictionary<TimeThread, TimeManager_Thread> threads = new();

        private static TimeThread[,] DEPENDENCIES =>
            new TimeThread[,] {
            { TimeThread.Difficulty, TimeThread.Player }
            };

        protected override void UnitySystemUpdate()
        {
            base.UnitySystemUpdate();
            foreach (TimeThread _key in threads.Keys)
            {
                TimeManager_Thread _thread = threads[_key];
                _thread.UpdateTimeSpeedMultiplier();
                _thread.SetDeltaTime(RealDeltaTime * TimeSpeedMultiplier * _thread.SpeedMultiplier);
                _thread.SetTime(_thread.Time + _thread.DeltaTime);
            }
        }

        /// <summary>
        /// Equivalent to <see cref ="TimeManager.Instance.Pause()"/>
        /// </summary>
        public static void PauseTime() => Instance.Pause();
        /// <summary>
        /// Equivalent to <see cref ="TimeManager.Instance.Resume()"/>
        /// </summary>
        public static void ResumeTime() => Instance.Resume();

        #region Get/Set Values

        public static TimeManager_Thread GetThread(TimeThread _thread)
        {
            if (!threads.ContainsKey(_thread))
            {
                threads.Add(_thread, new(_thread));
            }

            return threads[_thread];
        }

        public static void SuscribeToSpeedChange(TimeThread _thread, Action<float> _action)
        {
            onTimeSpeedChange += _action.Invoke;
            GetThread(_thread).onTimeSpeedChange += _action;
        }

        public static void UnsuscribeToSpeedChange(TimeThread _thread, Action<float> _action)
        {
            onTimeSpeedChange -= _action.Invoke;
            GetThread(_thread).onTimeSpeedChange -= _action;
        }

        public static float GetDeltaTime(TimeThread _thread) => GetThread(_thread).DeltaTime;

        public static float GetTime(TimeThread _thread) => GetThread(_thread).Time;


        public static void SuscribeModifier(TimeThread _thread, ITimeSpeedMultiplierModifier _modifier) => GetThread(_thread).SuscribeModifier(_modifier);
        public static void UnsuscribeModifier(TimeThread _thread, ITimeSpeedMultiplierModifier _modifier) => GetThread(_thread).UnsuscribeModifier(_modifier);

        public static float GetSpeedMultiplier(TimeThread _thread) =>
            GetThread(_thread).SpeedMultiplier * TimeSpeedMultiplier;


        #endregion

        public static IEnumerator WaitForEndOfFrame(TimeThread _thread = 0)
        {
            bool _wentInLoop = false;
            while (GetSpeedMultiplier(_thread) == 0)
            {
                _wentInLoop = true;
                yield return new WaitForEndOfFrame();
            }
            if (!_wentInLoop)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        public static IEnumerator WaitForTime(float _duration, TimeThread _thread = 0)
        {
            while (_duration > 0)
            {
                _duration = Mathf.MoveTowards(_duration, 0, GetDeltaTime(_thread));
                yield return WaitForEndOfFrame();
            }
        }

        public static void TriggerSlowedTime(float _intensity, TimeThread _thread = 0, bool _undoSlowedTime = false) => inSlowedTime = !_undoSlowedTime;

        public static IEnumerator WaitForRealTime(float _duration)
        {
            yield return new WaitForSeconds(_duration);
        }
    }

    public class TimeManager_Thread
    {
        public static implicit operator TimeManager_Thread(TimeThread _thread) => TimeManager.GetThread(_thread);
        public static implicit operator TimeManager_Thread(GameElement _gameElement) => TimeManager.GetThread(_gameElement.Thread);
        public static implicit operator TimeManager_Thread(ScriptableElement _scriptableElement) => TimeManager.GetThread(_scriptableElement.Thread);

        public UEvent<float> onTimeSpeedChange;

        public TimeManager_Thread(TimeThread _thread) => Thread = _thread;

        public TimeThread Thread { get; private set; }
        public float DeltaTime { get; private set; }
        public float Time { get; private set; }
        public TimeManager_Thread ParentThread { get; private set; }
        public float SpeedMultiplier => speedMultiplier * speedMultiplierFromModifiers * GetParentSpeedMultiplier();
        private float speedMultiplier = 1;
        private float speedMultiplierFromModifiers = 1;


        #region Modifiers
        private readonly List<ITimeSpeedMultiplierModifier> timeSpeedMultiplierModifiers = new();
        internal void SuscribeModifier(ITimeSpeedMultiplierModifier _modifier) => timeSpeedMultiplierModifiers.Add(_modifier);
        internal void UnsuscribeModifier(ITimeSpeedMultiplierModifier _modifier) => timeSpeedMultiplierModifiers.Remove(_modifier);
        #endregion

        internal void SetDeltaTime(float _value) => DeltaTime = _value;

        internal void SetTime(float _value) => Time = _value;

        internal void SetSpeedMultiplier(float _value)
        {
            if (speedMultiplier == _value) return;

            speedMultiplier = _value;
            SendEvent();
        }

        public void TransferSpeedMultiplierChange(float _value) => SendEvent();

        public void SetParent(TimeManager_Thread _parentThread)
        {
            if (ParentThread != null)
            {
                ParentThread.onTimeSpeedChange -= TransferSpeedMultiplierChange;
            }

            ParentThread = _parentThread;
            if (ParentThread != null)
            {
                ParentThread.onTimeSpeedChange += TransferSpeedMultiplierChange;
            }

            SendEvent();
        }

        private void SendEvent() =>
            onTimeSpeedChange?.Invoke(SpeedMultiplier * TimeManager.TimeSpeedMultiplier);

        private float GetParentSpeedMultiplier() => ParentThread != null ? ParentThread.SpeedMultiplier : 1;

        internal void UpdateTimeSpeedMultiplier()
        {
            float _wantedSpeedMultiplier = 1;
            if (timeSpeedMultiplierModifiers.Count > 0)
            {
                float _lowest = float.MaxValue;
                foreach (ITimeSpeedMultiplierModifier _modifier in timeSpeedMultiplierModifiers)
                    _lowest = Mathf.Min(_lowest, _modifier.GetWantedSpeedMultiplier());
                _wantedSpeedMultiplier = _lowest;
            }

            if (speedMultiplierFromModifiers != _wantedSpeedMultiplier)
            {
                speedMultiplierFromModifiers = _wantedSpeedMultiplier;
                SendEvent();
            }
        }
    }
}