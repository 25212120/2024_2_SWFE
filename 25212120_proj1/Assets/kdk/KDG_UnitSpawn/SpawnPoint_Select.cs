using UnityEngine;

public class SpawnPoint_Select : MonoBehaviour
{
    public float cellSize = 1f; // 그리드 셀 크기
    public Material highlightMaterial; // 강조 영역에 적용할 머티리얼
    public Material ImpossibleMaterial; // 설치 불가 영역에 적용할 머티리얼
    public float towerRange = 1f; // 각 타워의 스폰 반경

    private MeshFilter highlightMeshFilter;
    private MeshRenderer highlightMeshRenderer;

    private Vector3 hitPoint;
    private int cellX;
    private int cellZ;
    private bool isValidHit = false; // 레이캐스트 성공 여부
    private bool isValidPlacement = false; // 배치 가능 여부
    public OccupiedCell_Manager occupiedCell_Manager;

    // 새로운 변수: 설치된 SpawnStructure들
    public SpawnStructure[] spawnStructures; // SpawnStructure들을 담을 배열

    void Start()
    {
        CreateHighlightObject();
        if (occupiedCell_Manager == null)
        {
            occupiedCell_Manager = FindObjectOfType<OccupiedCell_Manager>();
        }

        // 설치된 SpawnStructure들을 찾아서 배열에 저장
        spawnStructures = FindObjectsOfType<SpawnStructure>();
    }

    void Update()
    {
        // 마우스 위치 업데이트
        UpdateMousePosition();

        // 강조 영역 업데이트
        UpdateHighlightArea();

        // 유닛 스폰 포인트 설정 (우클릭으로 설정)
        if (Input.GetMouseButtonDown(0) && isValidHit && isValidPlacement)
        {
            SetSpawnPointForTower(); // 타워에 맞는 스폰 포인트 설정
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
            // 그리드 상의 위치 계산
            hitPoint = hit.point;

            // 셀의 X, Z 좌표를 1x1 셀로 반영
            cellX = Mathf.FloorToInt(hitPoint.x / cellSize);
            cellZ = Mathf.FloorToInt(hitPoint.z / cellSize);

            isValidHit = true;
        }
        else
        {
            // 레이캐스트가 실패한 경우
            isValidHit = false;
        }
    }

    void UpdateHighlightArea()
    {
        if (isValidHit)
        {
            // 강조 영역의 범위 계산 (1x1 영역)
            float startX = cellX * cellSize;
            float startZ = cellZ * cellSize;
            float endX = (cellX + 1) * cellSize;
            float endZ = (cellZ + 1) * cellSize;

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
                if (isValidPlacement)
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
        }
        else
        {
            // 마우스가 바닥에 닿지 않을 경우 강조 영역 비우기
            if (highlightMeshFilter != null)
            {
                highlightMeshFilter.mesh = null;
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
        // 이미 배치된 셀에 배치 불가
        if (occupiedCell_Manager.GetOccupiedCells().Contains(new Vector2Int(cellX, cellZ)))
        {
            return false;
        }

        // 설치된 SpawnStructure들의 범위 내에 있는지만 확인 (towerRange는 제거)
        foreach (var spawnStructure in spawnStructures)
        {
            // SpawnStructure가 설치된 셀의 위치 계산
            Vector2Int structureCell = new Vector2Int(Mathf.FloorToInt(spawnStructure.transform.position.x / cellSize),
                                                      Mathf.FloorToInt(spawnStructure.transform.position.z / cellSize));

            // 설치된 구조물 주변 셀에만 스폰 포인트를 설정 가능하도록 수정
            if (IsAdjacentCell(structureCell, new Vector2Int(cellX, cellZ)))
            {
                Unit_SpawnManager spawnManager = spawnStructure.unitSpawnManager;

                // `CanSpawn()`이 가능하다면 스폰 가능
                if (spawnManager != null && spawnManager.CanSpawn())
                {
                    return true;
                }
            }
        }

        // 범위 내에서 배치할 수 없으면 false
        return false;
    }

    void SetSpawnPointForTower()
    {
        Vector2Int cellPosition = new Vector2Int(cellX, cellZ); // 셀 단위 좌표로 변환

        bool isSpawnPointSet = false; // 스폰 포인트 설정 여부를 추적

        foreach (var spawnStructure in spawnStructures)
        {
            Vector2Int structureCell = new Vector2Int(Mathf.FloorToInt(spawnStructure.transform.position.x / cellSize),
                                                      Mathf.FloorToInt(spawnStructure.transform.position.z / cellSize));

            // 해당 SpawnStructure의 인접 셀에서만 스폰 포인트 설정 가능
            if (IsAdjacentCell(structureCell, cellPosition))
            {
                Unit_SpawnManager spawnManager = spawnStructure.unitSpawnManager;

                if (spawnManager != null && spawnManager.CanSpawn()) // 스폰이 가능한 타워인지 확인
                {
                    spawnStructure.unitSpawnManager.SetSpawnPoint(cellPosition);

                    occupiedCell_Manager.AddOccupiedCell(cellPosition);

                    Debug.Log($"스폰 포인트가 {spawnStructure.name}에 설정되었습니다.");
                    isSpawnPointSet = true;
                    break;
                }
                else
                {
                    Debug.LogWarning($"SpawnStructure에 Unit_SpawnManager가 할당되지 않았습니다. ({spawnStructure.name})");
                }
            }
        }

        if (!isSpawnPointSet)
        {
            Debug.LogWarning("스폰 포인트를 설정할 수 있는 범위 내에 설치된 SpawnStructure가 없습니다.");
        }
    }

    bool IsAdjacentCell(Vector2Int cell1, Vector2Int cell2)
    {
        // 두 셀이 인접한 셀인지를 확인하는 함수 (1칸 범위 내)
        return Mathf.Abs(cell1.x - cell2.x) <= 1 && Mathf.Abs(cell1.y - cell2.y) <= 1;
    }
}

