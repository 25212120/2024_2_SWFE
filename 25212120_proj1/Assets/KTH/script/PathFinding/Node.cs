using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable; // 이동 가능 여부
    public Vector3 worldPosition; // 노드의 월드 좌표
    public int gridX;
    public int gridY;

    public int gCost; // 시작 노드부터의 실제 비용
    public int hCost; // 목표 노드까지의 예상 비용
    public int fCost => gCost + hCost; // 총 비용

    public Node parent; // 경로 추적을 위한 부모 노드

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }
}
