using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoreStructure : BaseStructure
{
    [SerializeField] private GridRenderer gridRenderer;
    protected override void Awake()
    {
        base.Awake();
    }
}
