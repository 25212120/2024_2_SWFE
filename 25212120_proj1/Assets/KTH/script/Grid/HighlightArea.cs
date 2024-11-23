using UnityEngine;
using System.Collections.Generic;

public class HighlightArea : MonoBehaviour
{
    public float cellSize = 1f; // 그리드 셀 크기
    public int highlightSize = 3; // 강조 영역의 크기 (nxn)
    public int detailGridResolution = 2; // 강조 영역 내의 세부 그리드 분할 수
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

    private float currentRotation = 0f; // 현재 타워의 회전 각도
    private GameObject previewTurret; // 미리 보기용 포탑 인스턴스

    void Start()
    {
        CreateHighlightObject();
        if (occupiedCell_Manager == null)
        {
            occupiedCell_Manager = FindObjectOfType<OccupiedCell_Manager>();  // 자동으로 찾기
        }
    }

    void Update()
    {
        // Q 키를 눌러 타워 회전
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateTurret();
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
            // 그리드 상의 위치 계산
            hitPoint = hit.point;

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
            // 강조 영역의 범위 계산
            int halfSize = highlightSize / 2;

            float startX = (cellX - halfSize) * cellSize;
            float startZ = (cellZ - halfSize) * cellSize;
            float endX = (cellX + halfSize + 1) * cellSize;
            float endZ = (cellZ + halfSize + 1) * cellSize;

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
                Vector3 cellCenter = new Vector3(x * cellSize + cellSize / 2, hitPoint.y + 0.5f, z * cellSize + cellSize / 2);
                Vector3 halfExtents = new Vector3(cellSize / 2, 0.5f, cellSize / 2);

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
            // 미리 보기용 포탑을 생성
            previewTurret = Instantiate(turretPrefab);
            previewTurret.SetActive(false); // 초기에는 비활성화
        }

        // 미리 보기용 포탑을 활성화하고 위치 및 회전 적용
        if (isValidHit && previewTurret != null)
        {
            previewTurret.SetActive(true);

            // 포탑의 위치는 하이라이트 영역의 중앙으로 설정
            float posX = (cellX + 0.5f) * cellSize;
            float posZ = (cellZ + 0.5f) * cellSize;
            float posY = hitPoint.y; // 지형의 높이를 사용
            Vector3 position = new Vector3(posX, posY, posZ);

            // 회전 값 적용
            Quaternion rotation = Quaternion.Euler(0f, currentRotation, 0f);
            previewTurret.transform.SetPositionAndRotation(position, rotation);
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
        float posX = (cellX + 0.5f) * cellSize;
        float posZ = (cellZ + 0.5f) * cellSize;
        float posY = hitPoint.y; // 지형의 높이를 사용
        Vector3 position = new Vector3(posX, posY, posZ);
        /*
        // 포탑 프리팹이 할당되었는지 확인
        if (turretPrefab != null)
        {
            // 포탑 프리팹 인스턴스화
            Instantiate(turretPrefab, position, Quaternion.identity);

            // 해당 셀들을 점유된 셀 목록에 추가
            foreach (Vector2Int cellPos in cellsToOccupy)
            {
                occupiedCells.Add(cellPos);
            }
        }
        else
        {
            Debug.LogError("포탑 프리팹이 할당되지 않았습니다.");
        }
        */
        if (turretPrefab != null)
        {
            Quaternion rotation = Quaternion.Euler(0f, currentRotation, 0f);
            Instantiate(turretPrefab, position, rotation);

            foreach (Vector2Int cellPos in cellsToOccupy)
            {
                occupiedCell_Manager.occupiedCells.Add(cellPos);
            }
        }
        else
        {
            Debug.LogError("포탑 프리팹이 할당되지 않았습니다.");
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
}