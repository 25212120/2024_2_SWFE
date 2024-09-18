using UnityEngine;

public class ProceduralTerrainGenerator : MonoBehaviour
{
    public int terrainWidth = 256;
    public int terrainLength = 256;
    public int terrainHeight = 50;
    public float noiseScale = 20f;

    public Terrain terrain;

    // 인스펙터에서 텍스처를 할당할 수 있도록 public 변수로 선언
    public Texture2D grassTexture;
    public Texture2D rockTexture;
    public Texture2D snowTexture;

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        TerrainData terrainData = terrain.terrainData;
        terrainData.heightmapResolution = terrainWidth + 1;

        terrainData.size = new Vector3(terrainWidth, terrainHeight, terrainLength);

        float[,] heights = GenerateHeights();
        terrainData.SetHeights(0, 0, heights);

        ApplyWFCRules(heights, terrainData);
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[terrainWidth, terrainLength];

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainLength; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / terrainWidth * noiseScale;
        float yCoord = (float)y / terrainLength * noiseScale;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return sample;
    }

    void ApplyWFCRules(float[,] heights, TerrainData terrainData)
    {
        // TerrainLayer를 사용하여 텍스처를 설정합니다.
        TerrainLayer[] terrainLayers = new TerrainLayer[3];

        // TerrainLayer 생성 및 텍스처 할당
        terrainLayers[0] = new TerrainLayer();
        terrainLayers[0].diffuseTexture = grassTexture;
        terrainLayers[0].tileSize = new Vector2(15, 15);

        terrainLayers[1] = new TerrainLayer();
        terrainLayers[1].diffuseTexture = rockTexture;
        terrainLayers[1].tileSize = new Vector2(15, 15);

        terrainLayers[2] = new TerrainLayer();
        terrainLayers[2].diffuseTexture = snowTexture;
        terrainLayers[2].tileSize = new Vector2(15, 15);

        terrainData.terrainLayers = terrainLayers;

        float[,,] alphamaps = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainLayers.Length];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float normX = x * 1.0f / (terrainData.alphamapWidth - 1);
                float normY = y * 1.0f / (terrainData.alphamapHeight - 1);

                float height = terrainData.GetInterpolatedHeight(normX, normY) / terrainHeight;

                float[] alphamapValues = new float[terrainLayers.Length];

                // 높이에 따른 텍스처 가중치 설정 (WFC 제약 조건 적용)
                if (height < 0.4f)
                {
                    alphamapValues[0] = 1f; // Grass
                }
                else if (height < 0.7f)
                {
                    alphamapValues[1] = 1f; // Rock
                }
                else
                {
                    alphamapValues[2] = 1f; // Snow
                }

                // 가중치 정규화
                float total = alphamapValues[0] + alphamapValues[1] + alphamapValues[2];
                for (int i = 0; i < terrainLayers.Length; i++)
                {
                    alphamapValues[i] /= total;
                    alphamaps[y, x, i] = alphamapValues[i];
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, alphamaps);
    }
}
