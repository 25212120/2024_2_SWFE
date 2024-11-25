using System.Collections.Generic;
using UnityEngine;

public class BaseStructure : BaseEntity
{
    [Header("���׷��̵忡 �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> upgradeRequirements = new List<ResourceRequirement>(); // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
    [SerializeField] private Material upgradedMaterial;  // ���׷��̵� �� ������ ������ �⺻ ��Ƽ����
    [SerializeField] private Mesh upgradedMesh;         // ���׷��̵� �� ����� �޽� (���û���)

    [Header("������ ���׷��̵忡 ���� ���� ����")]
    [SerializeField] private Material iceEssenceMaterial;   // IceEssence�� �ش��ϴ� ��Ƽ����
    [SerializeField] private Material fireEssenceMaterial;  // FireEssence�� �ش��ϴ� ��Ƽ����
    [SerializeField] private Material sandEssenceMaterial;  // SandEssence�� �ش��ϴ� ��Ƽ����
    [SerializeField] private Material woodEssenceMaterial;  // WoodEssence�� �ش��ϴ� ��Ƽ����

    [Header("�Ϲ� ���׷��̵� �� ����� ����")]
    [SerializeField] private StatUpgrade statUpgrade = new StatUpgrade(); // ���׷��̵� �� ����� ���� ������

    [Header("Ÿ���� �Ѿ� ������")]
    [SerializeField] private GameObject bulletPrefab;  // �Ѿ� ������

    private Renderer structureRenderer;
    private MeshFilter meshFilter;
    private bool EssenceUpgraded = false;
    protected override void Awake()
    {
        base.Awake();
        structureRenderer = GetComponent<Renderer>();  
        meshFilter = GetComponent<MeshFilter>();       
    }



    // �Ϲ� �ڿ� ���׷��̵� (������ ����)
    public bool UpgradeWithoutEssence()
    {
        
        List<ResourceRequirement> nonEssenceRequirements = new List<ResourceRequirement>();

        foreach (var requirement in upgradeRequirements)
        {
            if (!IsEssenceResource(requirement.resourceType))
            {
                nonEssenceRequirements.Add(requirement);
            }
        }

        // �Ϲ� �ڿ���� ���׷��̵尡 �������� Ȯ��
        foreach (var requirement in nonEssenceRequirements)
        {
            if (!MaterialManager.Instance.ConsumeResource(requirement.resourceType, requirement.amount))
            {
                // �ڿ��� �����ϸ� ���׷��̵� ����
                Debug.LogWarning($"{requirement.resourceType} �ڿ��� �����մϴ�. ���׷��̵� ����.");
                return false;
            }
        }

        // ��� �ڿ� �Һ� �����ϸ� ���׷��̵� ����
        PerformUpgrade();
        return true;
    }

    // ������ �ڿ��� ����Ͽ� ���׷��̵� ����
    public bool UpgradeWithEssence(MaterialManager.ResourceType essenceType)
    {
        if (EssenceUpgraded == true)
        {
            Debug.Log("������ ��� ������ 1����");
            return false;
        }
        List<ResourceRequirement> essenceRequirements = new List<ResourceRequirement>();

        foreach (var requirement in upgradeRequirements)
        {
            // ������ �ڿ��� ����
            if (IsEssenceResource(requirement.resourceType))
            {
                essenceRequirements.Add(requirement);
            }
        }

        // ������ �ڿ��鸸���� ���׷��̵尡 �������� Ȯ��
        bool essenceConsumed = false;
        foreach (var requirement in essenceRequirements)
        {
            if (requirement.resourceType == essenceType) // ������ ������ �ڿ��� �Ҹ�
            {
                if (!MaterialManager.Instance.ConsumeResource(requirement.resourceType, requirement.amount))
                {
                    // �ڿ��� �����ϸ� ���׷��̵� ����
                    Debug.LogWarning($"{requirement.resourceType} �ڿ��� �����մϴ�. ���� ���� ����.");
                    return false;
                }
                essenceConsumed = true;
                break; // �������� �� ������ ����ϹǷ� �� ���� �Ҹ�
            }
        }

        if (!essenceConsumed)
        {
            // ������ ������ �ڿ��� ������ ����
            Debug.LogWarning("��ȿ�� ������ �ڿ��� �����մϴ�. ���� ���� ����.");
            return false;
        }

        // �������� �Ҹ��ϰ� ���� ����
        AddEssenceUpgradeScript(essenceType);
        EssenceUpgraded = true;
        // ���׷��̵� �Ϸ�
        return true;
    }

    private void AddEssenceUpgradeScript(MaterialManager.ResourceType essenceType)
    {
        switch (essenceType)
        {
            case MaterialManager.ResourceType.IceEssence:
                // IceEssence�� �ش��ϴ� ��ũ��Ʈ�� �߰�
                if (structureRenderer != null && iceEssenceMaterial != null)
                    structureRenderer.material = iceEssenceMaterial;
                /*
                if (bulletPrefab.GetComponent<IceEssence_Bullet>() == null)  // �ߺ� �߰� ����
                    bulletPrefab.AddComponent<IceEssence_Bullet>();
                */
                break;

            case MaterialManager.ResourceType.FireEssence:
                // FireEssence�� �ش��ϴ� ��ũ��Ʈ�� �߰�
                if (structureRenderer != null && fireEssenceMaterial != null)
                    structureRenderer.material = fireEssenceMaterial;

                // FireEssence ��ũ��Ʈ �߰�
                if (bulletPrefab.GetComponent<FireEssence_Upgarde>() == null)  // �ߺ� �߰� ����
                    bulletPrefab.AddComponent<FireEssence_Upgarde>();

                break;

            case MaterialManager.ResourceType.SandEssence:
                // SandEssence�� �ش��ϴ� ��Ƽ����� ����
                if (structureRenderer != null && sandEssenceMaterial != null)
                    structureRenderer.material = sandEssenceMaterial;

                if (bulletPrefab.GetComponent<SandEssence_Upgrade>() == null)  // �ߺ� �߰� ����
                    gameObject.AddComponent<SandEssence_Upgrade>();
                break;

            case MaterialManager.ResourceType.WoodEssence:
                // WoodEssence�� �ش��ϴ� ��Ƽ����� ����
                if (structureRenderer != null && woodEssenceMaterial != null)
                    structureRenderer.material = woodEssenceMaterial;
                statData.UpgradeBaseStat(StatData.StatType.DEFENSE, 10); // ���� ����
                statData.UpgradeBaseStat(StatData.StatType.HP, 100); // ü�� ����
                break;

            default:
                Debug.LogWarning("�� �� ���� ������ Ÿ���Դϴ�.");
                break;
        }
        ChangeChildrenMaterials(essenceType);

    }
    // �������� ���� ���� ����
   
    private void ChangeChildrenMaterials(MaterialManager.ResourceType essenceType)
    {
        // �ڽ� ������Ʈ�� �߿��� Renderer ������Ʈ�� ã�Ƽ� Material�� ����
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer childRenderer in childRenderers)
        {
            if (childRenderer != null)
            {
                switch (essenceType)
                {
                    case MaterialManager.ResourceType.IceEssence:
                        if (iceEssenceMaterial != null)
                            childRenderer.material = iceEssenceMaterial;
                        break;

                    case MaterialManager.ResourceType.FireEssence:
                        if (fireEssenceMaterial != null)
                            childRenderer.material = fireEssenceMaterial;
                        break;

                    case MaterialManager.ResourceType.SandEssence:
                        if (sandEssenceMaterial != null)
                            childRenderer.material = sandEssenceMaterial;
                        break;

                    case MaterialManager.ResourceType.WoodEssence:
                        if (woodEssenceMaterial != null)
                            childRenderer.material = woodEssenceMaterial;
                        break;

                    default:
                        Debug.LogWarning("�� �� ���� ������ Ÿ���Դϴ�.");
                        break;
                }
            }
        }
    }

    // �ڿ��� ���������� Ȯ���ϴ� �Լ�
    private bool IsEssenceResource(MaterialManager.ResourceType resourceType)
    {
        switch (resourceType)
        {
            case MaterialManager.ResourceType.WoodEssence:
            case MaterialManager.ResourceType.IceEssence:
            case MaterialManager.ResourceType.FireEssence:
            case MaterialManager.ResourceType.SandEssence:
                return true; // ������ �ڿ��� ��� true ��ȯ
            default:
                return false; // �������� �ƴ� �ڿ�
        }
    }

    private void PerformUpgrade()
    {
        // ������ ������Ű�� ���� (StatUpgrade Ŭ�������� ���ǵ� ����ŭ ����)
        statData.UpgradeBaseStat(StatData.StatType.ATTACK, statUpgrade.attackIncrease); // ���ݷ� ����
        statData.UpgradeBaseStat(StatData.StatType.DEFENSE, statUpgrade.defenseIncrease); // ���� ����
        statData.UpgradeBaseStat(StatData.StatType.HP, statUpgrade.healthIncrease); // ü�� ����

        Debug.Log("���׷��̵� �Ϸ�: ���ݷ� + " + statUpgrade.attackIncrease + ", ���� + " + statUpgrade.defenseIncrease + ", ü�� + " + statUpgrade.healthIncrease);
    }

    public virtual void Repair(float amount)
    {
        statData.ModifyCurrentHp(amount);
    }

    public virtual void Attack(BaseMonster target)
    {
        float damage = statData.AttackCurrent; // ���� ���ݷ� ���
        target.TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();
    }

    // ���׷��̵忡 �ʿ��� �ڿ� Ŭ����
    [System.Serializable]
    public class ResourceRequirement
    {
        public MaterialManager.ResourceType resourceType; // �ڿ� Ÿ��
        public int amount;                                // �ڿ� ����
        public ResourceRequirement(MaterialManager.ResourceType resourceType, int amount)
        {
            this.resourceType = resourceType;
            this.amount = amount;
        }
    }

    public bool Upgradecheck()
    {
        return EssenceUpgraded;
    }
}
