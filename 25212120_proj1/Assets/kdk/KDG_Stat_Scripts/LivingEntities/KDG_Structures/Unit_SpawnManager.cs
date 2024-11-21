using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_SpawnManager : BaseStructure
{
    [Header("업그레이드에 필요한 자원")]
    [SerializeField] private List<ResourceRequirement> spawnRequirements = new List<ResourceRequirement>(); // 업그레이드에 필요한 자원 리스트

    [Header("유닛 스폰 설정")]
    [SerializeField] private GameObject unitPrefab; // 소환할 유닛의 프리팹
    private Transform spawnPoint; // 유닛이 소환될 위치 (사용자가 클릭으로 설정)

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // 메인 카메라 참조
    }

    protected override void Update()
    {
        // 스폰 포인트 설정
        if (spawnPoint == null)
        {
            HandleSpawnPointSelection();
        }

        // 자원 소비 후 유닛 소환
        if (Input.GetKeyDown(KeyCode.S)) // 예: S 키로 소환
        {
            Spawn();
        }
    }

    private void HandleSpawnPointSelection()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭 시
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 스폰 포인트로 설정
                spawnPoint = new GameObject("SpawnPoint").transform; // 새로운 게임 오브젝트로 설정
                spawnPoint.position = hit.point; // 클릭한 지점에 스폰 포인트 설정
                Debug.Log("스폰 포인트가 설정되었습니다.");
            }
        }
    }

    public bool Spawn()
    {
        // 업그레이드에 필요한 자원들을 모두 소모할 수 있는지 확인
        foreach (var requirement in spawnRequirements)
        {
            if (!MaterialManager.Instance.ConsumeResource(requirement.resourceType, requirement.amount))
            {
                // 자원이 부족하면 업그레이드 실패
                Debug.LogWarning($"{requirement.resourceType} 자원이 부족합니다. 유닛 스폰 실패.");
                return false;
            }
        }

        // 모든 자원 소비가 성공하면 유닛 소환 진행
        PerformSpawn();

        return true;
    }

    private void PerformSpawn()
    {
        if (unitPrefab != null && spawnPoint != null)
        {
            // 스폰 포인트에서 유닛을 소환
            Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("유닛이 소환되었습니다.");
        }
        else
        {
            Debug.LogWarning("유닛 프리팹 또는 스폰 포인트가 설정되지 않았습니다.");
        }
    }
}
