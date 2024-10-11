using UnityEngine;
using System.Collections.Generic;

public class GridRenderer : MonoBehaviour
{
    public float cellSize = 1f; // 각 셀의 크기
    public int gridExtent = 100; // 그리드를 그릴 범위 (카메라로부터의 거리)
    public Material lineMaterial; // 그리드 라인에 적용할 머티리얼 (Custom/AlwaysOnTop 셰이더 사용)

    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private Vector3 lastCameraPosition;

    void Start()
    {
        // 머티리얼 인스턴스 생성 및 셰이더 적용
        if (lineMaterial != null)
        {
            lineMaterial = new Material(lineMaterial);
            lineMaterial.shader = Shader.Find("Custom/AlwaysOnTop");
        }

        CreateGrid();

        lastCameraPosition = Camera.main.transform.position;
    }

    void Update()
    {
        Vector3 cameraPosition = Camera.main.transform.position;

        // 카메라가 셀 크기만큼 이동했을 때만 그리드 업데이트
        if (Vector3.Distance(cameraPosition, lastCameraPosition) >= cellSize)
        {
            UpdateGrid();
            lastCameraPosition = cameraPosition;
        }
    }

    void CreateGrid()
    {
        // 초기 그리드 생성
        UpdateGrid();
    }

    void UpdateGrid()
    {
        // 카메라 위치를 기준으로 그리드를 업데이트
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 gridOrigin = new Vector3(
            Mathf.Floor(cameraPosition.x / cellSize) * cellSize,
            0,
            Mathf.Floor(cameraPosition.z / cellSize) * cellSize
        );

        // 기존 그리드 삭제
        ClearGrid();

        // 그리드 생성 범위 계산
        int halfExtent = gridExtent / 2;

        // 가로선 그리기
        for (int i = -halfExtent; i <= halfExtent; i++)
        {
            float zPos = gridOrigin.z + i * cellSize;
            CreateLine(
                new Vector3(gridOrigin.x - halfExtent * cellSize, 0f, zPos),
                new Vector3(gridOrigin.x + halfExtent * cellSize, 0f, zPos)
            );
        }

        // 세로선 그리기
        for (int i = -halfExtent; i <= halfExtent; i++)
        {
            float xPos = gridOrigin.x + i * cellSize;
            CreateLine(
                new Vector3(xPos, 0f, gridOrigin.z - halfExtent * cellSize),
                new Vector3(xPos, 0f, gridOrigin.z + halfExtent * cellSize)
            );
        }
    }

    void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.03f;
        lineRenderer.endWidth = 0.03f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // 라인 오브젝트를 자식으로 설정하여 정리
        lineObj.transform.parent = transform;

        lineRenderers.Add(lineRenderer);
    }

    void ClearGrid()
    {
        foreach (LineRenderer lineRenderer in lineRenderers)
        {
            if (lineRenderer != null)
            {
                Destroy(lineRenderer.gameObject);
            }
        }
        lineRenderers.Clear();
    }
}
