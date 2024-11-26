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
        public MaterialManager.ResourceType resourceType;  // 자원 종류 (Money, Wood 등)
        public int amount;  // 요구 자원 양
    }
    [Header("근접 유닛 스폰에 필요한 자원")]
    [SerializeField] private List<ResourceRequirement> SWUnitSpawnRequirements = new List<ResourceRequirement>()
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Money, amount = 100 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Stone, amount = 30 }
        };
    [Header("마법 유닛 스폰에 필요한 자원")]
    [SerializeField] private List<ResourceRequirement> MGUnitSpawnRequirements = new List<ResourceRequirement>()
        {
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Money, amount = 100 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Wood, amount = 50 },
        new ResourceRequirement { resourceType = MaterialManager.ResourceType.Stone, amount = 30 }
        };

    [Header("근접 유닛 스폰 설정")]
    [SerializeField] private GameObject SWunitPrefab; // 소환할 유닛의 프리팹
    [Header("마법 유닛 스폰 설정")]
    [SerializeField] private GameObject MGunitPrefab; // 소환할 유닛의 프리팹

    [Header("현재 유닛 스폰 설정")]
    [SerializeField] private GameObject unitPrefab; // 소환할 유닛의 프리팹

    [Header("T면 근접, F면 마법")]
    [SerializeField] private bool SpawnUnitSelect; // 소환할 유닛의 프리팹

    private Transform spawnPoint; // 유닛이 소환될 위치 
    private Camera mainCamera;

    [Header("소환 가능 여부")]
    [SerializeField] private bool canSpawn = true;
    private float cellSize = 1;
    void Start()
    {
        mainCamera = Camera.main;

        // spawnPoint 초기화 (예시: 유닛 스폰을 위한 빈 게임 오브젝트를 찾아 할당)
        if (spawnPoint == null)
        {
            GameObject spawnPointObject = new GameObject("SpawnPoint");
            spawnPoint = spawnPointObject.transform;
            spawnPoint.position = transform.position; // 타워의 기본 위치를 스폰 위치로 설정
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
        // 자원 소비 후 유닛 소환
        if (Input.GetKeyDown(KeyCode.X)) // X 키로 소환
        {
            Spawn();
        }
    }

    public bool Spawn()
    {
        if (!canSpawn)
        {
            Debug.Log("스폰 불가");
            return false;
        }
        if (SpawnUnitSelect)
        {
            // 스폰에 필요한 자원들을 모두 소모할 수 있는지 확인
            foreach (var requirement in SWUnitSpawnRequirements)
            {
                if (!MaterialManager.Instance.ConsumeResource(requirement.resourceType, requirement.amount))
                {
                    // 자원이 부족하면 스폰 실패
                    Debug.LogWarning($"{requirement.resourceType} 자원이 부족합니다. 유닛 스폰 실패.");
                    return false;
                }
            }
            unitPrefab = SWunitPrefab;
        }
        else
        {
            // 스폰에 필요한 자원들을 모두 소모할 수 있는지 확인
            foreach (var requirement in MGUnitSpawnRequirements)
            {
                if (!MaterialManager.Instance.ConsumeResource(requirement.resourceType, requirement.amount))
                {
                    // 자원이 부족하면 스폰 실패
                    Debug.LogWarning($"{requirement.resourceType} 자원이 부족합니다. 유닛 스폰 실패.");
                    return false;
                }
            }
            unitPrefab = MGunitPrefab;
        }

        // 모든 자원 소비가 성공하면 유닛 소환 진행
        PerformSpawn();

        return true;
    }

    private void PerformSpawn()
    {
        if (GameSettings.IsMultiplayer == false)
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
        if (GameSettings.IsMultiplayer == true)
        {
            if (unitPrefab != null && spawnPoint != null)
            {
                // 스폰 포인트에서 유닛을 소환
                PhotonNetwork.Instantiate(unitPrefab.name, spawnPoint.position, spawnPoint.rotation);
                Debug.Log("유닛이 소환되었습니다.");
            }
            else
            {
                Debug.LogWarning("유닛 프리팹 또는 스폰 포인트가 설정되지 않았습니다.");
            }
        }
    }

    public void SetSpawnPoint(Vector2Int cellPosition)
    {
        // 셀 단위의 위치를 월드 좌표로 변환
        Vector3 newSpawnPosition = new Vector3(cellPosition.x * cellSize, 0, cellPosition.y * cellSize);

        // 실제로 스폰할 수 있는 포인트로 설정
        spawnPoint.position = newSpawnPosition;

        Debug.Log($"스폰 포인트가 셀({cellPosition.x}, {cellPosition.y})에서 설정되었습니다.");
    }

}
