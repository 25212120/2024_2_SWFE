using UnityEngine;
using System.Collections.Generic;

public class HighlightArea : MonoBehaviour
{
    public float cellSize_Virtical = 1f; // �׸��� ���� ũ��
    public float cellSize_Horizontal = 1f; // �׸��� ���� ũ��
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

    public Vector3 gridStartPosition = new Vector3(0, 0, 0); // �׸����� ���� ��ġ (����)


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
            SetRotatestate();
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
            // ���콺�� �׸����� ���� ��ġ���� �󸶳� ������ �ִ��� ���
            Vector3 offset = hit.point - gridStartPosition; // ���� ��ġ �������� ���콺 ������ ���

            // �׸��� �� ��ǥ ���
            cellX = Mathf.FloorToInt(offset.x / cellSize_Horizontal);
            cellZ = Mathf.FloorToInt(offset.z / cellSize_Virtical);

            hitPoint = hit.point;
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

            float startX = (cellX - halfSize) * cellSize_Horizontal;
            float startZ = (cellZ - halfSize) * cellSize_Virtical;
            float endX = (cellX + halfSize + 1) * cellSize_Horizontal;
            float endZ = (cellZ + halfSize + 1) * cellSize_Virtical;

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
                Vector3 cellCenter = new Vector3(x * cellSize_Horizontal + cellSize_Horizontal / 2, hitPoint.y + 0.5f, z * cellSize_Virtical + cellSize_Virtical / 2);
                Vector3 halfExtents = new Vector3(cellSize_Horizontal / 2, 0.5f, cellSize_Virtical / 2);

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
            float posX = (cellX + 0.5f) * cellSize_Horizontal;
            float posZ = (cellZ + 0.5f) * cellSize_Virtical;
            float posY = hitPoint.y; // ������ ���̸� ���
            Vector3 position = new Vector3(posX, posY, posZ);

            // ȸ�� �� ����
            Quaternion rotation = Quaternion.Euler(0f, currentRotation, 0f);
            previewTurret.transform.SetPositionAndRotation(position, rotation);

            // �ݶ��̴��� ��Ȱ��ȭ�Ͽ� ���� �浹 �˻翡�� ����
            Collider turretCollider = previewTurret.GetComponent<Collider>();
            if (turretCollider != null)
            {
                turretCollider.enabled = false; // �̸������ �ݶ��̴� ��Ȱ��ȭ
            }
        }
        else
        {
            // ���콺�� �ٴڿ� ���� ���� ��� �̸����� ��ž ��Ȱ��ȭ
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
        float posX = (cellX + 0.5f) * cellSize_Horizontal;
        float posZ = (cellZ + 0.5f) * cellSize_Virtical;
        float posY = hitPoint.y; // ������ ���̸� ���
        Vector3 position = new Vector3(posX, posY, posZ);

        if (turretPrefab != null)
        {
            Quaternion rotation = Quaternion.Euler(0f, currentRotation, 0f);
            GameObject newTurret = Instantiate(turretPrefab, position, rotation);

            // Ÿ�� ��ġ �� �ݶ��̴��� Ȱ��ȭ
            Collider turretCollider = newTurret.GetComponent<Collider>();
            if (turretCollider != null)
            {
                turretCollider.enabled = true; // ��ġ �� �ݶ��̴� Ȱ��ȭ
            }

            // ��ġ�� Ÿ���� �� ���� ���� ������Ʈ
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
}