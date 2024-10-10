using UnityEngine;
using System.Collections.Generic;

public class HighlightArea : MonoBehaviour
{
    public float cellSize = 1f; // 그리드 셀 크기
    public int highlightSize = 3; // 강조 영역의 크기 (nxn)
    public Material highlightMaterial; // 강조 영역에 적용할 머티리얼
    public GameObject turretPrefab; // 포탑 프리팹

    private MeshFilter highlightMeshFilter;

    private Vector3 hitPoint;
    private int cellX;
    private int cellZ;
    private bool isValidHit = false; // 레이캐스트 성공 여부를 추적

    private HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>(); // 이미 포탑이 배치된 셀들을 추적

    void Start()
    {
        CreateHighlightObject();
    }

    void Update()
    {
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
        MeshRenderer meshRenderer = highlightGO.AddComponent<MeshRenderer>();
        meshRenderer.material = highlightMaterial;

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
        // 레이어 마스크 제거 또는 수정
        // int layerMask = LayerMask.GetMask("Ground"); // 바닥 레이어로 설정

        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) // 레이어 마스크 제거
        {
            // 그리드 상의 위치 계산
            hitPoint = hit.point;

            cellX = Mathf.FloorToInt(hitPoint.x / cellSize);
            cellZ = Mathf.FloorToInt(hitPoint.z / cellSize);

            isValidHit = true; // 레이캐스트 성공
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

    void PlaceTurret()
    {
        if (!isValidHit)
        {
            // 마우스가 바닥에 닿지 않은 경우
            return;
        }

        Vector2Int cellPosition = new Vector2Int(cellX, cellZ);

        // 해당 셀이 이미 점유되었는지 확인
        if (occupiedCells.Contains(cellPosition))
        {
            Debug.Log("이 셀에는 이미 포탑이 배치되어 있습니다.");
            return;
        }

        // 포탑을 배치할 위치 계산
        float posX = (cellX + 0.5f) * cellSize;
        float posZ = (cellZ + 0.5f) * cellSize;
        float posY = hitPoint.y; // 지형의 높이를 사용
        Vector3 position = new Vector3(posX, posY, posZ);

        // 포탑 프리팹이 할당되었는지 확인
        if (turretPrefab != null)
        {
            // 포탑 프리팹 인스턴스화
            Instantiate(turretPrefab, position, Quaternion.identity);

            // 해당 셀을 점유된 셀 목록에 추가
            occupiedCells.Add(cellPosition);
        }
        else
        {
            Debug.LogError("포탑 프리팹이 할당되지 않았습니다.");
        }
    }
}
