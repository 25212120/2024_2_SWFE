using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using static BaseStructure;

public class Unit_SpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class ResourceRequirement
    {
        public MaterialManager.ResourceType resourceType;  // �ڿ� ���� (Money, Wood ��)
        public int amount;  // �䱸 �ڿ� ��
    }
    [Header("���� ���� ������ �ʿ��� �ڿ�")]
    [SerializeField] private List<ResourceRequirement> SWUnitSpawnRequirements = new List<ResourceRequirement>()
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Money, amount = 100 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Stone, amount = 30 }
        };
    [Header("���� ���� ������ �ʿ��� �ڿ�")]
    [SerializeField] private List<ResourceRequirement> MGUnitSpawnRequirements = new List<ResourceRequirement>()
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Money, amount = 100 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Stone, amount = 30 }
        };

    [Header("���� ���� ���� ����")]
    [SerializeField] private GameObject SWunitPrefab; // ��ȯ�� ������ ������
    [Header("���� ���� ���� ����")]
    [SerializeField] private GameObject MGunitPrefab; // ��ȯ�� ������ ������

    [Header("���� ���� ���� ����")]
    [SerializeField] private GameObject unitPrefab; // ��ȯ�� ������ ������

    [Header("T�� ����, F�� ����")]
    [SerializeField] private bool SpawnUnitSelect; // ��ȯ�� ������ ������

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
        PhotonNetwork.ConnectUsingSettings();

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
        if (SpawnUnitSelect)
        {
            // ������ �ʿ��� �ڿ����� ��� �Ҹ��� �� �ִ��� Ȯ��
            foreach (var requirement in SWUnitSpawnRequirements)
            {
                if (!MaterialManager.Instance.ConsumeResource(requirement.resourceType, requirement.amount))
                {
                    // �ڿ��� �����ϸ� ���� ����
                    Debug.LogWarning($"{requirement.resourceType} �ڿ��� �����մϴ�. ���� ���� ����.");
                    return false;
                }
            }
            unitPrefab = SWunitPrefab;
        }
        else
        {
            // ������ �ʿ��� �ڿ����� ��� �Ҹ��� �� �ִ��� Ȯ��
            foreach (var requirement in MGUnitSpawnRequirements)
            {
                if (!MaterialManager.Instance.ConsumeResource(requirement.resourceType, requirement.amount))
                {
                    // �ڿ��� �����ϸ� ���� ����
                    Debug.LogWarning($"{requirement.resourceType} �ڿ��� �����մϴ�. ���� ���� ����.");
                    return false;
                }
            }
            unitPrefab = MGunitPrefab;
        }

        // ��� �ڿ� �Һ� �����ϸ� ���� ��ȯ ����
        PerformSpawn();

        return true;
    }

    private void PerformSpawn()
    {
        if (GameSettings.IsMultiplayer == false)
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
        if (GameSettings.IsMultiplayer == true)
        {
            if (unitPrefab != null && spawnPoint != null)
            {
                // ���� ����Ʈ���� ������ ��ȯ
                PhotonNetwork.Instantiate(unitPrefab.name, spawnPoint.position, spawnPoint.rotation);
                Debug.Log("������ ��ȯ�Ǿ����ϴ�.");
            }
            else
            {
                Debug.LogWarning("���� ������ �Ǵ� ���� ����Ʈ�� �������� �ʾҽ��ϴ�.");
            }
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
