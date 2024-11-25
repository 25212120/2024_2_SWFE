using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseStructure;

public class Unit_SpawnManager : MonoBehaviour
{
    [Header("���� ������ �ʿ��� �ڿ�")]
    [SerializeField] private List<ResourceRequirement> UnitSpawnRequirements = new List<ResourceRequirement>();

    [Header("���� ���� ����")]
    [SerializeField] private GameObject unitPrefab; // ��ȯ�� ������ ������
    private Transform spawnPoint; // ������ ��ȯ�� ��ġ 
    private Camera mainCamera;

    [Header("��ȯ ���� ����")]
    [SerializeField] private bool canSpawn = true;
    private float cellSize = 1;
    void Start()
    {
        mainCamera = Camera.main;

        // spawnPoint �ʱ�ȭ (����: ���� ������ ���� �� ���� ������Ʈ�� ã�� �Ҵ�)
        if (spawnPoint == null)
        {
            GameObject spawnPointObject = new GameObject("SpawnPoint");
            spawnPoint = spawnPointObject.transform;
            spawnPoint.position = transform.position; // Ÿ���� �⺻ ��ġ�� ���� ��ġ�� ����
        }
    }

    public bool CanSpawn()
    {
        return canSpawn;
    }
    public void SetCanSpawn(bool canSpawn)
    {
        this.canSpawn = canSpawn;
    }
    void Update()
    {
        // �ڿ� �Һ� �� ���� ��ȯ
        if (Input.GetKeyDown(KeyCode.X)) // X Ű�� ��ȯ
        {
            Spawn();
        }
    }

    public bool Spawn()
    {
        if (!canSpawn)
        {
            Debug.Log("���� �Ұ�");
            return false;
        }
        // ������ �ʿ��� �ڿ����� ��� �Ҹ��� �� �ִ��� Ȯ��
        foreach (var requirement in UnitSpawnRequirements)
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

    public void SetSpawnPoint(Vector2Int cellPosition)
    {
        // �� ������ ��ġ�� ���� ��ǥ�� ��ȯ
        Vector3 newSpawnPosition = new Vector3(cellPosition.x * cellSize, 0, cellPosition.y * cellSize);

        // ������ ������ �� �ִ� ����Ʈ�� ����
        spawnPoint.position = newSpawnPosition;

        Debug.Log($"���� ����Ʈ�� ��({cellPosition.x}, {cellPosition.y})���� �����Ǿ����ϴ�.");
    }

}
