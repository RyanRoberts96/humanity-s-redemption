using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQDestroyed : MonoBehaviour
{
    private void OnDestroy()
    {
        Debug.Log("AI HQ destroyed! You win the game!");
        // Trigger your win condition here
        GameManager.Instance.LoseGame();
    }
}