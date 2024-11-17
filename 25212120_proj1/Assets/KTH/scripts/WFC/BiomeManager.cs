using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    public float scale = 20f;

    public float GetNoiseValue(Vector2 position)
    {
        // ��ǥ�� ����� ��ȯ�ϱ� ���� ������ ����
        float offset = 10000f;
        float x = (position.x + offset) / scale;
        float y = (position.y + offset) / scale;

        return Mathf.PerlinNoise(x, y);
    }

    public string GetBiomeForPosition(Vector2 position)
    {
        float noiseValue = GetNoiseValue(position);

        if (noiseValue < 0.3f)
            return "Forest";
        else if (noiseValue < 0.5f)
            return "Volcano";
        else if (noiseValue < 0.66f)
            return "Snow";
        else
            return "Sand";
    }
}