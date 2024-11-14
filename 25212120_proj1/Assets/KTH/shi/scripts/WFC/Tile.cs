using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ÿ���� �����ϴ� Ŭ����
/// �� Ÿ���� �̸��� ���� ���̿��� �����ϴ�.
/// ȸ���� ����Ͽ� TileVariant�� ����մϴ�.
[System.Serializable]
public class Tile
{
    /// Ÿ���� �̸�
    public string Name;
    /// Ÿ���� ���� ���̿� ����
    public BiomeType Biome;

    /// Ÿ���� ��� ȸ�� ����
    public List<TileVariant> Variants;

    /// Ÿ���� ������
    public Tile(string name, BiomeType biome)
    {
        Name = name;
        Biome = biome;
        Variants = new List<TileVariant>();
    }

    /// Ư�� ���⿡ ���Ǵ� �̿� Ÿ�� ����� �߰��ϴ� �޼���
    public void AddNeighbor(Direction direction, List<string> possibleTiles, Rotation rotation)
    {
        // �� ȸ�� ���¿� ���� �̿� Ÿ���� ����
        foreach (var variant in Variants)
        {
            if (variant.Rotation == rotation)
            {
                variant.Neighbors[direction] = possibleTiles;
            }
        }
    }
}
