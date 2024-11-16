using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TileLoader : MonoBehaviour
{
    [Header("Tile Data JSON")]
    public TextAsset tileDataJson;

    // �ε�� Ÿ���� �����ϴ� ��ųʸ�
    public Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();

    // �ε�� Ÿ�� ����Ʈ�� �����ϴ� �Ӽ� �߰�
    public List<Tile> LoadedTiles { get; private set; }

    void Awake()
    {
        LoadTilesFromJson();
    }

    void LoadTilesFromJson()
    {
        try
        {
            // JSON ���Ͽ��� Ÿ�� ������ �ε�
            TileDataList tileDataList = JsonUtility.FromJson<TileDataList>(tileDataJson.text);

            foreach (var data in tileDataList.tiles)
            {
                // ���� �����͸� ��ųʸ��� ��ȯ
                var socketDict = new Dictionary<string, List<string>>();
                foreach (var socket in data.sockets)
                {
                    socketDict[socket.direction] = socket.values;
                }

                // Ÿ�� ����
                Tile tile = new Tile
                {
                    tileName = data.tileName,
                    prefab = Resources.Load<GameObject>($"Tiles/{data.tileName}"),
                    canRotate = data.canRotate,
                    sockets = socketDict,
                    biomes = data.biomes.Select(b => b.Trim().ToLowerInvariant()).ToList(), // ���̿� �̸��� �ҹ��ڷ� ��ȯ
                    weight = data.weight > 0 ? data.weight : 1.0f,
                    isEdgeTile = data.isEdgeTile
                };

                if (tile.prefab == null)
                {
                    Debug.LogError($"Ÿ�� �������� ã�� �� �����ϴ�: Tiles/{data.tileName}");
                    continue;
                }

                if (tile.biomes.Count == 0)
                {
                    Debug.LogWarning($"Ÿ�� {tile.tileName}�� ���̿� ����Ʈ�� ����ֽ��ϴ�.");
                }
                else
                {
                    foreach (var biome in tile.biomes)
                    {
                        Debug.Log($"Ÿ�� {tile.tileName}�� ���̿�: '{biome}'");
                    }
                }

                // ȸ���� ���� ����
                tile.GenerateRotatedSockets();

                // ��ųʸ��� Ÿ�� �߰�
                tiles.Add(data.tileName, tile);
            }

            // LoadedTiles ����Ʈ�� tiles ��ųʸ��� ������ �ʱ�ȭ
            LoadedTiles = tiles.Values.ToList();

            Debug.Log($"{tiles.Count}���� Ÿ���� �ε�Ǿ����ϴ�!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON �ε� �� ���� �߻�: {e.Message}");
        }
    }
}

// JSON ������ ������ �´� Ŭ������
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
