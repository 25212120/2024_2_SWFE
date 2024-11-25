using System.Collections.Generic;
using UnityEngine;

public class SpawnStructure : BaseStructure
{
    public Unit_SpawnManager unitSpawnManager;

    void Start()
    {
        // Unit_SpawnManager�� ����� �Ҵ�Ǿ����� Ȯ��
        unitSpawnManager = GetComponent<Unit_SpawnManager>();

        if (unitSpawnManager == null)
        {
            Debug.LogError("Unit_SpawnManager�� �� Ÿ���� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    public void SetSpawnPointForThisStructure(Vector2Int cellPosition)
    {
        // �� SpawnStructure���� �ڽŸ��� ���� ���� ����Ʈ�� ����
        if (unitSpawnManager != null)
        {
            unitSpawnManager.SetSpawnPoint(cellPosition);
            Debug.Log($"{name}���� ���� ����Ʈ�� {cellPosition}�� �����Ǿ����ϴ�.");
        }
    }

    protected override void Awake()
    {
        base.Awake();

        List<ResourceRequirement> requirements = new List<ResourceRequirement>
        {
            new ResourceRequirement(MaterialManager.ResourceType.Money, 100),  // �����ڿ� �°� ����
            new ResourceRequirement(MaterialManager.ResourceType.Wood, 50),
            new ResourceRequirement(MaterialManager.ResourceType.Stone, 30)
        };

        // ������ ���׷��̵� �ڿ� ����
        upgradeRequirements = requirements;
    }
}
