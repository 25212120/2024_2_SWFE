using UnityEngine;

public class Material_Test : BaseMaterial
{
    [HideInInspector]
    public bool _WaitSuccess;

    protected override void Awake()
    {
        base.Awake();

        _WaitSuccess = WaitSuccess;
    }

}
