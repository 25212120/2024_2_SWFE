using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseStructure;

public class TowerSpawn_Manager : MonoBehaviour
{
    [System.Serializable]
    public class ResourceRequirement
    {
        public MaterialManager.ResourceType resourceType;  // 자원 종류 (Money, Wood 등)
        public int amount;  // 요구 자원 양
    }
    [Header("코어 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> CoreSpawnRequirements = new List<ResourceRequirement>() // 업그레이드에 필요한 자원 리스트
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Money, amount = 100 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Stone, amount = 30 }
        };

[Header("마법 타워 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> MagicTowerSpawnRequirements = new List<ResourceRequirement>() // 업그레이드에 필요한 자원 리스트
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Crystal, amount = 10 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.FireEssence, amount = 5 },


        };
    [Header("로켓 타워 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> RocketTowerSpawnRequirements = new List<ResourceRequirement>() // 업그레이드에 필요한 자원 리스트
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Stone, amount = 30 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Metal, amount = 10 },

        };
    [Header("번개 타워 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> LightTowerSpawnRequirements = new List<ResourceRequirement>() // 업그레이드에 필요한 자원 리스트
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Crystal, amount = 10 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.IceEssence, amount = 5 },

        };
    [Header("힐 타워 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> HealTowerSpawnRequirements = new List<ResourceRequirement>() // 업그레이드에 필요한 자원 리스트
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Crystal, amount = 10 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.WoodEssence, amount = 5},

        };
    [Header("화살 타워 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> ArrowTowerSpawnRequirements = new List<ResourceRequirement>() // 업그레이드에 필요한 자원 리스트
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Stone, amount = 30 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Metal, amount = 10 },

        };
    [Header("근접 유닛 소환 타워 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> SWSpawnTowerSpawnRequirements = new List<ResourceRequirement>() // 업그레이드에 필요한 자원 리스트
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Stone, amount = 30 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.SandEssence, amount = 5 },

        };
    [Header("원거리 유닛 소환 타워 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> MGSpawnTowerSpawnRequirements = new List<ResourceRequirement>() // 업그레이드에 필요한 자원 리스트
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Stone, amount = 30 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.FireEssence, amount = 5 },

        };
    [Header("벽 소환 타워 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> WallSpawnRequirements = new List<ResourceRequirement>() // 업그레이드에 필요한 자원 리스트
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Stone, amount = 30 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Metal, amount = 10 },

        };

    // 자원 확인 함수 (보유 자원량 >= 요구 자원량)
    public bool CheckIfResourcesAreSufficient(string currentPrefab)
    {
        if (string.IsNullOrEmpty(currentPrefab)) return false;

        List<ResourceRequirement> towerSpawnRequirements = GetSpawnRequirementsForPrefab(currentPrefab);

        if (towerSpawnRequirements == null || towerSpawnRequirements.Count == 0)
        {
            Debug.LogWarning("요구 자원 리스트가 비어있습니다.");
            return false;
        }

        // 각 자원 요구 사항 확인
        foreach (var requirement in towerSpawnRequirements)
        {
            if (MaterialManager.Instance.GetResource(requirement.resourceType) < requirement.amount)
            {
                // 자원이 부족하면 false 반환
                Debug.LogWarning($"{requirement.resourceType} 자원이 부족합니다.");
                return false;
            }
        }

        // 자원이 충분하면 true 반환
        return true;
    }

    // 자원 소모 함수 (소모할 자원이 충분하면 자원 소모 후 true 반환, 부족하면 false 반환)
    public bool SpawnAndConsumeMaterial(string currentPrefab)
    {
        if (string.IsNullOrEmpty(currentPrefab)) return false;

        List<ResourceRequirement> towerSpawnRequirements = GetSpawnRequirementsForPrefab(currentPrefab);

        if (towerSpawnRequirements == null || towerSpawnRequirements.Count == 0)
        {
            Debug.LogWarning("요구 자원 리스트가 비어있습니다.");
            return false;
        }

        // 자원 소모 처리
        foreach (var requirement in towerSpawnRequirements)
        {
            if (!MaterialManager.Instance.ConsumeResource(requirement.resourceType, requirement.amount))
            {
                // 자원이 부족하면 스폰 실패
                Debug.LogWarning($"{requirement.resourceType} 자원이 부족합니다. 유닛 스폰 실패.");
                return false;
            }
        }

        // 자원 소모 후 성공
        return true;
    }

    // 현재 프리팹에 해당하는 요구 자원 리스트 반환하는 함수
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
                Debug.LogWarning("알 수 없는 타워 프리팹: " + currentPrefab);
                return null;
        }

        // 디버그 로그로 요구 자원 리스트 출력
        if (requirements != null)
        {
            Debug.Log($"{currentPrefab}에 할당된 요구 자원 리스트:");

            foreach (var requirement in requirements)
            {
                Debug.Log($"{requirement.resourceType}: {requirement.amount}");
            }
        }
        return requirements;
    }
}