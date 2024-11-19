using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Material_1 : BaseMaterial
{
    protected override void Awake()
    {
        base.Awake();
    }
    public override void MaterialDie(float waittime)
    {
        base.MaterialDie(waittime);
    }
    
    void Update()
    {
        /*
        // A키가 눌렸을 때
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("a");

            // waittime을 임의로 설정, 예를 들어 2초로 설정
            MaterialDie(5f);
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            SetWaitSuccess_False();
        }
        */
    }
}
