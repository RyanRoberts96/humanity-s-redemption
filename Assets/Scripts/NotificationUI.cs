using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationUI : MonoBehaviour
{

    public float displayTime = 3f;
    public static NotificationUI Instance;
    public GameObject notificationPrefab;
    public Transform notificationContainer;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowMessage(string message, Color? color = null)
    {
        GameObject newNotification = Instantiate(notificationPrefab, notificationContainer);

        TMP_Text textComponent = newNotification.GetComponentInChildren<TMP_Text>();
        textComponent.text = message;
        if (color.HasValue)
            textComponent.color = color.Value;

        StartCoroutine(RemoveAfterTime(newNotification, displayTime));
    }

    private IEnumerator RemoveAfterTime(GameObject notification, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(notification);
    }
}
