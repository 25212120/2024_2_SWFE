using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TileLoader : MonoBehaviour
{
    [Header("Tile Data JSON")]
    public TextAsset tileDataJson;

    public Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();
    void Awake()
    {
        LoadTilesFromJson();
    }
    void Start()
    {
        if (tileDataJson == null)
        {
            Debug.LogError("Tile Data JSON이 설정되지 않았습니다!");
            return;
        }

        LoadTilesFromJson();
    }

    void LoadTilesFromJson()
    {
        try
        {
            TileDataList tileDataList = JsonUtility.FromJson<TileDataList>(tileDataJson.text);

            foreach (var data in tileDataList.tiles)
            {
                var socketDict = new Dictionary<string, List<string>>();
                foreach (var socket in data.sockets)
                {
                    socketDict[socket.direction] = socket.values;
                }

                Tile tile = new Tile
                {
                    tileName = data.tileName,
                    prefab = Resources.Load<GameObject>($"Tiles/{data.tileName}"),
                    canRotate = data.canRotate,
                    sockets = socketDict,
                    biomes = data.biomes.Select(b => b.Trim()).ToList(),
                    weight = data.weight > 0 ? data.weight : 1.0f,
                    isEdgeTile = data.isEdgeTile
                };

                if (tile.biomes.Count == 0)
                {
                    Debug.LogWarning($"타일 {tile.tileName}의 바이옴 리스트가 비어있습니다.");
                }
                else
                {
                    foreach (var biome in tile.biomes)
                    {
                        Debug.Log($"타일 {tile.tileName}의 바이옴: '{biome}' (길이: {biome.Length})");
                        DebugBiomeString(biome, $"타일 {tile.tileName}");
                    }
                }

                tile.GenerateRotatedSockets();
                tiles.Add(data.tileName, tile);
            }

            Debug.Log($"{tiles.Count}개의 타일이 로드되었습니다!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON 로드 중 오류 발생: {e.Message}");
        }
    }

    private void DebugBiomeString(string biome, string context)
    {
        var charCodes = biome.Select(c => ((int)c).ToString()).ToArray();
        string codes = string.Join(", ", charCodes);
        Debug.Log($"{context} 바이옴 문자열 유니코드 값: [{codes}]");
    }


}

[System.Serializable]
public class TileData
{
    public string tileName;
    public bool canRotate;
    public List<SocketData> sockets; // 수정된 소켓 구조
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
