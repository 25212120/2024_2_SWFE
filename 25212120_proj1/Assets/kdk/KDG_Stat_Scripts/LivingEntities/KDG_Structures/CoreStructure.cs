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
    void Start()
    {
       gridRenderer = GetComponent<GridRenderer>();
    }

    protected override void Update()
    {
        if (gridRenderer != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {

            }
        }
    }
}
