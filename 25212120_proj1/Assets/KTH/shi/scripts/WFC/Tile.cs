using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 타일을 정의하는 클래스
/// 각 타일은 이름과 속한 바이옴을 가집니다.
/// 회전을 고려하여 TileVariant를 사용합니다.
[System.Serializable]
public class Tile
{
    /// 타일의 이름
    public string Name;
    /// 타일이 속한 바이옴 유형
    public BiomeType Biome;

    /// 타일의 모든 회전 버전
    public List<TileVariant> Variants;

    /// 타일의 생성자
    public Tile(string name, BiomeType biome)
    {
        Name = name;
        Biome = biome;
        Variants = new List<TileVariant>();
    }

    /// 특정 방향에 허용되는 이웃 타일 목록을 추가하는 메서드
    public void AddNeighbor(Direction direction, List<string> possibleTiles, Rotation rotation)
    {
        // 각 회전 상태에 따라 이웃 타일을 설정
        foreach (var variant in Variants)
        {
            if (variant.Rotation == rotation)
            {
                variant.Neighbors[direction] = possibleTiles;
            }
        }
    }
}
