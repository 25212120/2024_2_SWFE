
using UnityEngine;

public class ArrowTower : BaseStructure
{
    protected override void Awake()
    {
        base.Awake();
    }
    public override void Attack(BaseMonster target)
    {
        base.Attack(target);
    }

    protected override void Update()
    {
        Debug.Log("ȭ�� ���� �غ���");
        base.Update();
        if (Input.GetKeyDown(KeyCode.A))
        {
            UpgradeWithEssence(MaterialManager.ResourceType.WoodEssence);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            UpgradeWithEssence(MaterialManager.ResourceType.SandEssence);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            UpgradeWithEssence(MaterialManager.ResourceType.FireEssence);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            UpgradeWithEssence(MaterialManager.ResourceType.IceEssence);
        }
    }
}
