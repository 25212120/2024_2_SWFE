using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public string tileName; // 타일 이름
    public GameObject prefab; // Unity 프리팹 참조
    public bool canRotate; // 회전 가능 여부
    public Dictionary<string, List<string>> sockets = new Dictionary<string, List<string>>();
    public List<string> biomes = new List<string>();
    public float weight = 1.0f; // 가중치
    public bool isEdgeTile = false; // 가장자리 타일 여부

    public List<Dictionary<string, List<string>>> rotatedSockets = new List<Dictionary<string, List<string>>>();
    public List<int> rotationAngles = new List<int>();


    public void GenerateRotatedSockets()
    {
        if (sockets == null)
        {
            Debug.LogError($"Tile {tileName} has null sockets. Skipping rotated sockets generation.");
            return;
        }

        rotationAngles.Clear();
        rotatedSockets.Clear();

        int rotationSteps = canRotate ? 4 : 1;
        for (int i = 0; i < rotationSteps; i++)
        {
            int angle = i * 90;
            rotationAngles.Add(angle);

            // 소켓 회전 처리
            Dictionary<string, List<string>> rotatedSocket = RotateSockets(sockets, angle);
            if (rotatedSocket != null)
            {
                rotatedSockets.Add(rotatedSocket);
            }
            else
            {
                Debug.LogError($"Tile {tileName}: Failed to generate rotated sockets for angle {angle}.");
            }
        }

        Debug.Log($"Tile {tileName} rotated sockets generated successfully. Total rotations: {rotatedSockets.Count}");
    }

    private Dictionary<string, List<string>> RotateSockets(Dictionary<string, List<string>> originalSockets, int angle)
    {
        if (originalSockets == null)
        {
            Debug.LogWarning("Original sockets are null. Returning empty dictionary.");
            return new Dictionary<string, List<string>>();
        }

        var rotated = new Dictionary<string, List<string>>();
        string[] directions = { "north", "east", "south", "west" };
        int steps = angle / 90;

        for (int i = 0; i < directions.Length; i++)
        {
            int rotatedIndex = (i + steps) % directions.Length;
            string originalDir = directions[i];
            string rotatedDir = directions[rotatedIndex];

            // Null-safe 소켓 값 처리
            if (originalSockets.ContainsKey(originalDir))
            {
                rotated[rotatedDir] = originalSockets[originalDir] ?? new List<string>();
            }
            else
            {
                Debug.LogWarning($"Missing direction '{originalDir}' in sockets.");
                rotated[rotatedDir] = new List<string>();
            }
        }

        return rotated;
    }
}
