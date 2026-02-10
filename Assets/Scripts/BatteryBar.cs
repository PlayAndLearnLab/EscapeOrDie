using UnityEngine;
using UnityEngine.UI;

public class BatteryBar : MonoBehaviour
{
    [SerializeField] private Slider batterySlider;
    [SerializeField] private float maxBattery = 10f;
    private float currentBattery;

    public bool IsBatteryDepleted => currentBattery <= 0f;

    void Start()
    {
        currentBattery = maxBattery;
        batterySlider.maxValue = maxBattery;
        batterySlider.value = currentBattery;
    }

    void Update()
    {
        batterySlider.value = currentBattery;
    }

    public bool UseBattery(float amount)
    {
        if (IsBatteryDepleted) return false;

        currentBattery -= amount;
        currentBattery = Mathf.Clamp(currentBattery, 0, maxBattery);
        return true;
    }

    public void RestoreBattery(float amount)
    {
        currentBattery += amount;
        currentBattery = Mathf.Clamp(currentBattery, 0, maxBattery);
    }
}
