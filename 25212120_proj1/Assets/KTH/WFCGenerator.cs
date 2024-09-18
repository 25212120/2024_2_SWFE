using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFC3DGenerator : MonoBehaviour
{
    public GameObject[] tiles; // ��ġ�� Ÿ�� �迭
    private int[,,] grid; // 3D �׸��带 ���� �迭
    public int width = 10;
    public int height = 10;
    public int depth = 10;

    void Start()
    {
        grid = new int[width, height, depth]; // 10x10x10 �׸��� ����
        Generate();
    }

    void Generate()
    {
        // ��� �׸��� ���� -1�� �ʱ�ȭ (����ִ� ����)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    grid[x, y, z] = -1;
                }
            }
        }

        // 3D ������ Ÿ���� ��ġ
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    PlaceTile(x, y, z);
                }
            }
        }
    }

    void PlaceTile(int x, int y, int z)
    {
        List<GameObject> availableTiles = new List<GameObject>(tiles);

        // ������ Ÿ���� Ȯ���Ͽ� ��ġ ������ Ÿ�� ���͸� (��Ģ ����)
        if (x > 0) // ���� Ÿ�� Ȯ��
        {
            availableTiles = FilterByNeighbor(availableTiles, grid[x - 1, y, z], "left");
        }
        if (y > 0) // �Ʒ� Ÿ�� Ȯ��
        {
            availableTiles = FilterByNeighbor(availableTiles, grid[x, y - 1, z], "bottom");
        }
        if (z > 0) // ���� Ÿ�� Ȯ��
        {
            availableTiles = FilterByNeighbor(availableTiles, grid[x, y, z - 1], "back");
        }

        // ������ Ÿ�� �� �ϳ��� �����ϰ� ����
        int tileIndex = Random.Range(0, availableTiles.Count);
        GameObject selectedTile = availableTiles[tileIndex];

        // Ÿ���� ���忡 ��ġ (3D ��ǥ ���)
        Instantiate(selectedTile, new Vector3(x, y, z), Quaternion.identity);

        // �׸��忡 Ÿ�� ���� ����
        grid[x, y, z] = tileIndex;
    }

    List<GameObject> FilterByNeighbor(List<GameObject> availableTiles, int neighborTileIndex, string direction)
    {
        // ��Ģ�� ���� ���� Ÿ���� ���͸��ϴ� �Լ� (left, right, top, bottom, back, front�� ���� ���� ���� Ȯ��)
        // �� �κп��� �� Ÿ���� ���⺰ ���� ���� ���θ� Ȯ���ϴ� ������ �߰��ؾ� ��
        return availableTiles;
    }
}
