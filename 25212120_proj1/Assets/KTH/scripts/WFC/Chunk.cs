using UnityEngine;

public class Chunk
{
    public int width;
    public int height;
    public Cell[,] cells;
    public GameObject chunkObject; // 추가된 부분

    public Chunk(int width, int height)
    {
        this.width = width;
        this.height = height;
        cells = new Cell[width, height];
        chunkObject = new GameObject("Chunk"); // 청크를 표현하는 GameObject 생성
    }
}
