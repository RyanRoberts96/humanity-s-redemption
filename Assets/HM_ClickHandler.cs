using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HM_ClickHandler : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private void OnMouseDown()
    {
        // Toggle panel visibility
        panel.SetActive(true);
    }
}