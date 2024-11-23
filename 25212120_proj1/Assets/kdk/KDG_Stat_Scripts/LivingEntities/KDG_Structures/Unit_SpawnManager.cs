using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_SpawnManager : BaseStructure
{
    [Header("���׷��̵忡 �ʿ��� �ڿ�")]
    [SerializeField] private List<ResourceRequirement> spawnRequirements = new List<ResourceRequirement>(); // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ

    [Header("���� ���� ����")]
    [SerializeField] private GameObject unitPrefab; // ��ȯ�� ������ ������
    private Transform spawnPoint; // ������ ��ȯ�� ��ġ 
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    protected override void Update()
    {
        // �ڿ� �Һ� �� ���� ��ȯ
        if (Input.GetKeyDown(KeyCode.X)) // X Ű�� ��ȯ
        {
            Spawn();
        }
    }

    public bool Spawn()
    {
        // ������ �ʿ��� �ڿ����� ��� �Ҹ��� �� �ִ��� Ȯ��
        foreach (var requirement in spawnRequirements)
        {
            if (!MaterialManager.Instance.ConsumeResource(requirement.resourceType, requirement.amount))
            {
                // �ڿ��� �����ϸ� ���� ����
                Debug.LogWarning($"{requirement.resourceType} �ڿ��� �����մϴ�. ���� ���� ����.");
                return false;
            }
        }

        // ��� �ڿ� �Һ� �����ϸ� ���� ��ȯ ����
        PerformSpawn();

        return true;
    }

    private void PerformSpawn()
    {
        if (unitPrefab != null && spawnPoint != null)
        {
            // ���� ����Ʈ���� ������ ��ȯ
            Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("������ ��ȯ�Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogWarning("���� ������ �Ǵ� ���� ����Ʈ�� �������� �ʾҽ��ϴ�.");
        }
    }

    // �� Ÿ���� ���� ����Ʈ�� �����ϴ� �޼���
    public void SetSpawnPoint(Vector3 newSpawnPosition)
    {
        spawnPoint.position = newSpawnPosition;
        Debug.Log($"���� ����Ʈ�� ({newSpawnPosition.x}, {newSpawnPosition.z})�� �����Ǿ����ϴ�.");
    }
}
