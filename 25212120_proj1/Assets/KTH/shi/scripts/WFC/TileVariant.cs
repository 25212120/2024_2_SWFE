using System.Collections.Generic;
using UnityEngine;

/// Ư�� Ÿ���� ȸ�� ���¸� ��Ÿ���� Ŭ����
[System.Serializable]
public class TileVariant
{
    /// Ÿ���� ȸ�� ����
    public Rotation Rotation;

    /// �� ���⿡ ���� ���Ǵ� �̿� Ÿ���� �̸� ���
    public Dictionary<Direction, List<string>> Neighbors;

    /// TileVariant�� ������
    public TileVariant(Rotation rotation)
    {
        Rotation = rotation;
        Neighbors = new Dictionary<Direction, List<string>>();
    }
}
