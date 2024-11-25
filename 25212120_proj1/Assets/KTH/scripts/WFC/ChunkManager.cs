using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class ChunkManager : MonoBehaviourPunCallbacks
{
    public int chunkSize = 10; // 청크 크기 (셀 단위)
    public int gridSize = 5;    // nxn 범위의 청크 관리 (홀수로 설정)
    public static float cellSize = 50f; // 타일의 크기

    private Dictionary<Vector2Int, Chunk> existingChunks = new Dictionary<Vector2Int, Chunk>(); // 이미 생성된 청크
    private Vector2Int currentPlayerChunk; // 현재 플레이어가 위치한 청크 좌표

    // 필요한 컴포넌트 참조
    private BiomeManager biomeManager;
    private TileLoader tileLoader;
    private GameObject player; // 실제 플레이어 오브젝트

    void Start()
    {
        // 필요한 컴포넌트 가져오기
        biomeManager = FindObjectOfType<BiomeManager>();
        tileLoader = FindObjectOfType<TileLoader>();

        if (biomeManager == null || tileLoader == null)
        {
            Debug.LogError("BiomeManager 또는 TileLoader를 찾을 수 없습니다.");
            return;
        }

        // 플레이어 찾기
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
            Debug.Log($"플레이어가 청크 {currentPlayerChunk}에서 {playerChunkCoord}로 이동했습니다.");
            currentPlayerChunk = playerChunkCoord;
            UpdateChunksAroundPlayer();
        }
    }

    private void FindLocalPlayer()
    {

        // "Player" 태그를 가진 로컬 플레이어 찾기
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
                    Debug.Log($"로컬 플레이어를 찾았습니다: {player.name}");
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
        Vector2Int centerChunk = new Vector2Int(0, 0); // 초기 청크 중심을 (0,0)으로 설정
        currentPlayerChunk = centerChunk;

        Debug.Log($"초기 청크를 {centerChunk}를 중심으로 생성합니다.");

        // 중심 청크를 기준으로 nxn 범위 생성
        for (int x = -gridSize / 2; x <= gridSize / 2; x++)
        {
            for (int y = -gridSize / 2; y <= gridSize / 2; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(centerChunk.x + x, centerChunk.y + y);
                if (!existingChunks.ContainsKey(chunkCoord))
                {
                    bool isInitial = (chunkCoord == centerChunk);
                    Debug.Log($"청크 {chunkCoord} 생성 시도 중. 초기 청크 여부: {isInitial}");
                    GenerateAndSendChunk(chunkCoord, isInitial);
                }
            }
        }
    }

    private void UpdateChunksAroundPlayer()
    {
        Vector2Int playerChunkCoord = currentPlayerChunk;

        Debug.Log($"플레이어 주변 청크를 {playerChunkCoord}를 중심으로 생성합니다.");

        // 플레이어 주변의 청크를 생성
        for (int x = -gridSize / 2; x <= gridSize / 2; x++)
        {
            for (int y = -gridSize / 2; y <= gridSize / 2; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(playerChunkCoord.x + x, playerChunkCoord.y + y);
                if (!existingChunks.ContainsKey(chunkCoord))
                {
                    bool isInitial = false; // 이동 후 생성된 청크는 초기 청크가 아님
                    Debug.Log($"청크 {chunkCoord} 생성 시도 중. 초기 청크 여부: {isInitial}");
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
            Debug.Log($"마스터 클라이언트가 청크 {chunkCoord}를 생성합니다.");

            // 마스터 클라이언트만 청크 생성
            Chunk newChunk = new Chunk(chunkCoord, chunkSize);
            WaveFunctionCollapse wfc = new WaveFunctionCollapse(biomeManager, cellSize);
            wfc.GenerateChunk(newChunk, chunkCoord, tileLoader.LoadedTiles, isPlayerSpawnChunk: isInitial);

            // 청크 데이터 직렬화
            string serializedChunk = SerializeChunk(newChunk);
            Debug.Log($"청크 {chunkCoord} 직렬화 완료: {serializedChunk}");

            // 모든 클라이언트에게 청크 데이터 전송
            photonView.RPC("ReceiveChunkData", RpcTarget.Others, chunkCoord.x, chunkCoord.y, serializedChunk);
            Debug.Log($"청크 {chunkCoord} 데이터를 모든 클라이언트에게 전송했습니다.");

            // 마스터 클라이언트 자신도 청크를 인스턴스화
            InstantiateChunk(newChunk);
            existingChunks[chunkCoord] = newChunk;

            Debug.Log($"청크 {chunkCoord} 생성 및 인스턴스화 완료.");
        }
        else
        {
            // 마스터 클라이언트에게 청크 생성 요청
            Debug.Log($"클라이언트가 마스터에게 청크 {chunkCoord} 생성을 요청합니다.");
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
        Debug.Log($"청크 데이터 수신: {chunkCoord}, 데이터: {serializedChunk}");

        if (existingChunks.ContainsKey(chunkCoord))
        {
            Debug.Log($"청크 {chunkCoord}는 이미 존재합니다.");
            return;
        }

        // 청크 데이터 역직렬화
        ChunkData chunkData = JsonUtility.FromJson<ChunkData>(serializedChunk);
        Chunk newChunk = DeserializeChunk(chunkData);

        // 청크를 인스턴스화
        InstantiateChunk(newChunk);
        existingChunks[chunkCoord] = newChunk;

        Debug.Log($"청크 {chunkCoord}가 클라이언트에서 생성되었습니다.");
    }

    private Chunk DeserializeChunk(ChunkData chunkData)
    {
        Chunk chunk = new Chunk(chunkData.chunkCoord, chunkSize);
        Debug.Log($"DeserializeChunk: Creating chunk at {chunk.chunkCoord} with position {chunk.chunkObject.transform.position}");

        foreach (var cellData in chunkData.cells)
        {
            if (string.IsNullOrEmpty(cellData.tileName))
            {
                Debug.LogError($"청크 {chunkData.chunkCoord}의 셀 ({cellData.x}, {cellData.y})에 tileName이 비어있습니다.");
                continue;
            }

            if (!tileLoader.tiles.ContainsKey(cellData.tileName))
            {
                Debug.LogError($"Tile '{cellData.tileName}'이 TileLoader에 존재하지 않습니다.");
                continue;
            }

            Tile tile = tileLoader.tiles[cellData.tileName];
            if (tile == null)
            {
                Debug.LogError($"TileLoader.tiles[{cellData.tileName}]이 null입니다.");
                continue;
            }

            // rotationIndex 유효성 검사
            if (tile.rotationAngles == null || tile.rotationAngles.Count <= cellData.rotationIndex)
            {
                Debug.LogError($"Tile '{tile.tileName}'의 rotationIndex {cellData.rotationIndex}이 유효하지 않습니다.");
                continue;
            }

            // x와 y 인덱스가 유효한지 확인
            if (cellData.x < 0 || cellData.x >= chunk.width || cellData.y < 0 || cellData.y >= chunk.height)
            {
                Debug.LogError($"DeserializeChunk: Cell coordinates ({cellData.x}, {cellData.y}) are out of bounds for chunk size {chunk.width}x{chunk.height}");
                continue;
            }

            // Cell 객체 생성 및 할당
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
        Debug.Log($"청크 {chunk.chunkCoord}를 인스턴스화합니다.");

        WaveFunctionCollapse wfc = new WaveFunctionCollapse(biomeManager, cellSize);
        wfc.InstantiateChunk(chunk);

        Debug.Log($"청크 {chunk.chunkCoord} 인스턴스화 완료.");
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
        Debug.Log($"청크 데이터 요청 수신: {chunkCoord}");

        if (PhotonNetwork.IsMasterClient && !existingChunks.ContainsKey(chunkCoord))
        {
            // 마스터 클라이언트가 요청받은 청크를 생성하고 전송
            GenerateAndSendChunk(chunkCoord, isInitial: false);
        }
    }

    // 클라이언트에서 새로운 청크로 이동할 때 호출
    public void OnPlayerMovedToChunk(Vector2Int newChunkCoord)
    {
        currentPlayerChunk = newChunkCoord;
        Debug.Log($"플레이어가 새로운 청크 {newChunkCoord}로 이동했습니다.");

        // 주변 청크 요청
        for (int x = -gridSize / 2; x <= gridSize / 2; x++)
        {
            for (int y = -gridSize / 2; y <= gridSize / 2; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(newChunkCoord.x + x, newChunkCoord.y + y);
                if (!existingChunks.ContainsKey(chunkCoord))
                {
                    Debug.Log($"청크 {chunkCoord}가 존재하지 않으므로 청크 데이터 요청 중.");
                    // 마스터 클라이언트에게 청크 데이터 요청
                    photonView.RPC("RequestChunkData", RpcTarget.MasterClient, chunkCoord.x, chunkCoord.y);
                }
            }
        }

    }


    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("새로운 마스터 클라이언트가 되었습니다. 모든 클라이언트에게 기존 청크 데이터를 전송해야 합니다.");

            // 모든 기존 청크 데이터를 모든 클라이언트에게 재전송
            foreach (var kvp in existingChunks)
            {
                Vector2Int chunkCoord = kvp.Key;
                Chunk chunk = kvp.Value;

                // 청크 데이터 직렬화
                string serializedChunk = SerializeChunk(chunk);
                Debug.Log($"마스터 클라이언트가 청크 {chunkCoord} 데이터를 직렬화하여 재전송합니다.");

                // 모든 클라이언트에게 청크 데이터 전송
                photonView.RPC("ReceiveChunkData", RpcTarget.Others, chunkCoord.x, chunkCoord.y, serializedChunk);
            }
        }
    }
}




// 추가적인 데이터 클래스를 정의합니다.

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
