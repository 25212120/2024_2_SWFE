using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Material_Crystal : BaseMaterial
{
    [HideInInspector]
    public bool _WaitSuccess;

    private List<ResourceDrop> customResourceDrops = new List<ResourceDrop>
    {
        new ResourceDrop(MaterialManager.ResourceType.Crystal, 5, 0.7f),   // ���� 5��, 70% Ȯ��
    };
    protected override void Awake()
    {
        base.Awake();

        _WaitSuccess = WaitSuccess;

        resourceDrops = customResourceDrops;

    }
}
