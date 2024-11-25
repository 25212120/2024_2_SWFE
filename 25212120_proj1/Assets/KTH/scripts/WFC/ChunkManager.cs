using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class ChunkManager : MonoBehaviourPunCallbacks
{
    public int chunkSize = 10; // ûũ ũ�� (�� ����)
    public int gridSize = 5;    // nxn ������ ûũ ���� (Ȧ���� ����)
    public static float cellSize = 50f; // Ÿ���� ũ��

    private Dictionary<Vector2Int, Chunk> existingChunks = new Dictionary<Vector2Int, Chunk>(); // �̹� ������ ûũ
    private Vector2Int currentPlayerChunk; // ���� �÷��̾ ��ġ�� ûũ ��ǥ

    // �ʿ��� ������Ʈ ����
    private BiomeManager biomeManager;
    private TileLoader tileLoader;
    private GameObject player; // ���� �÷��̾� ������Ʈ

    void Start()
    {
        // �ʿ��� ������Ʈ ��������
        biomeManager = FindObjectOfType<BiomeManager>();
        tileLoader = FindObjectOfType<TileLoader>();

        if (biomeManager == null || tileLoader == null)
        {
            Debug.LogError("BiomeManager �Ǵ� TileLoader�� ã�� �� �����ϴ�.");
            return;
        }

        // �÷��̾� ã��
        FindLocalPlayer();
        if (GameSettings.IsMultiplayer == false)
        {
            InitializeChunksAtZero();
        }
        else 
        {
            if (PhotonNetwork.IsMasterClient) { InitializeChunksAtZero(); }
        }
        
            
  
    }

    void Update()
    {
        if (player == null)
        {
            FindLocalPlayer();
            return;
        }

        Vector3 playerPosition = player.transform.position;
        Vector2Int playerChunkCoord = GetChunkCoordFromPosition(playerPosition);

        if (playerChunkCoord != currentPlayerChunk)
        {
            Debug.Log($"�÷��̾ ûũ {currentPlayerChunk}���� {playerChunkCoord}�� �̵��߽��ϴ�.");
            currentPlayerChunk = playerChunkCoord;
            UpdateChunksAroundPlayer();
        }
    }

    private void FindLocalPlayer()
    {

        // "Player" �±׸� ���� ���� �÷��̾� ã��
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var p in players)
        {
            PhotonView pv = p.GetComponent<PhotonView>();
            if (GameSettings.IsMultiplayer == true)
            {
                if (pv != null && pv.IsMine)
                {
                    player = p;
                    currentPlayerChunk = GetChunkCoordFromPosition(player.transform.position);
                    Debug.Log($"���� �÷��̾ ã�ҽ��ϴ�: {player.name}");
                    break;
                }
            }
            else
            {
                player = p;
            }
        }
    }

    private void InitializeChunksAtZero()
    {
        Vector2Int centerChunk = new Vector2Int(0, 0); // �ʱ� ûũ �߽��� (0,0)���� ����
        currentPlayerChunk = centerChunk;

        Debug.Log($"�ʱ� ûũ�� {centerChunk}�� �߽����� �����մϴ�.");

        // �߽� ûũ�� �������� nxn ���� ����
        for (int x = -gridSize / 2; x <= gridSize / 2; x++)
        {
            for (int y = -gridSize / 2; y <= gridSize / 2; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(centerChunk.x + x, centerChunk.y + y);
                if (!existingChunks.ContainsKey(chunkCoord))
                {
                    bool isInitial = (chunkCoord == centerChunk);
                    Debug.Log($"ûũ {chunkCoord} ���� �õ� ��. �ʱ� ûũ ����: {isInitial}");
                    GenerateAndSendChunk(chunkCoord, isInitial);
                }
            }
        }
    }

    private void UpdateChunksAroundPlayer()
    {
        Vector2Int playerChunkCoord = currentPlayerChunk;

        Debug.Log($"�÷��̾� �ֺ� ûũ�� {playerChunkCoord}�� �߽����� �����մϴ�.");

        // �÷��̾� �ֺ��� ûũ�� ����
        for (int x = -gridSize / 2; x <= gridSize / 2; x++)
        {
            for (int y = -gridSize / 2; y <= gridSize / 2; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(playerChunkCoord.x + x, playerChunkCoord.y + y);
                if (!existingChunks.ContainsKey(chunkCoord))
                {
                    bool isInitial = false; // �̵� �� ������ ûũ�� �ʱ� ûũ�� �ƴ�
                    Debug.Log($"ûũ {chunkCoord} ���� �õ� ��. �ʱ� ûũ ����: {isInitial}");
                    GenerateAndSendChunk(chunkCoord, isInitial);
                }
            }
        }

    }

    private void GenerateAndSendChunk(Vector2Int chunkCoord, bool isInitial)
    {
        if(GameSettings.IsMultiplayer == false)
        {
            Chunk newChunk = new Chunk(chunkCoord, chunkSize);
            WaveFunctionCollapse wfc = new WaveFunctionCollapse(biomeManager, cellSize);
            wfc.GenerateChunk(newChunk, chunkCoord, tileLoader.LoadedTiles, isPlayerSpawnChunk: isInitial);
            InstantiateChunk(newChunk);
            existingChunks[chunkCoord] = newChunk;
            return;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"������ Ŭ���̾�Ʈ�� ûũ {chunkCoord}�� �����մϴ�.");

            // ������ Ŭ���̾�Ʈ�� ûũ ����
            Chunk newChunk = new Chunk(chunkCoord, chunkSize);
            WaveFunctionCollapse wfc = new WaveFunctionCollapse(biomeManager, cellSize);
            wfc.GenerateChunk(newChunk, chunkCoord, tileLoader.LoadedTiles, isPlayerSpawnChunk: isInitial);

            // ûũ ������ ����ȭ
            string serializedChunk = SerializeChunk(newChunk);
            Debug.Log($"ûũ {chunkCoord} ����ȭ �Ϸ�: {serializedChunk}");

            // ��� Ŭ���̾�Ʈ���� ûũ ������ ����
            photonView.RPC("ReceiveChunkData", RpcTarget.Others, chunkCoord.x, chunkCoord.y, serializedChunk);
            Debug.Log($"ûũ {chunkCoord} �����͸� ��� Ŭ���̾�Ʈ���� �����߽��ϴ�.");

            // ������ Ŭ���̾�Ʈ �ڽŵ� ûũ�� �ν��Ͻ�ȭ
            InstantiateChunk(newChunk);
            existingChunks[chunkCoord] = newChunk;

            Debug.Log($"ûũ {chunkCoord} ���� �� �ν��Ͻ�ȭ �Ϸ�.");
        }
        else
        {
            // ������ Ŭ���̾�Ʈ���� ûũ ���� ��û
            Debug.Log($"Ŭ���̾�Ʈ�� �����Ϳ��� ûũ {chunkCoord} ������ ��û�մϴ�.");
            photonView.RPC("RequestChunkData", RpcTarget.MasterClient, chunkCoord.x, chunkCoord.y);
        }
    }




    private string SerializeChunk(Chunk chunk)
    {
        ChunkData chunkData = new ChunkData
        {
            chunkCoord = chunk.chunkCoord,
            cells = new List<CellData>()
        };

        for (int x = 0; x < chunk.width; x++)
        {
            for (int y = 0; y < chunk.height; y++)
            {
                Cell cell = chunk.cells[x, y];
                if (cell.collapsedTileState != null)
                {
                    CellData cellData = new CellData
                    {
                        x = x,
                        y = y,
                        tileName = cell.collapsedTileState.tile.tileName,
                        rotationIndex = cell.collapsedTileState.rotationIndex
                    };
                    chunkData.cells.Add(cellData);
                }
            }
        }

        return JsonUtility.ToJson(chunkData);
    }

    [PunRPC]
    private void ReceiveChunkData(int chunkX, int chunkY, string serializedChunk)
    {
        Vector2Int chunkCoord = new Vector2Int(chunkX, chunkY);
        Debug.Log($"ûũ ������ ����: {chunkCoord}, ������: {serializedChunk}");

        if (existingChunks.ContainsKey(chunkCoord))
        {
            Debug.Log($"ûũ {chunkCoord}�� �̹� �����մϴ�.");
            return;
        }

        // ûũ ������ ������ȭ
        ChunkData chunkData = JsonUtility.FromJson<ChunkData>(serializedChunk);
        Chunk newChunk = DeserializeChunk(chunkData);

        // ûũ�� �ν��Ͻ�ȭ
        InstantiateChunk(newChunk);
        existingChunks[chunkCoord] = newChunk;

        Debug.Log($"ûũ {chunkCoord}�� Ŭ���̾�Ʈ���� �����Ǿ����ϴ�.");
    }

    private Chunk DeserializeChunk(ChunkData chunkData)
    {
        Chunk chunk = new Chunk(chunkData.chunkCoord, chunkSize);
        Debug.Log($"DeserializeChunk: Creating chunk at {chunk.chunkCoord} with position {chunk.chunkObject.transform.position}");

        foreach (var cellData in chunkData.cells)
        {
            if (string.IsNullOrEmpty(cellData.tileName))
            {
                Debug.LogError($"ûũ {chunkData.chunkCoord}�� �� ({cellData.x}, {cellData.y})�� tileName�� ����ֽ��ϴ�.");
                continue;
            }

            if (!tileLoader.tiles.ContainsKey(cellData.tileName))
            {
                Debug.LogError($"Tile '{cellData.tileName}'�� TileLoader�� �������� �ʽ��ϴ�.");
                continue;
            }

            Tile tile = tileLoader.tiles[cellData.tileName];
            if (tile == null)
            {
                Debug.LogError($"TileLoader.tiles[{cellData.tileName}]�� null�Դϴ�.");
                continue;
            }

            // rotationIndex ��ȿ�� �˻�
            if (tile.rotationAngles == null || tile.rotationAngles.Count <= cellData.rotationIndex)
            {
                Debug.LogError($"Tile '{tile.tileName}'�� rotationIndex {cellData.rotationIndex}�� ��ȿ���� �ʽ��ϴ�.");
                continue;
            }

            // x�� y �ε����� ��ȿ���� Ȯ��
            if (cellData.x < 0 || cellData.x >= chunk.width || cellData.y < 0 || cellData.y >= chunk.height)
            {
                Debug.LogError($"DeserializeChunk: Cell coordinates ({cellData.x}, {cellData.y}) are out of bounds for chunk size {chunk.width}x{chunk.height}");
                continue;
            }

            // Cell ��ü ���� �� �Ҵ�
            Cell cell = new Cell
            {
                position = new Vector2Int(cellData.x, cellData.y),
                collapsedTileState = new TileState
                {
                    tile = tile,
                    rotationIndex = cellData.rotationIndex
                },
                possibleTileStates = new List<TileState>
            {
                new TileState
                {
                    tile = tile,
                    rotationIndex = cellData.rotationIndex
                }
            }
            };
            chunk.cells[cellData.x, cellData.y] = cell;

            Debug.Log($"DeserializeChunk: Assigned tile '{tile.tileName}' to chunk {chunk.chunkCoord} cell ({cellData.x}, {cellData.y})");
        }

        Debug.Log($"DeserializeChunk: Completed chunk {chunk.chunkCoord} at position {chunk.chunkObject.transform.position}");
        return chunk;
    }



    private void InstantiateChunk(Chunk chunk)
    {
        Debug.Log($"ûũ {chunk.chunkCoord}�� �ν��Ͻ�ȭ�մϴ�.");

        WaveFunctionCollapse wfc = new WaveFunctionCollapse(biomeManager, cellSize);
        wfc.InstantiateChunk(chunk);

        Debug.Log($"ûũ {chunk.chunkCoord} �ν��Ͻ�ȭ �Ϸ�.");
    }

    private Vector2Int GetChunkCoordFromPosition(Vector3 position)
    {
        float chunkWorldSize = chunkSize * cellSize;
        int chunkX = Mathf.FloorToInt(position.x / chunkWorldSize);
        int chunkY = Mathf.FloorToInt(position.z / chunkWorldSize);
        return new Vector2Int(chunkX, chunkY);
    }

    [PunRPC]
    private void RequestChunkData(int chunkX, int chunkY)
    {
        Vector2Int chunkCoord = new Vector2Int(chunkX, chunkY);
        Debug.Log($"ûũ ������ ��û ����: {chunkCoord}");

        if (PhotonNetwork.IsMasterClient && !existingChunks.ContainsKey(chunkCoord))
        {
            // ������ Ŭ���̾�Ʈ�� ��û���� ûũ�� �����ϰ� ����
            GenerateAndSendChunk(chunkCoord, isInitial: false);
        }
    }

    // Ŭ���̾�Ʈ���� ���ο� ûũ�� �̵��� �� ȣ��
    public void OnPlayerMovedToChunk(Vector2Int newChunkCoord)
    {
        currentPlayerChunk = newChunkCoord;
        Debug.Log($"�÷��̾ ���ο� ûũ {newChunkCoord}�� �̵��߽��ϴ�.");

        // �ֺ� ûũ ��û
        for (int x = -gridSize / 2; x <= gridSize / 2; x++)
        {
            for (int y = -gridSize / 2; y <= gridSize / 2; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(newChunkCoord.x + x, newChunkCoord.y + y);
                if (!existingChunks.ContainsKey(chunkCoord))
                {
                    Debug.Log($"ûũ {chunkCoord}�� �������� �����Ƿ� ûũ ������ ��û ��.");
                    // ������ Ŭ���̾�Ʈ���� ûũ ������ ��û
                    photonView.RPC("RequestChunkData", RpcTarget.MasterClient, chunkCoord.x, chunkCoord.y);
                }
            }
        }

    }


    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("���ο� ������ Ŭ���̾�Ʈ�� �Ǿ����ϴ�. ��� Ŭ���̾�Ʈ���� ���� ûũ �����͸� �����ؾ� �մϴ�.");

            // ��� ���� ûũ �����͸� ��� Ŭ���̾�Ʈ���� ������
            foreach (var kvp in existingChunks)
            {
                Vector2Int chunkCoord = kvp.Key;
                Chunk chunk = kvp.Value;

                // ûũ ������ ����ȭ
                string serializedChunk = SerializeChunk(chunk);
                Debug.Log($"������ Ŭ���̾�Ʈ�� ûũ {chunkCoord} �����͸� ����ȭ�Ͽ� �������մϴ�.");

                // ��� Ŭ���̾�Ʈ���� ûũ ������ ����
                photonView.RPC("ReceiveChunkData", RpcTarget.Others, chunkCoord.x, chunkCoord.y, serializedChunk);
            }
        }
    }
}




// �߰����� ������ Ŭ������ �����մϴ�.

[System.Serializable]
public class ChunkData
{
    public Vector2Int chunkCoord;
    public List<CellData> cells;
}

[System.Serializable]
public class CellData
{
    public int x;
    public int y;
    public string tileName;
    public int rotationIndex;
}
