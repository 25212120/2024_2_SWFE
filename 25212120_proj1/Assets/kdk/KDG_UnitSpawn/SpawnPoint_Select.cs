using UnityEngine;

public class SpawnPoint_Select : MonoBehaviour
{
    public float cellSize = 1f; // �׸��� �� ũ��
    public Material highlightMaterial; // ���� ������ ������ ��Ƽ����
    public Material ImpossibleMaterial; // ��ġ �Ұ� ������ ������ ��Ƽ����
    public float towerRange = 1f; // �� Ÿ���� ���� �ݰ�

    private MeshFilter highlightMeshFilter;
    private MeshRenderer highlightMeshRenderer;

    private Vector3 hitPoint;
    private int cellX;
    private int cellZ;
    private bool isValidHit = false; // ����ĳ��Ʈ ���� ����
    private bool isValidPlacement = false; // ��ġ ���� ����
    public OccupiedCell_Manager occupiedCell_Manager;

    // ���ο� ����: ��ġ�� SpawnStructure��
    public SpawnStructure[] spawnStructures; // SpawnStructure���� ���� �迭

    void Start()
    {
        CreateHighlightObject();
        if (occupiedCell_Manager == null)
        {
            occupiedCell_Manager = FindObjectOfType<OccupiedCell_Manager>();
        }

        // ��ġ�� SpawnStructure���� ã�Ƽ� �迭�� ����
        spawnStructures = FindObjectsOfType<SpawnStructure>();
    }

    void Update()
    {
        // ���콺 ��ġ ������Ʈ
        UpdateMousePosition();

        // ���� ���� ������Ʈ
        UpdateHighlightArea();

        // ���� ���� ����Ʈ ���� (��Ŭ������ ����)
        if (Input.GetMouseButtonDown(0) && isValidHit && isValidPlacement)
        {
            SetSpawnPointForTower(); // Ÿ���� �´� ���� ����Ʈ ����
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

            // ���� X, Z ��ǥ�� 1x1 ���� �ݿ�
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
            // ���� ������ ���� ��� (1x1 ����)
            float startX = cellX * cellSize;
            float startZ = cellZ * cellSize;
            float endX = (cellX + 1) * cellSize;
            float endZ = (cellZ + 1) * cellSize;

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

    bool CheckPlacementValidity()
    {
        // �̹� ��ġ�� ���� ��ġ �Ұ�
        if (occupiedCell_Manager.GetOccupiedCells().Contains(new Vector2Int(cellX, cellZ)))
        {
            return false;
        }

        // ��ġ�� SpawnStructure���� ���� ���� �ִ����� Ȯ�� (towerRange�� ����)
        foreach (var spawnStructure in spawnStructures)
        {
            // SpawnStructure�� ��ġ�� ���� ��ġ ���
            Vector2Int structureCell = new Vector2Int(Mathf.FloorToInt(spawnStructure.transform.position.x / cellSize),
                                                      Mathf.FloorToInt(spawnStructure.transform.position.z / cellSize));

            // ��ġ�� ������ �ֺ� ������ ���� ����Ʈ�� ���� �����ϵ��� ����
            if (IsAdjacentCell(structureCell, new Vector2Int(cellX, cellZ)))
            {
                Unit_SpawnManager spawnManager = spawnStructure.unitSpawnManager;

                // `CanSpawn()`�� �����ϴٸ� ���� ����
                if (spawnManager != null && spawnManager.CanSpawn())
                {
                    return true;
                }
            }
        }

        // ���� ������ ��ġ�� �� ������ false
        return false;
    }

    void SetSpawnPointForTower()
    {
        Vector2Int cellPosition = new Vector2Int(cellX, cellZ); // �� ���� ��ǥ�� ��ȯ

        bool isSpawnPointSet = false; // ���� ����Ʈ ���� ���θ� ����

        foreach (var spawnStructure in spawnStructures)
        {
            Vector2Int structureCell = new Vector2Int(Mathf.FloorToInt(spawnStructure.transform.position.x / cellSize),
                                                      Mathf.FloorToInt(spawnStructure.transform.position.z / cellSize));

            // �ش� SpawnStructure�� ���� �������� ���� ����Ʈ ���� ����
            if (IsAdjacentCell(structureCell, cellPosition))
            {
                Unit_SpawnManager spawnManager = spawnStructure.unitSpawnManager;

                if (spawnManager != null && spawnManager.CanSpawn()) // ������ ������ Ÿ������ Ȯ��
                {
                    spawnStructure.unitSpawnManager.SetSpawnPoint(cellPosition);

                    occupiedCell_Manager.AddOccupiedCell(cellPosition);

                    Debug.Log($"���� ����Ʈ�� {spawnStructure.name}�� �����Ǿ����ϴ�.");
                    isSpawnPointSet = true;
                    break;
                }
                else
                {
                    Debug.LogWarning($"SpawnStructure�� Unit_SpawnManager�� �Ҵ���� �ʾҽ��ϴ�. ({spawnStructure.name})");
                }
            }
        }

        if (!isSpawnPointSet)
        {
            Debug.LogWarning("���� ����Ʈ�� ������ �� �ִ� ���� ���� ��ġ�� SpawnStructure�� �����ϴ�.");
        }
    }

    bool IsAdjacentCell(Vector2Int cell1, Vector2Int cell2)
    {
        // �� ���� ������ �������� Ȯ���ϴ� �Լ� (1ĭ ���� ��)
        return Mathf.Abs(cell1.x - cell2.x) <= 1 && Mathf.Abs(cell1.y - cell2.y) <= 1;
    }
}

