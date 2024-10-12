using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable; // �̵� ���� ����
    public Vector3 worldPosition; // ����� ���� ��ǥ
    public int gridX;
    public int gridY;

    public int gCost; // ���� �������� ���� ���
    public int hCost; // ��ǥ �������� ���� ���
    public int fCost => gCost + hCost; // �� ���

    public Node parent; // ��� ������ ���� �θ� ���

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }
}
