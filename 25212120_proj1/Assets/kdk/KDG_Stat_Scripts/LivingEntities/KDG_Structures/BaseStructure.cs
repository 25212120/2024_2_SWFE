using System.Collections.Generic;
using UnityEngine;

public class BaseStructure : BaseEntity
{
    [Header("���׷��̵忡 �ʿ��� �ڿ�")]
    [SerializeField] private List<ResourceRequirement> upgradeRequirements = new List<ResourceRequirement>(); // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ
    [SerializeField] private Material upgradedMaterial;  // ���׷��̵� �� ������ ������ �⺻ ��Ƽ����
    [SerializeField] private Mesh upgradedMesh;         // ���׷��̵� �� ����� �޽� (���û���)

    [Header("������ ���׷��̵忡 ���� ���� ����")]
    [SerializeField] private Material iceEssenceMaterial;   // IceEssence�� �ش��ϴ� ��Ƽ����
    [SerializeField] private Material fireEssenceMaterial;  // FireEssence�� �ش��ϴ� ��Ƽ����
    [SerializeField] private Material sandEssenceMaterial;  // SandEssence�� �ش��ϴ� ��Ƽ����
    [SerializeField] private Material woodEssenceMaterial;  // WoodEssence�� �ش��ϴ� ��Ƽ����

    [Header("�Ϲ� ���׷��̵� �� ����� ����")]
    [SerializeField] private StatUpgrade statUpgrade = new StatUpgrade(); // ���׷��̵� �� ����� ���� ������

    private Renderer structureRenderer;
    private MeshFilter meshFilter;

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
        ChangeAppearanceWithEssence(essenceType);
        AddEssenceUpgradeScript(essenceType);
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

                break;

            case MaterialManager.ResourceType.FireEssence:
                // FireEssence�� �ش��ϴ� ��ũ��Ʈ�� �߰�
                if (structureRenderer != null && fireEssenceMaterial != null)
                    structureRenderer.material = fireEssenceMaterial;

                // FireEssence ��ũ��Ʈ �߰�
                if (GetComponent<FireEssence_Upgarde>() == null)  // �ߺ� �߰� ����
                    gameObject.AddComponent<FireEssence_Upgarde>();

                break;

            case MaterialManager.ResourceType.SandEssence:
                // SandEssence�� �ش��ϴ� ��Ƽ����� ����
                if (structureRenderer != null && sandEssenceMaterial != null)
                    structureRenderer.material = sandEssenceMaterial;

                if (GetComponent<SandEssence_Upgrade>() == null)  // �ߺ� �߰� ����
                    gameObject.AddComponent<SandEssence_Upgrade>();
                break;

            case MaterialManager.ResourceType.WoodEssence:
                // WoodEssence�� �ش��ϴ� ��Ƽ����� ����
                if (structureRenderer != null && woodEssenceMaterial != null)
                    structureRenderer.material = woodEssenceMaterial;
                break;

            default:
                Debug.LogWarning("�� �� ���� ������ Ÿ���Դϴ�.");
                break;
        }
    }
    // �������� ���� ���� ����
    private void ChangeAppearanceWithEssence(MaterialManager.ResourceType essenceType)
    {
        switch (essenceType)
        {
            case MaterialManager.ResourceType.IceEssence:
                if (structureRenderer != null && iceEssenceMaterial != null)
                    structureRenderer.material = iceEssenceMaterial;  // IceEssence�� �ش��ϴ� ��Ƽ����� ����
                break;

            case MaterialManager.ResourceType.FireEssence:
                if (structureRenderer != null && fireEssenceMaterial != null)
                    structureRenderer.material = fireEssenceMaterial;  // FireEssence�� �ش��ϴ� ��Ƽ����� ����
                break;

            case MaterialManager.ResourceType.SandEssence:
                if (structureRenderer != null && sandEssenceMaterial != null)
                    structureRenderer.material = sandEssenceMaterial;  // SandEssence�� �ش��ϴ� ��Ƽ����� ����
                break;

            case MaterialManager.ResourceType.WoodEssence:
                if (structureRenderer != null && woodEssenceMaterial != null)
                    structureRenderer.material = woodEssenceMaterial;  // WoodEssence�� �ش��ϴ� ��Ƽ����� ����
                break;

            default:
                Debug.LogWarning("�� �� ���� ������ Ÿ���Դϴ�.");
                break;
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
    }
}
