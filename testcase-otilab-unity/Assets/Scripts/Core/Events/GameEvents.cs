using UnityEngine;
using System;

namespace Core.Events
{
    public static class GameEvents
    {
        public static event Action OnGameStart;
        public static event Action OnGamePause;
        public static event Action OnGameResume;
        public static event Action OnGameFail;
        public static event Action OnGameRestart;
        public static event Action<int> OnScoreUpdate;
        public static event Action<int> OnHealthUpdate;
        public static event Action<int> OnCarValueUpdate;

        public static void TriggerGameStart() => OnGameStart?.Invoke();
        public static void TriggerGamePause() => OnGamePause?.Invoke();
        public static void TriggerGameResume() => OnGameResume?.Invoke();
        public static void TriggerGameFail() => OnGameFail?.Invoke();
        public static void TriggerGameRestart() => OnGameRestart?.Invoke();
        public static void TriggerScoreUpdate(int score) => OnScoreUpdate?.Invoke(score);
        public static void TriggerHealthUpdate(int health) => OnHealthUpdate?.Invoke(health);
        public static void TriggerCarValueUpdate(int value) => OnCarValueUpdate?.Invoke(value);
    }
} 