using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float attackPower;  // 무기의 공격력

    // 무기를 EquipmentItem으로 변환하여 추가하는 함수
    public void EquipToInventory(EquipmentInventory inventory)
    {
        // EquipmentItem을 생성
        EquipmentEffect effect = new EquipmentEffect();
        effect.Attack = attackPower;  // 무기의 공격력 효과 설정

        // 새 EquipmentItem 생성
        EquipmentItem newWeapon = new EquipmentItem("Weapon: " + gameObject.name, effect);

        // EquipmentInventory에 장비 추가
        inventory.AddEquipment(newWeapon);

        
        Debug.Log($"{gameObject.name} has been equipped.");
    }
}
