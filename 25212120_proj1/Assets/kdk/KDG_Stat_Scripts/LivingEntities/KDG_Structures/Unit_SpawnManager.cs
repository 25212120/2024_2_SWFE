using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_SpawnManager : BaseStructure
{
    [Header("���׷��̵忡 �ʿ��� �ڿ�")]
    [SerializeField] private List<ResourceRequirement> spawnRequirements = new List<ResourceRequirement>(); // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ

    [Header("���� ���� ����")]
    [SerializeField] private GameObject unitPrefab; // ��ȯ�� ������ ������
    private Transform spawnPoint; // ������ ��ȯ�� ��ġ (����ڰ� Ŭ������ ����)

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // ���� ī�޶� ����
    }

    protected override void Update()
    {
        // ���� ����Ʈ ����
        if (spawnPoint == null)
        {
            HandleSpawnPointSelection();
        }

        // �ڿ� �Һ� �� ���� ��ȯ
        if (Input.GetKeyDown(KeyCode.S)) // ��: S Ű�� ��ȯ
        {
            Spawn();
        }
    }

    private void HandleSpawnPointSelection()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Ŭ�� ��
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // ���� ����Ʈ�� ����
                spawnPoint = new GameObject("SpawnPoint").transform; // ���ο� ���� ������Ʈ�� ����
                spawnPoint.position = hit.point; // Ŭ���� ������ ���� ����Ʈ ����
                Debug.Log("���� ����Ʈ�� �����Ǿ����ϴ�.");
            }
        }
    }

    public bool Spawn()
    {
        // ���׷��̵忡 �ʿ��� �ڿ����� ��� �Ҹ��� �� �ִ��� Ȯ��
        foreach (var requirement in spawnRequirements)
        {
            if (!MaterialManager.Instance.ConsumeResource(requirement.resourceType, requirement.amount))
            {
                // �ڿ��� �����ϸ� ���׷��̵� ����
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
}
