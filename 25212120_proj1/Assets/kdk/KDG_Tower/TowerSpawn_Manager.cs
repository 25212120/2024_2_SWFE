using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseStructure;

public class TowerSpawn_Manager : MonoBehaviour
{
    [Header("�ھ� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> CoreSpawnRequirements = new List<ResourceRequirement>(); // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
    [Header("���� Ÿ�� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> MagicTowerSpawnRequirements = new List<ResourceRequirement>(); // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
    [Header("���� Ÿ�� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> RocketTowerSpawnRequirements = new List<ResourceRequirement>(); // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
    [Header("���� Ÿ�� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> LightTowerSpawnRequirements = new List<ResourceRequirement>(); // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
    [Header("�� Ÿ�� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> HealTowerSpawnRequirements = new List<ResourceRequirement>(); // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
    [Header("ȭ�� Ÿ�� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> ArrowTowerSpawnRequirements = new List<ResourceRequirement>(); // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
    [Header("���� ���� ��ȯ Ÿ�� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> SWSpawnTowerSpawnRequirements = new List<ResourceRequirement>(); // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
    [Header("���Ÿ� ���� ��ȯ Ÿ�� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> MGSpawnTowerSpawnRequirements = new List<ResourceRequirement>(); // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ

    // �ڿ� Ȯ�� �Լ� (���� �ڿ��� >= �䱸 �ڿ���)
    public bool CheckIfResourcesAreSufficient(string currentPrefab)
    {
        if (string.IsNullOrEmpty(currentPrefab)) return false;

        List<ResourceRequirement> towerSpawnRequirements = GetSpawnRequirementsForPrefab(currentPrefab);

        if (towerSpawnRequirements == null || towerSpawnRequirements.Count == 0)
        {
            Debug.LogWarning("�䱸 �ڿ� ����Ʈ�� ����ֽ��ϴ�.");
            return false;
        }

        // �� �ڿ� �䱸 ���� Ȯ��
        foreach (var requirement in towerSpawnRequirements)
        {
            if (MaterialManager.Instance.GetResource(requirement.resourceType) < requirement.amount)
            {
                // �ڿ��� �����ϸ� false ��ȯ
                Debug.LogWarning($"{requirement.resourceType} �ڿ��� �����մϴ�.");
                return false;
            }
        }

        // �ڿ��� ����ϸ� true ��ȯ
        return true;
    }

    // �ڿ� �Ҹ� �Լ� (�Ҹ��� �ڿ��� ����ϸ� �ڿ� �Ҹ� �� true ��ȯ, �����ϸ� false ��ȯ)
    public bool SpawnAndConsumeMaterial(string currentPrefab)
    {
        if (string.IsNullOrEmpty(currentPrefab)) return false;

        List<ResourceRequirement> towerSpawnRequirements = GetSpawnRequirementsForPrefab(currentPrefab);

        if (towerSpawnRequirements == null || towerSpawnRequirements.Count == 0)
        {
            Debug.LogWarning("�䱸 �ڿ� ����Ʈ�� ����ֽ��ϴ�.");
            return false;
        }

        // �ڿ� �Ҹ� ó��
        foreach (var requirement in towerSpawnRequirements)
        {
            if (!MaterialManager.Instance.ConsumeResource(requirement.resourceType, requirement.amount))
            {
                // �ڿ��� �����ϸ� ���� ����
                Debug.LogWarning($"{requirement.resourceType} �ڿ��� �����մϴ�. ���� ���� ����.");
                return false;
            }
        }

        // �ڿ� �Ҹ� �� ����
        return true;
    }

    // ���� �����տ� �ش��ϴ� �䱸 �ڿ� ����Ʈ ��ȯ�ϴ� �Լ�
    private List<ResourceRequirement> GetSpawnRequirementsForPrefab(string currentPrefab)
    {
        switch (currentPrefab)
        {
            case "Core":
                return CoreSpawnRequirements;

            case "MagicTower_1":
                return MagicTowerSpawnRequirements;

            case "RocketTower_1":
                return RocketTowerSpawnRequirements;

            case "LightTower_1":
                return LightTowerSpawnRequirements;

            case "HealTower_1":
                return HealTowerSpawnRequirements;

            case "ArrowTower_1":
                return ArrowTowerSpawnRequirements;

            case "SpawnTower_1":
                return SWSpawnTowerSpawnRequirements;

            case "SpawnTower_2":
                return MGSpawnTowerSpawnRequirements;

            default:
                Debug.LogWarning("�� �� ���� Ÿ�� ������: " + currentPrefab);
                return null;
        }
    }
}
