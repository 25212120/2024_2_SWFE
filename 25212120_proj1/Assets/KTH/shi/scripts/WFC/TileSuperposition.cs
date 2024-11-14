using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Ÿ�� ���� ������ Ÿ�ϰ� ȸ�� ���µ��� �����ϴ� Ŭ����
/// WFC �˰��򿡼� Ÿ�ϰ� ȸ�� ���¸� �����ϰ� ���� ������ �����ϴ� �� ���˴ϴ�.
/// </summary>
public class TileSuperposition
{
    /// <summary>
    /// ������ Ÿ�ϰ� ȸ�� ���µ��� ����Ʈ
    /// </summary>
    private List<TileVariant> possibleTileVariants;

    /// <summary>
    /// ���� ���� ��Ʈ���� (������ Ÿ�ϰ� ȸ�� ������ ����)
    /// </summary>
    public int Entropy => possibleTileVariants.Count;

    /// <summary>
    /// Ÿ�� ������������ ������
    /// </summary>
    /// <param name="tileSet">��ü Ÿ�� ��Ʈ</param>
    public TileSuperposition(List<Tile> tileSet)
    {
        possibleTileVariants = new List<TileVariant>();

        // ��� Ÿ���� ��� ȸ�� ���¸� ������ Ÿ�Ϸ� �߰�
        foreach (var tile in tileSet)
        {
            foreach (var variant in tile.Variants)
            {
                possibleTileVariants.Add(variant);
            }
        }
    }

    /// <summary>
    /// ���� �ϳ��� Ÿ�ϰ� ȸ�� ���·� �����ϴ� �޼���
    /// �������� �ϳ��� Ÿ�ϰ� ȸ�� ���¸� �����Ͽ� ������ Ÿ�� ����� �ش� ���·� ����
    /// </summary>
    /// <returns>���õ� Ÿ�ϰ� ȸ�� ����</returns>
    public TileVariant Collapse()
    {
        // ������ Ÿ�ϰ� ȸ�� ���� �� �������� �ϳ��� ����
        TileVariant selectedVariant = possibleTileVariants[Random.Range(0, possibleTileVariants.Count)];
        // ������ Ÿ�� ����� ���õ� Ÿ�ϰ� ȸ�� ���·� ����
        possibleTileVariants = new List<TileVariant> { selectedVariant };
        return selectedVariant;
    }

    /// <summary>
    /// ���Ǵ� Ÿ�ϰ� ȸ�� ���� ������� ������ Ÿ�ϰ� ȸ�� ���¸� ���̴� �޼���
    /// </summary>
    /// <param name="allowedVariants">���Ǵ� Ÿ�ϰ� ȸ�� ���� ���</param>
    /// <returns>Ÿ�� ����� ����Ǿ����� ����</returns>
    public bool ReducePossibleVariants(List<TileVariant> allowedVariants)
    {
        int beforeCount = possibleTileVariants.Count;
        // ������ Ÿ�� ����� ���Ǵ� Ÿ�ϰ� ȸ�� ���·� ���͸�
        possibleTileVariants = possibleTileVariants.Where(variant => allowedVariants.Contains(variant)).ToList();
        // Ÿ�� ����� ����Ǿ����� ��ȯ
        return possibleTileVariants.Count != beforeCount;
    }

    /// <summary>
    /// Ư�� Ÿ�ϰ� ȸ�� ���¸� ������ Ÿ�� ��Ͽ��� �����ϴ� �޼���
    /// </summary>
    /// <param name="variant">������ Ÿ�ϰ� ȸ�� ����</param>
    /// <returns>Ÿ�� ���� �Ŀ��� ������ Ÿ���� �����ִ��� ����</returns>
    public bool RemoveVariant(TileVariant variant)
    {
        if (possibleTileVariants.Remove(variant))
            return possibleTileVariants.Count > 0;

        return false;
    }

    /// <summary>
    /// ������ Ÿ�ϰ� ȸ�� ���¸� ��ȯ�ϴ� �޼���
    /// </summary>
    /// <returns>������ Ÿ�ϰ� ȸ�� ����</returns>
    public TileVariant GetCollapsedVariant()
    {
        return possibleTileVariants.FirstOrDefault();
    }

    /// <summary>
    /// ���� ������ Ÿ�ϰ� ȸ�� ���� ����� ��ȯ�ϴ� �޼���
    /// </summary>
    /// <returns>������ Ÿ�ϰ� ȸ�� ���� ���</returns>
    public List<TileVariant> GetPossibleVariants()
    {
        return new List<TileVariant>(possibleTileVariants);
    }

    /// <summary>
    /// Ư�� ���⿡ ���� ������ �̿� Ÿ�ϰ� ȸ�� ���� ����� ��ȯ�ϴ� �޼���
    /// </summary>
    /// <param name="direction">����(Direction)</param>
    /// <returns>������ �̿� Ÿ�ϰ� ȸ�� ���� ���</returns>
    public List<TileVariant> GetPossibleNeighborVariants(Direction direction)
    {
        List<TileVariant> neighborVariants = new List<TileVariant>();
        foreach (TileVariant variant in possibleTileVariants)
        {
            if (variant.Neighbors.TryGetValue(direction, out List<string> neighbors))
            {
                // �̿� Ÿ���� �̸� ����� ������� TileVariants�� ���͸�
                // �� �κ��� Ÿ�� �̸��� ȸ�� ���¿� ���� ��ü������ �����ؾ� �մϴ�.
                // ���÷� Ÿ�� �̸��� ���͸�
                foreach (var neighborName in neighbors)
                {
                    // ���� ���������� Ÿ�� ��Ʈ�� ȸ�� ���¸� �����Ͽ� TileVariants�� ��ȯ�ؾ� �մϴ�.
                    // ���⼭�� �ܼ��� �̸����� ���͸��� TileVariants�� ��ȯ�Ѵٰ� �����մϴ�.
                    // ���� ��ü Ÿ�� ��Ʈ�� �����Ͽ� �����ؾ� �մϴ�.
                }
            }
        }
        return neighborVariants.Distinct().ToList();
    }
}
