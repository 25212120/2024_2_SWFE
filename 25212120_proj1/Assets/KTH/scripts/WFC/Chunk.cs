using UnityEngine;

public class Chunk
{
    public GameObject chunkObject; // 청크를 나타내는 GameObject
    public Cell[,] cells;         // 청크 내부의 셀 배열
    public int width;             // 청크의 너비 (셀 단위)
    public int height;            // 청크의 높이 (셀 단위)
    public Vector2Int chunkCoord; // 청크의 위치 (청크 단위 좌표)

    // 생성자
    public Chunk(Vector2Int chunkCoord, int chunkSize)
    {
        this.chunkCoord = chunkCoord; // 청크 좌표 설정
        this.width = chunkSize;       // 청크의 너비
        this.height = chunkSize;      // 청크의 높이

        // 청크 GameObject 생성
        chunkObject = new GameObject($"Chunk_{chunkCoord.x}_{chunkCoord.y}");
    }
}
