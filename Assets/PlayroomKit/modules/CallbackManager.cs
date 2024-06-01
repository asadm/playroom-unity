using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Playroom
{
    public static class CallbackManager
    {
        private static Dictionary<string, Delegate> callbacks = new();

        public static string RegisterCallback(Action callback, string key = null)
        {
            if (string.IsNullOrEmpty(key))
                key = GenerateKey();


            if (!callbacks.ContainsKey(key))
            {
                callbacks.Add(key, callback);
            }
            else
            {
                callbacks[key] = callback;
            }

            return key;
        }


        public static void InvokeCallback(string key)
        {
            if (callbacks.TryGetValue(key, out Delegate callback))
            {
                (callback as Action)?.Invoke();
            }
            else
            {
                Debug.LogError($"Callback with key {key} not found!");
            }
        }

        private static string GenerateKey()
        {
            return Guid.NewGuid().ToString();
        }



        /// <summary>
        /// Calls an external method to convert a string associated with a given key, called from the JS side only.
        /// </summary>
        /// <param name="key">The key associated with the string to be converted.</param>
        [DllImport("__Internal")]
        private static extern void ConvertString(string key);

    }
}