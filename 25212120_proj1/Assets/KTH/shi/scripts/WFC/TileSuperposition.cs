using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 타일 셀의 가능한 타일과 회전 상태들을 관리하는 클래스
/// WFC 알고리즘에서 타일과 회전 상태를 결정하고 제약 조건을 적용하는 데 사용됩니다.
/// </summary>
public class TileSuperposition
{
    /// <summary>
    /// 가능한 타일과 회전 상태들의 리스트
    /// </summary>
    private List<TileVariant> possibleTileVariants;

    /// <summary>
    /// 현재 셀의 엔트로피 (가능한 타일과 회전 상태의 개수)
    /// </summary>
    public int Entropy => possibleTileVariants.Count;

    /// <summary>
    /// 타일 슈퍼포지션의 생성자
    /// </summary>
    /// <param name="tileSet">전체 타일 세트</param>
    public TileSuperposition(List<Tile> tileSet)
    {
        possibleTileVariants = new List<TileVariant>();

        // 모든 타일의 모든 회전 상태를 가능한 타일로 추가
        foreach (var tile in tileSet)
        {
            foreach (var variant in tile.Variants)
            {
                possibleTileVariants.Add(variant);
            }
        }
    }

    /// <summary>
    /// 셀을 하나의 타일과 회전 상태로 결정하는 메서드
    /// 무작위로 하나의 타일과 회전 상태를 선택하여 가능한 타일 목록을 해당 상태로 설정
    /// </summary>
    /// <returns>선택된 타일과 회전 상태</returns>
    public TileVariant Collapse()
    {
        // 가능한 타일과 회전 상태 중 무작위로 하나를 선택
        TileVariant selectedVariant = possibleTileVariants[Random.Range(0, possibleTileVariants.Count)];
        // 가능한 타일 목록을 선택된 타일과 회전 상태로 제한
        possibleTileVariants = new List<TileVariant> { selectedVariant };
        return selectedVariant;
    }

    /// <summary>
    /// 허용되는 타일과 회전 상태 목록으로 가능한 타일과 회전 상태를 줄이는 메서드
    /// </summary>
    /// <param name="allowedVariants">허용되는 타일과 회전 상태 목록</param>
    /// <returns>타일 목록이 변경되었는지 여부</returns>
    public bool ReducePossibleVariants(List<TileVariant> allowedVariants)
    {
        int beforeCount = possibleTileVariants.Count;
        // 가능한 타일 목록을 허용되는 타일과 회전 상태로 필터링
        possibleTileVariants = possibleTileVariants.Where(variant => allowedVariants.Contains(variant)).ToList();
        // 타일 목록이 변경되었는지 반환
        return possibleTileVariants.Count != beforeCount;
    }

    /// <summary>
    /// 특정 타일과 회전 상태를 가능한 타일 목록에서 제거하는 메서드
    /// </summary>
    /// <param name="variant">제거할 타일과 회전 상태</param>
    /// <returns>타일 제거 후에도 가능한 타일이 남아있는지 여부</returns>
    public bool RemoveVariant(TileVariant variant)
    {
        if (possibleTileVariants.Remove(variant))
            return possibleTileVariants.Count > 0;

        return false;
    }

    /// <summary>
    /// 결정된 타일과 회전 상태를 반환하는 메서드
    /// </summary>
    /// <returns>결정된 타일과 회전 상태</returns>
    public TileVariant GetCollapsedVariant()
    {
        return possibleTileVariants.FirstOrDefault();
    }

    /// <summary>
    /// 현재 가능한 타일과 회전 상태 목록을 반환하는 메서드
    /// </summary>
    /// <returns>가능한 타일과 회전 상태 목록</returns>
    public List<TileVariant> GetPossibleVariants()
    {
        return new List<TileVariant>(possibleTileVariants);
    }

    /// <summary>
    /// 특정 방향에 대해 가능한 이웃 타일과 회전 상태 목록을 반환하는 메서드
    /// </summary>
    /// <param name="direction">방향(Direction)</param>
    /// <returns>가능한 이웃 타일과 회전 상태 목록</returns>
    public List<TileVariant> GetPossibleNeighborVariants(Direction direction)
    {
        List<TileVariant> neighborVariants = new List<TileVariant>();
        foreach (TileVariant variant in possibleTileVariants)
        {
            if (variant.Neighbors.TryGetValue(direction, out List<string> neighbors))
            {
                // 이웃 타일의 이름 목록을 기반으로 TileVariants를 필터링
                // 이 부분은 타일 이름과 회전 상태에 따라 구체적으로 구현해야 합니다.
                // 예시로 타일 이름만 필터링
                foreach (var neighborName in neighbors)
                {
                    // 실제 구현에서는 타일 세트와 회전 상태를 참조하여 TileVariants를 반환해야 합니다.
                    // 여기서는 단순히 이름으로 필터링된 TileVariants를 반환한다고 가정합니다.
                    // 추후 전체 타일 세트를 참조하여 구현해야 합니다.
                }
            }
        }
        return neighborVariants.Distinct().ToList();
    }
}
