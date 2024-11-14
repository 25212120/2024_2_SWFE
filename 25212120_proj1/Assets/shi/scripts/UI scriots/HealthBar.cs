using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private StatData statData;
    [SerializeField] private Slider healthSlider;

    void Start()
    {
        healthSlider.maxValue = statData.hpMax;
        healthSlider.value = statData.HpCurrent;
    }

    void Update()
    {
        healthSlider.value = statData.HpCurrent;
    }
}
