using System;
using System.Collections;
using UnityEngine;

/// <summary>Static helpers shared across the project.</summary>
public static class Utils
{
    /// <summary>Invokes the action after the given seconds, unless the behaviour was destroyed meanwhile. Usage: this.Wait(2f, () => ...);</summary>
    public static void Wait(this MonoBehaviour mb, float seconds, Action action) => mb.StartCoroutine(WaitRoutine(mb, seconds, action));

    static IEnumerator WaitRoutine(MonoBehaviour mb, float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        if (mb) action?.Invoke();
    }

    /// <summary>Formats a counter as "used / total" for UI labels.</summary>
    public static string FormatCounter(int used, int total) => $"{used} / {total}";

    /// <summary>Wraps text in a TMP color tag. Usage: "text".Colored("green")</summary>
    public static string Colored(this string text, string color) => $"<color={color}>{text}</color>";

    /// <summary>Shows a message in the in-world log. Reuses an inactive slot if available, otherwise recycles the oldest message.</summary>
    public static void SendLogMessage(string text)
    {
        var logMessages = GlobalReferences.Instance.LogMessages;

        foreach (var logMessage in logMessages)
        {
            if (logMessage.gameObject.activeSelf) continue;
            logMessage.Display(text);
            return;
        }

        // All slots are in use: the oldest message is the first sibling
        logMessages[0].transform.parent.GetChild(0).GetComponent<LogMessage>().Display(text);
    }
}
