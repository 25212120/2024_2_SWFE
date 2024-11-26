using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.UIElements;
using System.Net.NetworkInformation;

public class HighlightArea : MonoBehaviour
{
    public float cellSize_Virtical = 1f; // 그리드 세로 크기
    public float cellSize_Horizontal = 1f; // 그리드 가로 크기
    public int highlightSize = 3; // 강조 영역의 크기 (nxn)
    public int detailGridResolution = 3; // 강조 영역 내의 세부 그리드 분할 수
    public Material highlightMaterial; // 강조 영역에 적용할 머티리얼
    public Material ImpossibleMaterial; // 설치 불가 영역에 적용할 머티리얼
    public GameObject turretPrefab; // 포탑 프리팹

    private MeshFilter highlightMeshFilter;
    private MeshRenderer highlightMeshRenderer;

    private Vector3 hitPoint;
    private int cellX;
    private int cellZ;
    private bool isValidHit = false; // 레이캐스트 성공 여부
    private bool isValidPlacement = false; // 배치 가능 여부
    public OccupiedCell_Manager occupiedCell_Manager;
    public TowerSpawn_Manager towerSpawn_Manager;

    private float currentRotation = 0f; // 현재 타워의 회전 각도
    private GameObject previewTurret; // 미리 보기용 포탑 인스턴스

    public Vector3 gridStartPosition = new Vector3(0, 0, 0); // 그리드의 시작 위치 (고정)

    public List<GameObject> coreObjects; // 여러 개의 코어 오브젝트
    public float corePlacementRange = 20f; // Core 오브젝트 근처에서 설치할 수 있는 범위

    public Vector3 PT_V = new Vector3(0, 0, 0);
    public Quaternion PT_R = Quaternion.Euler(0, 0, 0);

    public bool isActive = true;

    void Start()
    {
        CreateHighlightObject();
        if (occupiedCell_Manager == null)
        {
            occupiedCell_Manager = FindObjectOfType<OccupiedCell_Manager>();  // 자동으로 찾기
        }
        if (towerSpawn_Manager == null)
        {
            towerSpawn_Manager = FindAnyObjectByType<TowerSpawn_Manager>();
        }
        coreObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Core"));

        PhotonNetwork.ConnectUsingSettings();

    }

    void Update()
    {
        if(!isActive)
        {
            DeactivateHighlightArea();
            return;
        }

        // Q 키를 눌러 타워 회전
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetRotatestate();
            RotateTurret();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetTurretPrefab("Wall_1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetTurretPrefab("MagicTower_1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetTurretPrefab("ArrowTower_1");
        }
        // 마우스 위치 업데이트
        UpdateMousePosition();

        // 강조 영역 업데이트
        UpdateHighlightArea();

        // 마우스 왼쪽 버튼 클릭 시 포탑 배치
        if (Input.GetMouseButtonDown(0))
        {
            PlaceTurret();
        }

    }

    void CreateHighlightObject()
    {
        GameObject highlightGO = new GameObject("HighlightArea");
        highlightMeshFilter = highlightGO.AddComponent<MeshFilter>();
        highlightMeshRenderer = highlightGO.AddComponent<MeshRenderer>();
        highlightMeshRenderer.material = new Material(highlightMaterial); // 머티리얼 인스턴스 생성

        highlightGO.transform.parent = transform;
    }

    void UpdateMousePosition()
    {
        // Camera.main이 null인지 확인
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera가 존재하지 않거나 'MainCamera' 태그가 설정되지 않았습니다.");
            isValidHit = false;
            return;
        }

        // 마우스 위치로부터 레이캐스트 생성
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // 마우스가 그리드의 시작 위치에서 얼마나 떨어져 있는지 계산
            Vector3 offset = hit.point - gridStartPosition; // 시작 위치 기준으로 마우스 오프셋 계산

            // 그리드 셀 좌표 계산
            cellX = Mathf.FloorToInt(offset.x / cellSize_Horizontal);
            cellZ = Mathf.FloorToInt(offset.z / cellSize_Virtical);

            hitPoint = hit.point;
            isValidHit = true;
        }
        else
        {
            // 레이캐스트가 실패한 경우
            Debug.Log("레이캐스트문제");
            isValidHit = false;
        }
    }

    void UpdateHighlightArea()
    {
        if (isValidHit)
        {
            // 강조 영역의 범위 계산
            int halfSize = highlightSize / 2;

            float startX = (cellX - halfSize) * cellSize_Horizontal;
            float startZ = (cellZ - halfSize) * cellSize_Virtical;
            float endX = (cellX + halfSize + 1) * cellSize_Horizontal;
            float endZ = (cellZ + halfSize + 1) * cellSize_Virtical;

            // 강조 영역 메쉬 생성
            Mesh mesh = BuildHighlightMesh(startX, startZ, endX, endZ);

            // 강조 영역의 메쉬 업데이트
            if (highlightMeshFilter != null)
            {
                highlightMeshFilter.mesh = mesh;
            }

            // 배치 가능 여부 확인
            isValidPlacement = CheckPlacementValidity();

            // 강조 영역의 색상 업데이트
            if (highlightMeshRenderer != null)
            {
                if (isValidPlacement && towerSpawn_Manager.CheckIfResourcesAreSufficient(CurrentPrefab))
                {
                    // 배치 가능: 녹색
                    highlightMeshRenderer.material = highlightMaterial;
                }
                else
                {
                    // 배치 불가: 빨간색
                    highlightMeshRenderer.material = ImpossibleMaterial;
                }
            }
            UpdatePreviewTurret();

        }
        else
        {
            // 마우스가 바닥에 닿지 않을 경우 강조 영역 비우기
            if (highlightMeshFilter != null)
            {
                highlightMeshFilter.mesh = null;
            }

            // 미리보기 포탑 비활성화
            if (previewTurret != null)
            {
                previewTurret.SetActive(false);
            }
        }
    }

    Mesh BuildHighlightMesh(float startX, float startZ, float endX, float endZ)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[6];

        // 버텍스 생성
        vertices[0] = new Vector3(startX, 0.02f, startZ);
        vertices[1] = new Vector3(startX, 0.02f, endZ);
        vertices[2] = new Vector3(endX, 0.02f, startZ);
        vertices[3] = new Vector3(endX, 0.02f, endZ);

        // 인덱스 설정
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 2;
        triangles[4] = 1;
        triangles[5] = 3;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    bool CheckPlacementValidity()
    {
        if (coreObjects.Count > 0)
        {
            bool isNearCore = false;
            foreach (GameObject core in coreObjects)
            {
                if (Vector3.Distance(hitPoint, core.transform.position) <= corePlacementRange)
                {
                    isNearCore = true;
                    break;
                }
            }

            if (!isNearCore)
            {
                Debug.Log("코어근처아님");
                return false; // 코어 근처가 아니면 배치 불가
            }
        }
        int halfSize = highlightSize / 2;

        for (int x = cellX - halfSize; x <= cellX + halfSize; x++)
        {
            for (int z = cellZ - halfSize; z <= cellZ + halfSize; z++)
            {
                Vector2Int cellPos = new Vector2Int(x, z);

                // 셀 점유 상태 확인
                if (occupiedCell_Manager.occupiedCells.Contains(cellPos))
                {
                    return false; // 이미 점유된 셀이 있음
                }

                // 셀 중심 좌표 계산
                Vector3 cellCenter = new Vector3(x * cellSize_Horizontal + cellSize_Horizontal / 2, hitPoint.y + 0.5f, z * cellSize_Virtical + cellSize_Virtical / 2);
                Vector3 halfExtents = new Vector3(cellSize_Horizontal / 2, 0.5f, cellSize_Virtical / 2);

                // 박스 콜라이더로 검사
                Collider[] colliders = Physics.OverlapBox(cellCenter, halfExtents, Quaternion.identity);

                foreach (Collider collider in colliders)
                {
                    // Ground 태그가 아닌 경우 배치 불가
                    if (collider.tag != "Ground")
                    {
                        return false;
                    }
                }
            }
        }

        return true; // 모든 지점이 배치 가능함
    }

    void UpdatePreviewTurret()
    {
        if (previewTurret == null)
        {
            if (GameSettings.IsMultiplayer == false)
            {            // 미리 보기용 포탑을 생성
                previewTurret = Instantiate(turretPrefab);
                previewTurret.SetActive(false); // 초기에는 비활성화
            }
            if (GameSettings.IsMultiplayer == true)
            {
                PhotonNetwork.Instantiate(turretPrefab.name, PT_V, PT_R);
                previewTurret.SetActive(false); // 초기에는 비활성화

            }
        }

        // 미리 보기용 포탑을 활성화하고 위치 및 회전 적용
        if (isValidHit && previewTurret != null)
        {
            previewTurret.SetActive(true);

            // 포탑의 위치는 하이라이트 영역의 중앙으로 설정
            float posX = (cellX + 0.5f) * cellSize_Horizontal;
            float posZ = (cellZ + 0.5f) * cellSize_Virtical;
            float posY = hitPoint.y; // 지형의 높이를 사용
            Vector3 position = new Vector3(posX, posY, posZ);

            // 회전 값 적용
            Quaternion rotation = Quaternion.Euler(0f, currentRotation, 0f);
            previewTurret.transform.SetPositionAndRotation(position, rotation);

            // 자식 오브젝트의 콜라이더를 비활성화
            Collider[] turretColliders = previewTurret.GetComponentsInChildren<Collider>();
            foreach (Collider col in turretColliders)
            {
                col.enabled = false; // 모든 자식 오브젝트의 콜라이더 비활성화
            }
        }
        else
        {
            // 마우스가 바닥에 닿지 않을 경우 미리보기 포탑 비활성화
            if (previewTurret != null)
            {
                previewTurret.SetActive(false);
            }
        }
    }
    void PlaceTurret()
    {
        if (!isValidHit || !isValidPlacement)
        {
            // 배치 불가
            return;
        }

        // 강조 영역의 범위 계산
        int halfSize = highlightSize / 2;

        List<Vector2Int> cellsToOccupy = new List<Vector2Int>();

        for (int x = cellX - halfSize; x <= cellX + halfSize; x++)
        {
            for (int z = cellZ - halfSize; z <= cellZ + halfSize; z++)
            {
                Vector2Int cellPos = new Vector2Int(x, z);
                cellsToOccupy.Add(cellPos);
            }
        }

        // 포탑을 배치할 위치 계산 (강조 영역의 중앙)
        float posX = (cellX + 0.5f) * cellSize_Horizontal;
        float posZ = (cellZ + 0.5f) * cellSize_Virtical;
        float posY = hitPoint.y; // 지형의 높이를 사용
        Vector3 position = new Vector3(posX, posY, posZ);

        if (turretPrefab != null)
        {
            Quaternion rotation = Quaternion.Euler(0f, currentRotation, 0f);
            if (GameSettings.IsMultiplayer == false)
            {
                if (towerSpawn_Manager.SpawnAndConsumeMaterial(CurrentPrefab))
                {
                    GameObject newTurret = Instantiate(turretPrefab, position, rotation);
                    // 타워 배치 후 콜라이더를 활성화
                    Collider turretCollider = newTurret.GetComponent<Collider>();
                    if (turretCollider != null)
                    {
                        turretCollider.enabled = true; // 배치 후 콜라이더 활성화
                    }

                    // 배치된 타워의 셀 점유 상태 업데이트
                    foreach (Vector2Int cellPos in cellsToOccupy)
                    {
                        occupiedCell_Manager.occupiedCells.Add(cellPos);
                    }
                }
                else
                {
                    Debug.Log("자원부족으로 설치 불가");
                }
            }
            if (GameSettings.IsMultiplayer == true)
            {
                if (towerSpawn_Manager.SpawnAndConsumeMaterial(CurrentPrefab))
                {
                    GameObject newTurret = PhotonNetwork.Instantiate(turretPrefab.name, position, rotation);
                    // 타워 배치 후 콜라이더를 활성화
                    Collider turretCollider = newTurret.GetComponent<Collider>();
                    if (turretCollider != null)
                    {
                        turretCollider.enabled = true; // 배치 후 콜라이더 활성화
                    }

                    // 배치된 타워의 셀 점유 상태 업데이트
                    foreach (Vector2Int cellPos in cellsToOccupy)
                    {
                        occupiedCell_Manager.occupiedCells.Add(cellPos);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("포탑 프리팹이 할당되지 않았습니다.");
        }
    }

    private float temp;
    private bool isRotate = false;
    private void SetRotatestate()
    {
        if (isRotate)
        {
            temp = cellSize_Horizontal;
            cellSize_Horizontal = cellSize_Virtical;
            cellSize_Virtical = temp;
            isRotate = false;
        }
        else
        {
            temp = cellSize_Horizontal;
            cellSize_Horizontal = cellSize_Virtical;
            cellSize_Virtical = temp;
            isRotate = true;
        }
    }
    void RotateTurret()
    {
        currentRotation += 90f;

        if (currentRotation >= 360f)
        {
            currentRotation = 0f;
        }
    }
    public string CurrentPrefab;
    public void SetTurretPrefab(string turretPrefabName)
    {
        // Resources 폴더에서 프리팹을 로드
        GameObject newTurretPrefab = Resources.Load<GameObject>("Prefabs/Towers/" + turretPrefabName);
        GameObject newTurretPreviewPrefab = Resources.Load<GameObject>("Prefabs/Towers/" + turretPrefabName + "_preview");

        if (newTurretPrefab != null)
        {
            // 기존 미리보기 포탑을 비활성화
            if (previewTurret != null)
            {
                Destroy(previewTurret); // 기존 미리보기 포탑을 씬에서 제거
            }

            // 타워 프리팹 설정
            turretPrefab = newTurretPrefab;

            // 새로 미리보기 포탑 생성
            if (GameSettings.IsMultiplayer == false)
            {
                previewTurret = Instantiate(newTurretPreviewPrefab);
                previewTurret.SetActive(false); // 초기에는 비활성화
            }
            if (GameSettings.IsMultiplayer == true)
            {
                previewTurret = PhotonNetwork.Instantiate(newTurretPreviewPrefab.name,PT_V,PT_R);
                previewTurret.SetActive(false); // 초기에는 비활성화
            }
                // 프리팹의 크기 추출
            if (turretPrefabName == "Wall_1")
            {
                cellSize_Horizontal = 1.25f;
                cellSize_Virtical = 0.4f;
                CurrentPrefab = "Wall_1";
            }
            else if (turretPrefabName == "Core")
            {
                cellSize_Horizontal = 2f;
                cellSize_Virtical = 2f;
                CurrentPrefab = "Core";
            }
            else if (turretPrefabName == "ArrowTower_1")
            {
                cellSize_Horizontal = 1.2f;
                cellSize_Virtical = 1.2f;
                CurrentPrefab = "ArrowTower_1";
            }
            else if (turretPrefabName == "MagicTower_1")
            {
                cellSize_Horizontal = 1.2f;
                cellSize_Virtical = 1.2f;
                CurrentPrefab = "MagicTower_1";
            }
            else if (turretPrefabName == "RocketTower_1")
            {
                cellSize_Horizontal = 1.2f;
                cellSize_Virtical = 1.2f;
                CurrentPrefab = "RocketTower_1";
            }
            else if (turretPrefabName == "HealTower_1")
            {
                cellSize_Horizontal = 1.2f;
                cellSize_Virtical = 1.2f;
                CurrentPrefab = "HealTower_1";
            }
            else if (turretPrefabName == "LightTower_1")
            {
                cellSize_Horizontal = 1.2f;
                cellSize_Virtical = 1.2f;
                CurrentPrefab = "LightTower_1";
            }
            else if (turretPrefabName == "SpawnTower_1")
            {
                cellSize_Horizontal = 1.2f;
                cellSize_Virtical = 1.2f;
                CurrentPrefab = "SpawnTower_1";
            }
            else if (turretPrefabName == "SpawnTower_2")
            {
                cellSize_Horizontal = 1.2f;
                cellSize_Virtical = 1.2f;
                CurrentPrefab = "SpawnTower_2";
            }

            // 타워 회전 초기화
            currentRotation = 0f; // 회전 값을 0으로 초기화

            Debug.Log("타워 프리팹의 크기에 따라 cellSize 설정됨: " +
                      "Horizontal = " + cellSize_Horizontal + ", Vertical = " + cellSize_Virtical);
        }
        else
        {
            Debug.LogError("프리팹을 로드할 수 없습니다: Turrets/" + turretPrefabName);
        }
    }

    public void DeactivateHighlightArea()
    {
        if (highlightMeshFilter != null)
        {
            highlightMeshFilter.mesh = null; // 하이라이트 메쉬를 비우기
        }

        if (highlightMeshRenderer != null)
        {
            highlightMeshRenderer.enabled = false; // 하이라이트 렌더러 비활성화
        }

        if (previewTurret != null)
        {
            previewTurret.SetActive(false); // 미리 보기 포탑 비활성화
        }
    }

    public void ActivateHighlightArea()
    {
        // 하이라이트 메쉬와 렌더러 활성화
        if (highlightMeshFilter != null)
        {
            highlightMeshFilter.mesh = BuildHighlightMesh(0, 0, 0, 0); // 메쉬를 다시 설정
        }

        if (highlightMeshRenderer != null)
        {
            highlightMeshRenderer.enabled = true; // 렌더러 활성화
        }

        // 미리 보기 포탑 활성화
        if (previewTurret != null)
        {
            previewTurret.SetActive(true); // 포탑 활성화
        }
    }
}