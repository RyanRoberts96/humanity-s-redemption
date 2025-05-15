using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth = 20;
    public int CurrentHealth { get; private set;}
    public Slider healthSlider;

    void Start()
    {
        CurrentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = CurrentHealth;
        }
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        Debug.Log(gameObject.name + " took " + amount + " damage. Remaining health is: " + CurrentHealth);

        if (healthSlider != null)
        {
            healthSlider.value = CurrentHealth;
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has died.");

        if (NotificationUI.Instance != null)
        {
            string unitName = gameObject.name;
            Vector3 position = transform.position;
            string posString = $"({position.x:F1}, {position.y:F1}, {position.z:F1})";
               
            NotificationUI.Instance.ShowMessage($"{unitName} has been destroyed at: {posString}", Color.yellow);
        }
        Destroy(gameObject);
    }

    public void SetHealthSlider(Slider slider)
    {
        healthSlider = slider;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = CurrentHealth;
        }
    }
    public void SetHealth(int value)
    {
        CurrentHealth = Mathf.Clamp(value, 0, maxHealth);
        if (healthSlider != null)
        {
            healthSlider.value = CurrentHealth;
        }
    }
}
