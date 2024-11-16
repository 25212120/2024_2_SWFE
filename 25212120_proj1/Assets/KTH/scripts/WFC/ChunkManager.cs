using UnityEngine;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour
{
    public int chunkWidth = 5; // 청크의 너비
    public int chunkHeight = 5; // 청크의 높이
    public Vector2 chunkPosition = Vector2.zero; // 청크의 시작 위치

    public TileLoader tileLoader; // 타일 로더
    public BiomeManager biomeManager; // 바이옴 관리자

    private WaveFunctionCollapse wfc; // WFC 알고리즘 인스턴스

    void Start()
    {
        // TileLoader와 BiomeManager가 설정되었는지 확인
        if (tileLoader == null || biomeManager == null)
        {
            Debug.LogError("TileLoader 또는 BiomeManager가 설정되지 않았습니다!");
            return;
        }

        // WaveFunctionCollapse 인스턴스 생성
        wfc = new WaveFunctionCollapse(biomeManager);

        // 모든 타일 데이터 로드
        List<Tile> allTiles = new List<Tile>(tileLoader.tiles.Values);

        // 청크 생성
        GenerateChunk(chunkWidth, chunkHeight, chunkPosition, allTiles);
    }

    void GenerateChunk(int width, int height, Vector2 position, List<Tile> allTiles)
    {
        // 청크 생성
        Chunk chunk = new Chunk(width, height);

        // 청크의 GameObject를 이 스크립트가 붙은 오브젝트의 자식으로 설정
        chunk.chunkObject.transform.parent = this.transform;
        chunk.chunkObject.transform.position = new Vector3(position.x, 0, position.y);

        // WFC 알고리즘 실행
        wfc.GenerateChunk(chunk, position, allTiles);

        // 청크 데이터가 생성된 후 필요에 따라 추가 작업 가능
        Debug.Log("청크 생성 완료!");
    }
}
