using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationUI : MonoBehaviour
{

    public TMP_Text notificationText;
    public float displayTime = 3f;

    private Coroutine currentRoutine;
    public static NotificationUI Instance;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (notificationText != null)
            notificationText.text = "";
    }

    public void ShowMessage(string message, Color? color = null)
    {
        if (notificationText == null) return;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        if (color.HasValue)
            notificationText.color = color.Value;

        currentRoutine = StartCoroutine(DisplayMessageRoutine(message));
    }

    private IEnumerator DisplayMessageRoutine(string message)
    {
        notificationText.text = message;
        yield return new WaitForSeconds(displayTime);
        notificationText.text = "";
    }
}
