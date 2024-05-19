using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{

    [SerializeField] Health health;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI healthText;

    int maxHealth;

    void Awake()
    {
        maxHealth = health.maxHealth;
        health.CurrentHealth.AddListener(UpdateUI);
    }

    void UpdateUI(int currentHealth)
    {
        slider.value = (float) currentHealth / maxHealth;
        healthText.text = $"{currentHealth}/{maxHealth}";
    }

}
