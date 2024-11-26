using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.UIElements;
using System.Net.NetworkInformation;

public class HighlightArea : MonoBehaviour
{
    public float cellSize_Virtical = 1f; // �׸��� ���� ũ��
    public float cellSize_Horizontal = 1f; // �׸��� ���� ũ��
    public int highlightSize = 3; // ���� ������ ũ�� (nxn)
    public int detailGridResolution = 3; // ���� ���� ���� ���� �׸��� ���� ��
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
    public TowerSpawn_Manager towerSpawn_Manager;

    private float currentRotation = 0f; // ���� Ÿ���� ȸ�� ����
    private GameObject previewTurret; // �̸� ����� ��ž �ν��Ͻ�

    public Vector3 gridStartPosition = new Vector3(0, 0, 0); // �׸����� ���� ��ġ (����)

    public List<GameObject> coreObjects; // ���� ���� �ھ� ������Ʈ
    public float corePlacementRange = 20f; // Core ������Ʈ ��ó���� ��ġ�� �� �ִ� ����

    public Vector3 PT_V = new Vector3(0, 0, 0);
    public Quaternion PT_R = Quaternion.Euler(0, 0, 0);

    public bool isActive = true;

    void Start()
    {
        CreateHighlightObject();
        if (occupiedCell_Manager == null)
        {
            occupiedCell_Manager = FindObjectOfType<OccupiedCell_Manager>();  // �ڵ����� ã��
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

        // Q Ű�� ���� Ÿ�� ȸ��
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
            Debug.Log("����ĳ��Ʈ����");
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
                if (isValidPlacement && towerSpawn_Manager.CheckIfResourcesAreSufficient(CurrentPrefab))
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
                Debug.Log("�ھ��ó�ƴ�");
                return false; // �ھ� ��ó�� �ƴϸ� ��ġ �Ұ�
            }
        }
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
            if (GameSettings.IsMultiplayer == false)
            {            // �̸� ����� ��ž�� ����
                previewTurret = Instantiate(turretPrefab);
                previewTurret.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ
            }
            if (GameSettings.IsMultiplayer == true)
            {
                PhotonNetwork.Instantiate(turretPrefab.name, PT_V, PT_R);
                previewTurret.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ

            }
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

            // �ڽ� ������Ʈ�� �ݶ��̴��� ��Ȱ��ȭ
            Collider[] turretColliders = previewTurret.GetComponentsInChildren<Collider>();
            foreach (Collider col in turretColliders)
            {
                col.enabled = false; // ��� �ڽ� ������Ʈ�� �ݶ��̴� ��Ȱ��ȭ
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
            if (GameSettings.IsMultiplayer == false)
            {
                if (towerSpawn_Manager.SpawnAndConsumeMaterial(CurrentPrefab))
                {
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
                    Debug.Log("�ڿ��������� ��ġ �Ұ�");
                }
            }
            if (GameSettings.IsMultiplayer == true)
            {
                if (towerSpawn_Manager.SpawnAndConsumeMaterial(CurrentPrefab))
                {
                    GameObject newTurret = PhotonNetwork.Instantiate(turretPrefab.name, position, rotation);
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
    public string CurrentPrefab;
    public void SetTurretPrefab(string turretPrefabName)
    {
        // Resources �������� �������� �ε�
        GameObject newTurretPrefab = Resources.Load<GameObject>("Prefabs/Towers/" + turretPrefabName);
        GameObject newTurretPreviewPrefab = Resources.Load<GameObject>("Prefabs/Towers/" + turretPrefabName + "_preview");

        if (newTurretPrefab != null)
        {
            // ���� �̸����� ��ž�� ��Ȱ��ȭ
            if (previewTurret != null)
            {
                Destroy(previewTurret); // ���� �̸����� ��ž�� ������ ����
            }

            // Ÿ�� ������ ����
            turretPrefab = newTurretPrefab;

            // ���� �̸����� ��ž ����
            if (GameSettings.IsMultiplayer == false)
            {
                previewTurret = Instantiate(newTurretPreviewPrefab);
                previewTurret.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ
            }
            if (GameSettings.IsMultiplayer == true)
            {
                previewTurret = PhotonNetwork.Instantiate(newTurretPreviewPrefab.name,PT_V,PT_R);
                previewTurret.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ
            }
                // �������� ũ�� ����
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

            // Ÿ�� ȸ�� �ʱ�ȭ
            currentRotation = 0f; // ȸ�� ���� 0���� �ʱ�ȭ

            Debug.Log("Ÿ�� �������� ũ�⿡ ���� cellSize ������: " +
                      "Horizontal = " + cellSize_Horizontal + ", Vertical = " + cellSize_Virtical);
        }
        else
        {
            Debug.LogError("�������� �ε��� �� �����ϴ�: Turrets/" + turretPrefabName);
        }
    }

    public void DeactivateHighlightArea()
    {
        if (highlightMeshFilter != null)
        {
            highlightMeshFilter.mesh = null; // ���̶���Ʈ �޽��� ����
        }

        if (highlightMeshRenderer != null)
        {
            highlightMeshRenderer.enabled = false; // ���̶���Ʈ ������ ��Ȱ��ȭ
        }

        if (previewTurret != null)
        {
            previewTurret.SetActive(false); // �̸� ���� ��ž ��Ȱ��ȭ
        }
    }

    public void ActivateHighlightArea()
    {
        // ���̶���Ʈ �޽��� ������ Ȱ��ȭ
        if (highlightMeshFilter != null)
        {
            highlightMeshFilter.mesh = BuildHighlightMesh(0, 0, 0, 0); // �޽��� �ٽ� ����
        }

        if (highlightMeshRenderer != null)
        {
            highlightMeshRenderer.enabled = true; // ������ Ȱ��ȭ
        }

        // �̸� ���� ��ž Ȱ��ȭ
        if (previewTurret != null)
        {
            previewTurret.SetActive(true); // ��ž Ȱ��ȭ
        }
    }
}