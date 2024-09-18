using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFC3DGenerator : MonoBehaviour
{
    public GameObject[] tiles; // 배치할 타일 배열
    private int[,,] grid; // 3D 그리드를 위한 배열
    public int width = 10;
    public int height = 10;
    public int depth = 10;

    void Start()
    {
        grid = new int[width, height, depth]; // 10x10x10 그리드 생성
        Generate();
    }

    void Generate()
    {
        // 모든 그리드 셀을 -1로 초기화 (비어있는 상태)
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

        // 3D 공간에 타일을 배치
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

        // 인접한 타일을 확인하여 배치 가능한 타일 필터링 (규칙 적용)
        if (x > 0) // 왼쪽 타일 확인
        {
            availableTiles = FilterByNeighbor(availableTiles, grid[x - 1, y, z], "left");
        }
        if (y > 0) // 아래 타일 확인
        {
            availableTiles = FilterByNeighbor(availableTiles, grid[x, y - 1, z], "bottom");
        }
        if (z > 0) // 뒤쪽 타일 확인
        {
            availableTiles = FilterByNeighbor(availableTiles, grid[x, y, z - 1], "back");
        }

        // 가능한 타일 중 하나를 랜덤하게 선택
        int tileIndex = Random.Range(0, availableTiles.Count);
        GameObject selectedTile = availableTiles[tileIndex];

        // 타일을 월드에 배치 (3D 좌표 사용)
        Instantiate(selectedTile, new Vector3(x, y, z), Quaternion.identity);

        // 그리드에 타일 정보 저장
        grid[x, y, z] = tileIndex;
    }

    List<GameObject> FilterByNeighbor(List<GameObject> availableTiles, int neighborTileIndex, string direction)
    {
        // 규칙에 따라 인접 타일을 필터링하는 함수 (left, right, top, bottom, back, front에 따라 가능 여부 확인)
        // 이 부분에서 각 타일의 방향별 연결 가능 여부를 확인하는 로직을 추가해야 함
        return availableTiles;
    }
}
