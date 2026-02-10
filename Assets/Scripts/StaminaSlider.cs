using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private float maxStamina = 10f; // 1 minute of stamina
    [SerializeField] private float regenTime = 30f; // 5 minutes to recharge


    private float currentStamina;
    private bool isDepleted = false;

    void Start()
    {
        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = currentStamina;
    }

    void Update()
    {
        if (currentStamina < maxStamina)
        {
            RegenerateStamina(Time.deltaTime);
        }

        // Stay "depleted" until full stamina is restored
        if (isDepleted && currentStamina >= maxStamina)
        {
            isDepleted = false;
        }

        staminaSlider.value = currentStamina;
    }

    public void UseStamina(float amount)
    {
        if (isDepleted) return;

        currentStamina -= amount;
        if (currentStamina <= 0)
        {
            currentStamina = 0;
            isDepleted = true;
        }
    }

    public bool CanRun()
    {
        return !isDepleted;
    }

    private void RegenerateStamina(float deltaTime)
    {
        float regenRate = maxStamina / regenTime;
        currentStamina += regenRate * deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    public float GetMaxStamina()
    {
        return maxStamina;
    }

    public void RestoreStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }


}

