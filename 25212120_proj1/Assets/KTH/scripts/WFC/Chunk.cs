using UnityEngine;
/*
public class Chunk
{
    public Vector2Int chunkCoord;
    public int width;
    public int height;
    public Cell[,] cells;
    public GameObject chunkObject;

    public Chunk(Vector2Int coord, int size)
    {
        chunkCoord = coord;
        width = size;
        height = size;
        cells = new Cell[width, height]; // cells 배열 초기화
        chunkObject = new GameObject($"Chunk_{chunkCoord.x}_{chunkCoord.y}");
        chunkObject.SetActive(true);
    }
}
*/

public class Chunk
{
    public Vector2Int chunkCoord;
    public int width;
    public int height;
    public Cell[,] cells;
    public GameObject chunkObject;

    public Chunk(Vector2Int coord, int size)
    {
        chunkCoord = coord;
        width = size;
        height = size;
        cells = new Cell[width, height];
        chunkObject = new GameObject($"Chunk_{chunkCoord.x}_{chunkCoord.y}");

        // 청크의 월드 위치 설정
        float chunkWorldSize = width * ChunkManager.cellSize;
        Vector2 chunkWorldPosition = new Vector2(
            chunkCoord.x * chunkWorldSize,
            chunkCoord.y * chunkWorldSize
        );

        // 청크의 중앙 위치 사용 (옵션)
        chunkWorldPosition += new Vector2(chunkWorldSize / 2, chunkWorldSize / 2);

        chunkObject.transform.position = new Vector3(chunkWorldPosition.x, 0, chunkWorldPosition.y);
        Debug.Log($"Chunk 생성: {chunkCoord} 위치: {chunkObject.transform.position}");

        chunkObject.SetActive(true);
        chunkObject.layer = LayerMask.NameToLayer("Default"); // 적절한 레이어 설정
    }
}
