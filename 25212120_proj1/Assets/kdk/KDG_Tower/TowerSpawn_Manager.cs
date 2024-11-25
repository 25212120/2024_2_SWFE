using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseStructure;

public class TowerSpawn_Manager : MonoBehaviour
{
    [Header("코어 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> CoreSpawnRequirements = new List<ResourceRequirement>(); // 업그레이드에 필요한 자원 리스트
    [Header("마법 타워 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> MagicTowerSpawnRequirements = new List<ResourceRequirement>(); // 업그레이드에 필요한 자원 리스트
    [Header("로켓 타워 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> RocketTowerSpawnRequirements = new List<ResourceRequirement>(); // 업그레이드에 필요한 자원 리스트
    [Header("번개 타워 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> LightTowerSpawnRequirements = new List<ResourceRequirement>(); // 업그레이드에 필요한 자원 리스트
    [Header("힐 타워 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> HealTowerSpawnRequirements = new List<ResourceRequirement>(); // 업그레이드에 필요한 자원 리스트
    [Header("화살 타워 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> ArrowTowerSpawnRequirements = new List<ResourceRequirement>(); // 업그레이드에 필요한 자원 리스트
    [Header("근접 유닛 소환 타워 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> SWSpawnTowerSpawnRequirements = new List<ResourceRequirement>(); // 업그레이드에 필요한 자원 리스트
    [Header("원거리 유닛 소환 타워 소환에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> MGSpawnTowerSpawnRequirements = new List<ResourceRequirement>(); // 업그레이드에 필요한 자원 리스트

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
                Debug.LogWarning("알 수 없는 타워 프리팹: " + currentPrefab);
                return null;
        }
    }
}
