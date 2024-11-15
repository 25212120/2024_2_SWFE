using UnityEngine;
using System.Collections.Generic;

public class TileLoader : MonoBehaviour
{
    [Header("Tile Data JSON")]
    public TextAsset tileDataJson;

    public Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();

    void Start()
    {
        if (tileDataJson == null)
        {
            Debug.LogError("Tile Data JSON�� �������� �ʾҽ��ϴ�!");
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
                    biomes = data.biomes,
                    weight = data.weight > 0 ? data.weight : 1.0f,
                    isEdgeTile = data.isEdgeTile // JSON���� isEdgeTile �б�
                };

                tile.GenerateRotatedSockets();
                tiles.Add(data.tileName, tile);
            }

            Debug.Log($"{tiles.Count}���� Ÿ���� �ε�Ǿ����ϴ�!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON �ε� �� ���� �߻�: {e.Message}");
        }
    }
}

[System.Serializable]
public class TileData
{
    public string tileName;
    public bool canRotate;
    public List<SocketData> sockets; // ������ ���� ����
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
