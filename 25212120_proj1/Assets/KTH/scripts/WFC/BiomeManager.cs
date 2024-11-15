using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    public float scale = 20f;

    public string GetBiomeForPosition(Vector2 position)
    {
        float noiseValue = Mathf.PerlinNoise(position.x / scale, position.y / scale);

        if (noiseValue < 0.3f)
            return "Forest";
        else if (noiseValue < 0.6f)
            return "Forest";
        else
            return "Forest";
    }
}
