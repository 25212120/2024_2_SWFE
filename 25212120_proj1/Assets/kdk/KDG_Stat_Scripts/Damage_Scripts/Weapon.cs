using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float attackPower;  // ������ ���ݷ�

    // ���⸦ EquipmentItem���� ��ȯ�Ͽ� �߰��ϴ� �Լ�
    public void EquipToInventory(EquipmentInventory inventory)
    {
        // EquipmentItem�� ����
        EquipmentEffect effect = new EquipmentEffect();
        effect.Attack = attackPower;  // ������ ���ݷ� ȿ�� ����

        // �� EquipmentItem ����
        EquipmentItem newWeapon = new EquipmentItem("Weapon: " + gameObject.name, effect);

        // EquipmentInventory�� ��� �߰�
        inventory.AddEquipment(newWeapon);

        
        Debug.Log($"{gameObject.name} has been equipped.");
    }
}
