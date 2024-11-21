using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentEffect
{
    public float Attack;           // ���ݷ�
    public float MovementSpeed;    // �̵� �ӵ�
    public float Defense;          // ����
}

[System.Serializable]
public class EquipmentItem
{
    public string itemName;        // ������ �̸�
    public EquipmentEffect effect; // �������� ȿ��
    public int level = 1;             // ����� ���� (�⺻ ������ 1)
    public float experience = 0f;     // ����� ����ġ


    [Header("������ �� �����Ǵ� ȿ��")]
    public float attackIncreasePerLevel = 2f;           // ������ �� ���ݷ� ������
    public float movementSpeedIncreasePerLevel = 0.5f;  // ������ �� �̵� �ӵ� ������
    public float defenseIncreasePerLevel = 1f;          // ������ �� ���� ������
    public float maxExperience = 100f;                   // ����� �ִ� ����ġ


    public EquipmentItem(string name, EquipmentEffect effect)
    {
        itemName = name;
        this.effect = effect;
    }

    public void LevelUp()
    {
        level++; // ����� ���� ����

        // ����� ȿ���� ������ �� ����
        effect.Attack += attackIncreasePerLevel;            // ���ݷ� ����
        effect.MovementSpeed += movementSpeedIncreasePerLevel;  // �̵��ӵ� ����
        effect.Defense += defenseIncreasePerLevel;           // ���� ����

        maxExperience *= 2.0f;
        experience = 0f;

        Debug.Log($"{itemName} ������! ���� ����: {level}, ���ݷ�: {effect.Attack}, �̵��ӵ�: {effect.MovementSpeed}, ����: {effect.Defense}");
    }



    public void AddExperience(float amount)
    {
        experience += amount;

        // ����ġ�� �ִ�ġ �̻����� �������� ��
        if (experience >= maxExperience)
        {
            experience = 0f;  // ����ġ �ʱ�ȭ
            LevelUp();  // ������
            ApplyLevelEffect();
        }
    }
    public void ApplyLevelEffect()
    {
        // �� �������� �����Ǵ� ȿ�� ���
        effect.Attack = level * attackIncreasePerLevel;
        effect.MovementSpeed = level * movementSpeedIncreasePerLevel;
        effect.Defense = level * defenseIncreasePerLevel;
    }
}


public class EquipmentInventory : MonoBehaviour
{
    [Header("�ʱ� ��� ���")]
    [SerializeField] public EquipmentItem[] availableEquipments = new EquipmentItem[4];  // 4���� ��� ���
    public EquipmentItem currentEquipment;  // ���� ������ ���
    [SerializeField] private GameObject[] rightHandWeapons;  // ������ �տ� ������ �����
    public int currentRightHandIndex = 0;  // ���� ������ �տ� ������ ������ �ε���

    private PlayerStat playerstat;
    private int currentIndex = 0;  // ���� ������ ����� �ε��� (���������� ��ü)

    private void Start()
    {
        playerstat = GetComponent<PlayerStat>();  // PlayerStat ������Ʈ ã��

        if (playerstat == null || playerstat.statData == null)
        {
            Debug.LogError("PlayerStat �Ǵ� StatData ������Ʈ�� ã�� �� �����ϴ�! ��� ȿ���� ������ �� �����ϴ�.");
            return;
        }
        SetupInitialEquipments();

        // �⺻ ��� ù ��° ���������� ����
        Equip(availableEquipments[currentIndex]);
    }

    public void SetupInitialEquipments()
    {
        // �� ����� �ʱ� ȿ���� ������ �� �����ϴ� ��ġ ����
        availableEquipments[0] = new EquipmentItem("SwordAndSheild", new EquipmentEffect { Attack = 10, MovementSpeed = 3, Defense = 5 })
        {
            attackIncreasePerLevel = 1f, // ���ݷ� ������
            movementSpeedIncreasePerLevel = 1f, // �̵��ӵ� ������
            defenseIncreasePerLevel = 1f // ���� ������
        };

        availableEquipments[1] = new EquipmentItem("SingleTwoHandeSword", new EquipmentEffect { Attack = 15, MovementSpeed = 1, Defense = 2 })
        {
            attackIncreasePerLevel = 1f, // ���ݷ� ������
            movementSpeedIncreasePerLevel = 1f, // �̵��ӵ� ������
            defenseIncreasePerLevel = 1f // ���� ������
        };

        availableEquipments[2] = new EquipmentItem("DoubleSwords", new EquipmentEffect { Attack = 20, MovementSpeed = 5, Defense = -5 })
        {
            attackIncreasePerLevel = 1f, // ���ݷ� ������
            movementSpeedIncreasePerLevel = 1f, // �̵��ӵ� ������
            defenseIncreasePerLevel = 1f // ���� ���ҷ�
        };

        availableEquipments[3] = new EquipmentItem("BowAndArrow", new EquipmentEffect { Attack = 15, MovementSpeed = 7, Defense = -5 })
        {
            attackIncreasePerLevel = 1f, // ���ݷ� ������
            movementSpeedIncreasePerLevel = 1f, // �̵��ӵ� ������
            defenseIncreasePerLevel = 1f // ���� ���ҷ�
        };

        Debug.Log("��� �ʱ�ȭ�Ǿ����ϴ�.");
    }


    // ��� ��ü �޼��� (����)
    public void Equip(EquipmentItem newEquipment)
    {
        if (currentEquipment != null)
        {
            // ���� ��� ȿ�� ����
            RemoveEquipmentEffect(currentEquipment.effect);
        }

        // ���ο� ��� ȿ�� �ݿ�
        currentEquipment = newEquipment;
        ApplyEquipmentEffect(currentEquipment.effect);
        Debug.Log($"{currentEquipment.itemName} ��� �����߽��ϴ�.");
    }

    public void EquipByIndex(int index)
    {
        if (index >= 0 && index < availableEquipments.Length)
        {
            Equip(availableEquipments[index]);
        }
    }

    // ��� ȿ���� StatData�� �ݿ��ϴ� �޼���
    private void ApplyEquipmentEffect(EquipmentEffect equipmentEffect)
    {
        // BaseEntity���� StatData�� ������ ��� ȿ�� �ݿ�
        playerstat.statData.baseAttack += equipmentEffect.Attack;
        playerstat.statData.baseMovementSpeed += equipmentEffect.MovementSpeed;
        playerstat.statData.baseDefense += equipmentEffect.Defense;

        // ��� ȿ�� �ݿ� �� ���� ����
        Debug.Log($"��� ȿ�� �ݿ�: ���ݷ�: {playerstat.statData.baseAttack}, �̵��ӵ�: {playerstat.statData.baseMovementSpeed}, ����: {playerstat.statData.baseDefense}");
    }

    // ��� ȿ���� �����ϴ� �޼ҵ�
    private void RemoveEquipmentEffect(EquipmentEffect equipmentEffect)
    {
        // ���� ��� ȿ���� ����
        playerstat.statData.baseAttack -= equipmentEffect.Attack;
        playerstat.statData.baseMovementSpeed -= equipmentEffect.MovementSpeed;
        playerstat.statData.baseDefense -= equipmentEffect.Defense;

        // ��� ȿ�� ���� �� ���� ����
        Debug.Log($"��� ȿ�� ����: ���ݷ�: {playerstat.statData.baseAttack}, �̵��ӵ�: {playerstat.statData.baseMovementSpeed}, ����: {playerstat.statData.baseDefense}");
    }


    // ���� ������ ��� Ȯ���ϴ� �޼���
    public void PrintCurrentEquipment()
    {
        if (currentEquipment != null)
        {
            Debug.Log($"���� ������ ���: {currentEquipment.itemName}, ���ݷ�: {currentEquipment.effect.Attack}, �̵��ӵ�: {currentEquipment.effect.MovementSpeed}, ����: {currentEquipment.effect.Defense}");
        }
        else
        {
            Debug.Log("���� ��� �����ϴ�.");
        }
    }
}


