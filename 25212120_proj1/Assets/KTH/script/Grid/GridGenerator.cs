using UnityEngine;
using System.Collections.Generic;

public class GridRenderer : MonoBehaviour
{
    public float cellSize = 1f; // �� ���� ũ��
    public int gridExtent = 100; // �׸��带 �׸� ���� (ī�޶�κ����� �Ÿ�)
    public Material lineMaterial; // �׸��� ���ο� ������ ��Ƽ���� (Custom/AlwaysOnTop ���̴� ���)

    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private Vector3 lastCameraPosition;

    void Start()
    {
        // ��Ƽ���� �ν��Ͻ� ���� �� ���̴� ����
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

        // ī�޶� �� ũ�⸸ŭ �̵����� ���� �׸��� ������Ʈ
        if (Vector3.Distance(cameraPosition, lastCameraPosition) >= cellSize)
        {
            UpdateGrid();
            lastCameraPosition = cameraPosition;
        }
    }

    void CreateGrid()
    {
        // �ʱ� �׸��� ����
        UpdateGrid();
    }

    void UpdateGrid()
    {
        // ī�޶� ��ġ�� �������� �׸��带 ������Ʈ
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 gridOrigin = new Vector3(
            Mathf.Floor(cameraPosition.x / cellSize) * cellSize,
            0,
            Mathf.Floor(cameraPosition.z / cellSize) * cellSize
        );

        // ���� �׸��� ����
        ClearGrid();

        // �׸��� ���� ���� ���
        int halfExtent = gridExtent / 2;

        // ���μ� �׸���
        for (int i = -halfExtent; i <= halfExtent; i++)
        {
            float zPos = gridOrigin.z + i * cellSize;
            CreateLine(
                new Vector3(gridOrigin.x - halfExtent * cellSize, 0f, zPos),
                new Vector3(gridOrigin.x + halfExtent * cellSize, 0f, zPos)
            );
        }

        // ���μ� �׸���
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

        // ���� ������Ʈ�� �ڽ����� �����Ͽ� ����
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
