using System.Collections.Generic;
using UnityEngine;

public class SpawnStructure : BaseStructure
{
    public Unit_SpawnManager unitSpawnManager;

    void Start()
    {
        // Unit_SpawnManager가 제대로 할당되었는지 확인
        unitSpawnManager = GetComponent<Unit_SpawnManager>();

        if (unitSpawnManager == null)
        {
            Debug.LogError("Unit_SpawnManager가 이 타워에 할당되지 않았습니다.");
        }
    }

    public void SetSpawnPointForThisStructure(Vector2Int cellPosition)
    {
        // 각 SpawnStructure에서 자신만의 유닛 스폰 포인트를 설정
        if (unitSpawnManager != null)
        {
            unitSpawnManager.SetSpawnPoint(cellPosition);
            Debug.Log($"{name}에서 스폰 포인트가 {cellPosition}로 설정되었습니다.");
        }
    }

    protected override void Awake()
    {
        base.Awake();

        List<ResourceRequirement> requirements = new List<ResourceRequirement>
        {
            new ResourceRequirement(MaterialManager.ResourceType.Money, 100),  // 생성자에 맞게 수정
            new ResourceRequirement(MaterialManager.ResourceType.Wood, 50),
            new ResourceRequirement(MaterialManager.ResourceType.Stone, 30)
        };

        // 유닛의 업그레이드 자원 설정
        upgradeRequirements = requirements;
    }
}
