using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseStructure;

public class TowerSpawn_Manager : MonoBehaviour
{
    [System.Serializable]
    public class ResourceRequirement
    {
        public MaterialManager.ResourceType resourceType;  // �ڿ� ���� (Money, Wood ��)
        public int amount;  // �䱸 �ڿ� ��
    }
    [Header("�ھ� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> CoreSpawnRequirements = new List<ResourceRequirement>() // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Money, amount = 100 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Stone, amount = 30 }
        };

[Header("���� Ÿ�� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> MagicTowerSpawnRequirements = new List<ResourceRequirement>() // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Crystal, amount = 10 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.FireEssence, amount = 5 },


        };
    [Header("���� Ÿ�� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> RocketTowerSpawnRequirements = new List<ResourceRequirement>() // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Stone, amount = 30 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Metal, amount = 10 },

        };
    [Header("���� Ÿ�� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> LightTowerSpawnRequirements = new List<ResourceRequirement>() // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Crystal, amount = 10 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.IceEssence, amount = 5 },

        };
    [Header("�� Ÿ�� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> HealTowerSpawnRequirements = new List<ResourceRequirement>() // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Crystal, amount = 10 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.WoodEssence, amount = 5},

        };
    [Header("ȭ�� Ÿ�� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> ArrowTowerSpawnRequirements = new List<ResourceRequirement>() // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Stone, amount = 30 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Metal, amount = 10 },

        };
    [Header("���� ���� ��ȯ Ÿ�� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> SWSpawnTowerSpawnRequirements = new List<ResourceRequirement>() // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Stone, amount = 30 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.SandEssence, amount = 5 },

        };
    [Header("���Ÿ� ���� ��ȯ Ÿ�� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> MGSpawnTowerSpawnRequirements = new List<ResourceRequirement>() // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Stone, amount = 30 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.FireEssence, amount = 5 },

        };
    [Header("�� ��ȯ Ÿ�� ��ȯ�� �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> WallSpawnRequirements = new List<ResourceRequirement>() // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Stone, amount = 30 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Metal, amount = 10 },

        };

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
        List<ResourceRequirement> requirements = null;

        switch (currentPrefab)
        {
            case "Core":
                requirements = CoreSpawnRequirements;
                break;

            case "MagicTower_1":
                requirements = MagicTowerSpawnRequirements;
                break;

            case "RocketTower_1":
                requirements = RocketTowerSpawnRequirements;
                break;

            case "LightTower_1":
                requirements = LightTowerSpawnRequirements;
                break;

            case "HealTower_1":
                requirements = HealTowerSpawnRequirements;
                break;

            case "ArrowTower_1":
                requirements = ArrowTowerSpawnRequirements;
                break;

            case "SpawnTower_1":
                requirements = SWSpawnTowerSpawnRequirements;
                break;

            case "SpawnTower_2":
                requirements = MGSpawnTowerSpawnRequirements;
                break;

            case "Wall_1":
                requirements = WallSpawnRequirements;
                break;

            default:
                Debug.LogWarning("�� �� ���� Ÿ�� ������: " + currentPrefab);
                return null;
        }

        // ����� �α׷� �䱸 �ڿ� ����Ʈ ���
        if (requirements != null)
        {
            Debug.Log($"{currentPrefab}�� �Ҵ�� �䱸 �ڿ� ����Ʈ:");

            foreach (var requirement in requirements)
            {
                Debug.Log($"{requirement.resourceType}: {requirement.amount}");
            }
        }
        return requirements;
    }
}