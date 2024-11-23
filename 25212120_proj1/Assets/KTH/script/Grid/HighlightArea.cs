using UnityEngine;
using System.Collections.Generic;

public class HighlightArea : MonoBehaviour
{
    public float cellSize = 1f; // �׸��� �� ũ��
    public int highlightSize = 3; // ���� ������ ũ�� (nxn)
    public int detailGridResolution = 2; // ���� ���� ���� ���� �׸��� ���� ��
    public Material highlightMaterial; // ���� ������ ������ ��Ƽ����
    public Material ImpossibleMaterial; // ��ġ �Ұ� ������ ������ ��Ƽ����
    public GameObject turretPrefab; // ��ž ������

    private MeshFilter highlightMeshFilter;
    private MeshRenderer highlightMeshRenderer;

    private Vector3 hitPoint;
    private int cellX;
    private int cellZ;
    private bool isValidHit = false; // ����ĳ��Ʈ ���� ����
    private bool isValidPlacement = false; // ��ġ ���� ����
    public OccupiedCell_Manager occupiedCell_Manager;

    private float currentRotation = 0f; // ���� Ÿ���� ȸ�� ����
    private GameObject previewTurret; // �̸� ����� ��ž �ν��Ͻ�

    void Start()
    {
        CreateHighlightObject();
        if (occupiedCell_Manager == null)
        {
            occupiedCell_Manager = FindObjectOfType<OccupiedCell_Manager>();  // �ڵ����� ã��
        }
    }

    void Update()
    {
        // Q Ű�� ���� Ÿ�� ȸ��
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateTurret();
        }
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
        highlightMeshRenderer = highlightGO.AddComponent<MeshRenderer>();
        highlightMeshRenderer.material = new Material(highlightMaterial); // ��Ƽ���� �ν��Ͻ� ����

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

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // �׸��� ���� ��ġ ���
            hitPoint = hit.point;

            cellX = Mathf.FloorToInt(hitPoint.x / cellSize);
            cellZ = Mathf.FloorToInt(hitPoint.z / cellSize);

            isValidHit = true;
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

            // ��ġ ���� ���� Ȯ��
            isValidPlacement = CheckPlacementValidity();

            // ���� ������ ���� ������Ʈ
            if (highlightMeshRenderer != null)
            {
                if (isValidPlacement)
                {
                    // ��ġ ����: ���
                    highlightMeshRenderer.material = highlightMaterial;
                }
                else
                {
                    // ��ġ �Ұ�: ������
                    highlightMeshRenderer.material = ImpossibleMaterial;
                }
            }
            UpdatePreviewTurret();

        }
        else
        {
            // ���콺�� �ٴڿ� ���� ���� ��� ���� ���� ����
            if (highlightMeshFilter != null)
            {
                highlightMeshFilter.mesh = null;
            }

            // �̸����� ��ž ��Ȱ��ȭ
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

    bool CheckPlacementValidity()
    {
        int halfSize = highlightSize / 2;

        for (int x = cellX - halfSize; x <= cellX + halfSize; x++)
        {
            for (int z = cellZ - halfSize; z <= cellZ + halfSize; z++)
            {
                Vector2Int cellPos = new Vector2Int(x, z);

                // �� ���� ���� Ȯ��
                if (occupiedCell_Manager.occupiedCells.Contains(cellPos))
                {
                    return false; // �̹� ������ ���� ����
                }

                // �� �߽� ��ǥ ���
                Vector3 cellCenter = new Vector3(x * cellSize + cellSize / 2, hitPoint.y + 0.5f, z * cellSize + cellSize / 2);
                Vector3 halfExtents = new Vector3(cellSize / 2, 0.5f, cellSize / 2);

                // �ڽ� �ݶ��̴��� �˻�
                Collider[] colliders = Physics.OverlapBox(cellCenter, halfExtents, Quaternion.identity);

                foreach (Collider collider in colliders)
                {
                    // Ground �±װ� �ƴ� ��� ��ġ �Ұ�
                    if (collider.tag != "Ground")
                    {
                        return false;
                    }
                }
            }
        }

        return true; // ��� ������ ��ġ ������
    }

    void UpdatePreviewTurret()
    {
        if (previewTurret == null)
        {
            // �̸� ����� ��ž�� ����
            previewTurret = Instantiate(turretPrefab);
            previewTurret.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ
        }

        // �̸� ����� ��ž�� Ȱ��ȭ�ϰ� ��ġ �� ȸ�� ����
        if (isValidHit && previewTurret != null)
        {
            previewTurret.SetActive(true);

            // ��ž�� ��ġ�� ���̶���Ʈ ������ �߾����� ����
            float posX = (cellX + 0.5f) * cellSize;
            float posZ = (cellZ + 0.5f) * cellSize;
            float posY = hitPoint.y; // ������ ���̸� ���
            Vector3 position = new Vector3(posX, posY, posZ);

            // ȸ�� �� ����
            Quaternion rotation = Quaternion.Euler(0f, currentRotation, 0f);
            previewTurret.transform.SetPositionAndRotation(position, rotation);
        }
    }

    void PlaceTurret()
    {
        if (!isValidHit || !isValidPlacement)
        {
            // ��ġ �Ұ�
            return;
        }

        // ���� ������ ���� ���
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

        // ��ž�� ��ġ�� ��ġ ��� (���� ������ �߾�)
        float posX = (cellX + 0.5f) * cellSize;
        float posZ = (cellZ + 0.5f) * cellSize;
        float posY = hitPoint.y; // ������ ���̸� ���
        Vector3 position = new Vector3(posX, posY, posZ);
        /*
        // ��ž �������� �Ҵ�Ǿ����� Ȯ��
        if (turretPrefab != null)
        {
            // ��ž ������ �ν��Ͻ�ȭ
            Instantiate(turretPrefab, position, Quaternion.identity);

            // �ش� ������ ������ �� ��Ͽ� �߰�
            foreach (Vector2Int cellPos in cellsToOccupy)
            {
                occupiedCells.Add(cellPos);
            }
        }
        else
        {
            Debug.LogError("��ž �������� �Ҵ���� �ʾҽ��ϴ�.");
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
            Debug.LogError("��ž �������� �Ҵ���� �ʾҽ��ϴ�.");
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