using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentEffect
{
    public float Attack;
    public float MovementSpeed;
    public float Defense;
}

[System.Serializable]
public class EquipmentItem
{
    public string itemName;
    public EquipmentEffect effect;

    public EquipmentItem(string name, EquipmentEffect effect)
    {
        itemName = name;
        this.effect = effect;
    }
}

public class EquipmentInventory : MonoBehaviour
{
    [SerializeField]
    private List<EquipmentItem> equipmentItems = new List<EquipmentItem>();

    // 현재 장착 중인 장비 효과
    public EquipmentEffect CurrentEquipmentEffect { get; private set; } = new EquipmentEffect();

    public void AddEquipment(EquipmentItem item)
    {
        equipmentItems.Add(item);
        UpdateCurrentEquipmentEffect();
    }

    public void RemoveEquipment(EquipmentItem item)
    {
        if (equipmentItems.Remove(item))
        {
            UpdateCurrentEquipmentEffect();
        }
    }

    private void UpdateCurrentEquipmentEffect()
    {
        CurrentEquipmentEffect = new EquipmentEffect();

        foreach (var item in equipmentItems)
        {
            CurrentEquipmentEffect.Attack += item.effect.Attack;
            CurrentEquipmentEffect.MovementSpeed += item.effect.MovementSpeed;
            CurrentEquipmentEffect.Defense += item.effect.Defense;
        }
    }
}
