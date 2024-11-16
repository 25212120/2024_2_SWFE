using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TileLoader : MonoBehaviour
{
    [Header("Tile Data JSON")]
    public TextAsset tileDataJson;

    // 로드된 타일을 저장하는 딕셔너리
    public Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();

    // 로드된 타일 리스트를 제공하는 속성 추가
    public List<Tile> LoadedTiles { get; private set; }

    void Awake()
    {
        LoadTilesFromJson();
    }

    void LoadTilesFromJson()
    {
        try
        {
            // JSON 파일에서 타일 데이터 로드
            TileDataList tileDataList = JsonUtility.FromJson<TileDataList>(tileDataJson.text);

            foreach (var data in tileDataList.tiles)
            {
                // 소켓 데이터를 딕셔너리로 변환
                var socketDict = new Dictionary<string, List<string>>();
                foreach (var socket in data.sockets)
                {
                    socketDict[socket.direction] = socket.values;
                }

                // 타일 생성
                Tile tile = new Tile
                {
                    tileName = data.tileName,
                    prefab = Resources.Load<GameObject>($"Tiles/{data.tileName}"),
                    canRotate = data.canRotate,
                    sockets = socketDict,
                    biomes = data.biomes.Select(b => b.Trim().ToLowerInvariant()).ToList(), // 바이옴 이름을 소문자로 변환
                    weight = data.weight > 0 ? data.weight : 1.0f,
                    isEdgeTile = data.isEdgeTile
                };

                if (tile.prefab == null)
                {
                    Debug.LogError($"타일 프리팹을 찾을 수 없습니다: Tiles/{data.tileName}");
                    continue;
                }

                if (tile.biomes.Count == 0)
                {
                    Debug.LogWarning($"타일 {tile.tileName}의 바이옴 리스트가 비어있습니다.");
                }
                else
                {
                    foreach (var biome in tile.biomes)
                    {
                        Debug.Log($"타일 {tile.tileName}의 바이옴: '{biome}'");
                    }
                }

                // 회전된 소켓 생성
                tile.GenerateRotatedSockets();

                // 딕셔너리에 타일 추가
                tiles.Add(data.tileName, tile);
            }

            // LoadedTiles 리스트를 tiles 딕셔너리의 값으로 초기화
            LoadedTiles = tiles.Values.ToList();

            Debug.Log($"{tiles.Count}개의 타일이 로드되었습니다!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON 로드 중 오류 발생: {e.Message}");
        }
    }
}

// JSON 파일의 구조에 맞는 클래스들
[System.Serializable]
public class TileData
{
    public string tileName;
    public bool canRotate;
    public List<SocketData> sockets;
    public List<string> biomes;
    public float weight;
    public bool isEdgeTile;
}

[System.Serializable]
public class SocketData
{
    public string direction;
    public List<string> values;
}

[System.Serializable]
public class TileDataList
{
    public List<TileData> tiles;
}
