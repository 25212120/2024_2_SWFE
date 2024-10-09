using UnityEngine;
using System.Collections.Generic;

public class HighlightArea : MonoBehaviour
{
    public float cellSize = 1f; // �׸��� �� ũ��
    public int highlightSize = 3; // ���� ������ ũ�� (nxn)
    public Material highlightMaterial; // ���� ������ ������ ��Ƽ����
    public GameObject turretPrefab; // ��ž ������

    private MeshFilter highlightMeshFilter;

    private Vector3 hitPoint;
    private int cellX;
    private int cellZ;
    private bool isValidHit = false; // ����ĳ��Ʈ ���� ���θ� ����

    private HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>(); // �̹� ��ž�� ��ġ�� ������ ����

    void Start()
    {
        CreateHighlightObject();
    }

    void Update()
    {
        // ���콺 ��ġ ������Ʈ
        UpdateMousePosition();

        // ���� ���� ������Ʈ
        UpdateHighlightArea();

        // ���콺 ���� ��ư Ŭ�� �� ��ž ��ġ
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
        // Camera.main�� null���� Ȯ��
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera�� �������� �ʰų� 'MainCamera' �±װ� �������� �ʾҽ��ϴ�.");
            isValidHit = false;
            return;
        }

        // ���콺 ��ġ�κ��� ����ĳ��Ʈ ����
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // ���̾� ����ũ ���� �Ǵ� ����
        // int layerMask = LayerMask.GetMask("Ground"); // �ٴ� ���̾�� ����

        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) // ���̾� ����ũ ����
        {
            // �׸��� ���� ��ġ ���
            hitPoint = hit.point;

            cellX = Mathf.FloorToInt(hitPoint.x / cellSize);
            cellZ = Mathf.FloorToInt(hitPoint.z / cellSize);

            isValidHit = true; // ����ĳ��Ʈ ����
        }
        else
        {
            // ����ĳ��Ʈ�� ������ ���
            isValidHit = false;
        }
    }

    void UpdateHighlightArea()
    {
        if (isValidHit)
        {
            // ���� ������ ���� ���
            int halfSize = highlightSize / 2;

            float startX = (cellX - halfSize) * cellSize;
            float startZ = (cellZ - halfSize) * cellSize;
            float endX = (cellX + halfSize + 1) * cellSize;
            float endZ = (cellZ + halfSize + 1) * cellSize;

            // ���� ���� �޽� ����
            Mesh mesh = BuildHighlightMesh(startX, startZ, endX, endZ);

            // ���� ������ �޽� ������Ʈ
            if (highlightMeshFilter != null)
            {
                highlightMeshFilter.mesh = mesh;
            }
        }
        else
        {
            // ���콺�� �ٴڿ� ���� ���� ��� ���� ���� ����
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

        // ���ؽ� ����
        vertices[0] = new Vector3(startX, 0.02f, startZ);
        vertices[1] = new Vector3(startX, 0.02f, endZ);
        vertices[2] = new Vector3(endX, 0.02f, startZ);
        vertices[3] = new Vector3(endX, 0.02f, endZ);

        // �ε��� ����
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
            // ���콺�� �ٴڿ� ���� ���� ���
            return;
        }

        Vector2Int cellPosition = new Vector2Int(cellX, cellZ);

        // �ش� ���� �̹� �����Ǿ����� Ȯ��
        if (occupiedCells.Contains(cellPosition))
        {
            Debug.Log("�� ������ �̹� ��ž�� ��ġ�Ǿ� �ֽ��ϴ�.");
            return;
        }

        // ��ž�� ��ġ�� ��ġ ���
        float posX = (cellX + 0.5f) * cellSize;
        float posZ = (cellZ + 0.5f) * cellSize;
        float posY = hitPoint.y; // ������ ���̸� ���
        Vector3 position = new Vector3(posX, posY, posZ);

        // ��ž �������� �Ҵ�Ǿ����� Ȯ��
        if (turretPrefab != null)
        {
            // ��ž ������ �ν��Ͻ�ȭ
            Instantiate(turretPrefab, position, Quaternion.identity);

            // �ش� ���� ������ �� ��Ͽ� �߰�
            occupiedCells.Add(cellPosition);
        }
        else
        {
            Debug.LogError("��ž �������� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }
}
