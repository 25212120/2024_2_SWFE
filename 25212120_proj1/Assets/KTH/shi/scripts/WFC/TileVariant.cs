using System.Collections.Generic;
using UnityEngine;

/// 특정 타일의 회전 상태를 나타내는 클래스
[System.Serializable]
public class TileVariant
{
    /// 타일의 회전 상태
    public Rotation Rotation;

    /// 각 방향에 따른 허용되는 이웃 타일의 이름 목록
    public Dictionary<Direction, List<string>> Neighbors;

    /// TileVariant의 생성자
    public TileVariant(Rotation rotation)
    {
        Rotation = rotation;
        Neighbors = new Dictionary<Direction, List<string>>();
    }
}
