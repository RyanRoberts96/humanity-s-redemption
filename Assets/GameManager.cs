using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void WinGame()
    {
        StartCoroutine(ShowWinningMessageLoop());
        NotificationUI.Instance.ShowMessage("You won!, You won!, You won!", Color.green);

    }
    private IEnumerator ShowWinningMessageLoop()
    {
        while (true)
        {
            NotificationUI.Instance.ShowMessage("You won!, You won!, You won!", Color.green);
            yield return new WaitForSeconds(2f); // Show message every 2 seconds (adjust as needed)
        }
    }
}
