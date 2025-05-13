using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSliderSetup : MonoBehaviour
{
    [SerializeField] private GameObject sliderPrefab;
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private Vector3 sliderOffset = new Vector3(0, 1.5f, 0);

    private static HealthSliderSetup instance;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Health[] healthComponents = FindObjectsOfType<Health>();

        foreach(Health health in healthComponents)
        {
            CreateHealthSlider(health);
        }
    }

    void CreateHealthSlider(Health health)
    {

        GameObject sliderobj = Instantiate(sliderPrefab, worldCanvas.transform);

        //position slider
        sliderobj.transform.position = health.transform.position + sliderOffset;

        //allow the slider to follow the unit
        FollowTarget followScript = sliderobj.AddComponent<FollowTarget>();
        followScript.target = health.transform;
        followScript.offset = sliderOffset;

        //connect slider to health component
        Slider slider = sliderobj.GetComponent<Slider>();
        health.SetHealthSlider(slider);
    }

    public static void AttachSliderTo(Health health)
    {
        if (instance != null)
        {
            instance.CreateHealthSlider(health);
        }
        else
        {
            Debug.Log("Health slider setup instancd not found");
        }
    }
}
