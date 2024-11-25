using System.Collections.Generic;
using UnityEngine;

public class BaseStructure : BaseEntity
{
    [Header("업그레이드에 필요한 자원")]
    [SerializeField] public List<ResourceRequirement> upgradeRequirements = new List<ResourceRequirement>(); // 업그레이드에 필요한 자원 리스트
    [SerializeField] private Material upgradedMaterial;  // 업그레이드 후 외형을 변경할 기본 머티리얼
    [SerializeField] private Mesh upgradedMesh;         // 업그레이드 후 사용할 메쉬 (선택사항)

    [Header("에센스 업그레이드에 따른 외형 변경")]
    [SerializeField] private Material iceEssenceMaterial;   // IceEssence에 해당하는 머티리얼
    [SerializeField] private Material fireEssenceMaterial;  // FireEssence에 해당하는 머티리얼
    [SerializeField] private Material sandEssenceMaterial;  // SandEssence에 해당하는 머티리얼
    [SerializeField] private Material woodEssenceMaterial;  // WoodEssence에 해당하는 머티리얼

    [Header("일반 업그레이드 시 적용될 스텟")]
    [SerializeField] private StatUpgrade statUpgrade = new StatUpgrade(); // 업그레이드 시 적용될 스텟 증가값

    [Header("타워의 총알 프리팹")]
    [SerializeField] private GameObject bulletPrefab;  // 총알 프리팹

    private Renderer structureRenderer;
    private MeshFilter meshFilter;
    private bool EssenceUpgraded = false;
    protected override void Awake()
    {
        base.Awake();
        structureRenderer = GetComponent<Renderer>();  
        meshFilter = GetComponent<MeshFilter>();       
    }



    // 일반 자원 업그레이드 (에센스 제외)
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

        // 일반 자원들로 업그레이드가 가능한지 확인
        foreach (var requirement in nonEssenceRequirements)
        {
            if (!MaterialManager.Instance.ConsumeResource(requirement.resourceType, requirement.amount))
            {
                // 자원이 부족하면 업그레이드 실패
                Debug.LogWarning($"{requirement.resourceType} 자원이 부족합니다. 업그레이드 실패.");
                return false;
            }
        }

        // 모든 자원 소비가 성공하면 업그레이드 진행
        PerformUpgrade();
        return true;
    }

    // 에센스 자원을 사용하여 업그레이드 진행
    public bool UpgradeWithEssence(MaterialManager.ResourceType essenceType)
    {
        if (EssenceUpgraded == true)
        {
            Debug.Log("에센스 사용 업글은 1번만");
            return false;
        }
        List<ResourceRequirement> essenceRequirements = new List<ResourceRequirement>();

        foreach (var requirement in upgradeRequirements)
        {
            // 에센스 자원만 추출
            if (IsEssenceResource(requirement.resourceType))
            {
                essenceRequirements.Add(requirement);
            }
        }

        // 에센스 자원들만으로 업그레이드가 가능한지 확인
        bool essenceConsumed = false;
        foreach (var requirement in essenceRequirements)
        {
            if (requirement.resourceType == essenceType) // 지정된 에센스 자원만 소모
            {
                if (!MaterialManager.Instance.ConsumeResource(requirement.resourceType, requirement.amount))
                {
                    // 자원이 부족하면 업그레이드 실패
                    Debug.LogWarning($"{requirement.resourceType} 자원이 부족합니다. 외형 변경 실패.");
                    return false;
                }
                essenceConsumed = true;
                break; // 에센스는 한 종류만 사용하므로 한 번만 소모
            }
        }

        if (!essenceConsumed)
        {
            // 지정된 에센스 자원이 없으면 실패
            Debug.LogWarning("유효한 에센스 자원이 부족합니다. 외형 변경 실패.");
            return false;
        }

        // 에센스를 소모하고 외형 변경
        AddEssenceUpgradeScript(essenceType);
        EssenceUpgraded = true;
        // 업그레이드 완료
        return true;
    }

    private void AddEssenceUpgradeScript(MaterialManager.ResourceType essenceType)
    {
        switch (essenceType)
        {
            case MaterialManager.ResourceType.IceEssence:
                // IceEssence에 해당하는 스크립트를 추가
                if (structureRenderer != null && iceEssenceMaterial != null)
                    structureRenderer.material = iceEssenceMaterial;
                /*
                if (bulletPrefab.GetComponent<IceEssence_Bullet>() == null)  // 중복 추가 방지
                    bulletPrefab.AddComponent<IceEssence_Bullet>();
                */
                break;

            case MaterialManager.ResourceType.FireEssence:
                // FireEssence에 해당하는 스크립트를 추가
                if (structureRenderer != null && fireEssenceMaterial != null)
                    structureRenderer.material = fireEssenceMaterial;

                // FireEssence 스크립트 추가
                if (bulletPrefab.GetComponent<FireEssence_Upgarde>() == null)  // 중복 추가 방지
                    bulletPrefab.AddComponent<FireEssence_Upgarde>();

                break;

            case MaterialManager.ResourceType.SandEssence:
                // SandEssence에 해당하는 머티리얼로 변경
                if (structureRenderer != null && sandEssenceMaterial != null)
                    structureRenderer.material = sandEssenceMaterial;

                if (bulletPrefab.GetComponent<SandEssence_Upgrade>() == null)  // 중복 추가 방지
                    gameObject.AddComponent<SandEssence_Upgrade>();
                break;

            case MaterialManager.ResourceType.WoodEssence:
                // WoodEssence에 해당하는 머티리얼로 변경
                if (structureRenderer != null && woodEssenceMaterial != null)
                    structureRenderer.material = woodEssenceMaterial;
                statData.UpgradeBaseStat(StatData.StatType.DEFENSE, 10); // 방어력 증가
                statData.UpgradeBaseStat(StatData.StatType.HP, 100); // 체력 증가
                break;

            default:
                Debug.LogWarning("알 수 없는 에센스 타입입니다.");
                break;
        }
        ChangeChildrenMaterials(essenceType);

    }
    // 에센스에 따른 외형 변경
   
    private void ChangeChildrenMaterials(MaterialManager.ResourceType essenceType)
    {
        // 자식 오브젝트들 중에서 Renderer 컴포넌트를 찾아서 Material을 변경
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
                        Debug.LogWarning("알 수 없는 에센스 타입입니다.");
                        break;
                }
            }
        }
    }

    // 자원이 에센스인지 확인하는 함수
    private bool IsEssenceResource(MaterialManager.ResourceType resourceType)
    {
        switch (resourceType)
        {
            case MaterialManager.ResourceType.WoodEssence:
            case MaterialManager.ResourceType.IceEssence:
            case MaterialManager.ResourceType.FireEssence:
            case MaterialManager.ResourceType.SandEssence:
                return true; // 에센스 자원일 경우 true 반환
            default:
                return false; // 에센스가 아닌 자원
        }
    }

    private void PerformUpgrade()
    {
        // 스텟을 증가시키는 로직 (StatUpgrade 클래스에서 정의된 값만큼 증가)
        statData.UpgradeBaseStat(StatData.StatType.ATTACK, statUpgrade.attackIncrease); // 공격력 증가
        statData.UpgradeBaseStat(StatData.StatType.DEFENSE, statUpgrade.defenseIncrease); // 방어력 증가
        statData.UpgradeBaseStat(StatData.StatType.HP, statUpgrade.healthIncrease); // 체력 증가

        Debug.Log("업그레이드 완료: 공격력 + " + statUpgrade.attackIncrease + ", 방어력 + " + statUpgrade.defenseIncrease + ", 체력 + " + statUpgrade.healthIncrease);
    }

    public virtual void Repair(float amount)
    {
        statData.ModifyCurrentHp(amount);
    }

    public virtual void Attack(BaseMonster target)
    {
        float damage = statData.AttackCurrent; // 현재 공격력 사용
        target.TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();
    }

    // 업그레이드에 필요한 자원 클래스
    [System.Serializable]
    public class ResourceRequirement
    {
        public MaterialManager.ResourceType resourceType; // 자원 타입
        public int amount;                                // 자원 수량
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
