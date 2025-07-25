using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.Haptic {
    public static class Haptic {
        public enum FORCE {
            LIGHT = 0,
            MEDIUM = 1,
            HEAVY = 2,
        }

        private const string HAPTIC_KEY = "HapticEnabled";

        public static bool Enabled {
            get => PlayerPrefs.GetInt(HAPTIC_KEY, 1) > 0;
            set {
                PlayerPrefs.SetInt(HAPTIC_KEY, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void _TriggerHapticFeedback(int force);
#endif

        public static void Feedback(FORCE force) {
            if (!Enabled) return;

#if UNITY_EDITOR
            return;
#elif UNITY_IOS
            _TriggerHapticFeedback((int)force);
#elif UNITY_ANDROID
            switch (force) {
                case FORCE.LIGHT:
                    Vibrate(new long[] { 0, 50 }, -1);
                    break;
                case FORCE.MEDIUM:
                    Vibrate(new long[] { 0, 100 }, -1);
                    break;
                case FORCE.HEAVY:
                    Vibrate(new long[] { 0, 200 }, -1);
                    break;
                default:
                    Vibrate(new long[] { 0, 50 }, -1);
                    break;
            }
#endif
        }

        public static void Vibrate(long[] pattern, int repeat) {
            if (Application.platform == RuntimePlatform.Android) {
                using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                    using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
                        using (var vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator")) {
                            vibrator.Call("vibrate", pattern, repeat);
                        }
                    }
                }
            }
        }
    }
}