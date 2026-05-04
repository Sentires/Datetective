using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheDates
{
    public class MessageLogger : MonoBehaviour
    {
        public string[] defaultMessages = Array.Empty<string>();

        private string GetFormattedMessage(string message) => $"[{name}]: {message}";
        
        public void CustomErrorLog(string message) {
            Debug.LogError(GetFormattedMessage(message));
        }
        public void CustomWarningLog(string message) {
            Debug.LogWarning(GetFormattedMessage(message));
        }
        public void CustomInfoLog(string message) {
            Debug.Log(GetFormattedMessage(message));
        }
        
        public void ErrorLog(int index) {
            if (!defaultMessages.IsWithinBounds(index)) return;
            Debug.LogError(GetFormattedMessage(defaultMessages[index]));
        }
        public void WarningLog(int index) {
            if (!defaultMessages.IsWithinBounds(index)) return;
            Debug.LogWarning(GetFormattedMessage(defaultMessages[index]));
        }
        public void InfoLog(int index) {
            if (!defaultMessages.IsWithinBounds(index)) return;
            Debug.Log(GetFormattedMessage(defaultMessages[index]));
        }
    }
}
