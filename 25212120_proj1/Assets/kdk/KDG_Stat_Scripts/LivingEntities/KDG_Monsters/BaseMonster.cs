using UnityEngine;
using System.Collections.Generic;

public class BaseMonster : BaseEntity
{
    [Header("���� ����ġ")]
    [SerializeField] protected int expAmount;

    [Header("���� �ڿ� ���")]
    [SerializeField] protected List<ResourceDrop> resourceDrops = new List<ResourceDrop>();  // �ڿ� ��� ����Ʈ

    [System.Serializable]
    public class ResourceDrop
    {
        public MaterialManager.ResourceType resourceType;  // �ڿ� Ÿ��
        public int amount;  // �ڿ� ����
        public float dropChance;  // ��� Ȯ�� (0~1)

        // ��� Ȯ���� ����
        public ResourceDrop(MaterialManager.ResourceType resourceType, int amount, float dropChance)
        {
            this.resourceType = resourceType;
            this.amount = amount;
            this.dropChance = dropChance;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        InitializeMonsterStats();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected virtual void InitializeMonsterStats()
    {
        statData.SetHpMax(100);  // ������ �ִ� ü�� �ʱ�ȭ
        statData.baseAttack = 10;  // ������ �⺻ ���ݷ� �ʱ�ȭ
    }

    public void Attack(BaseEntity target)
    {
        float damage = statData.AttackCurrent; // ���� ���ݷ� ���
        target.TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();  // �⺻ ��� ó��
        AwardExperienceToPlayer();
        DropResources();  // �ڿ� ��� ó��
    }

    private void AwardExperienceToPlayer()
    {
        ExpManager.Instance.AddExp(expAmount);  // ����ġ ����
    }

    private void DropResources()
    {
        // �ڿ� ��� ó��
        foreach (var resourceDrop in resourceDrops)
        {
            MaterialManager.Instance.GainResourceWithChance(resourceDrop.resourceType, resourceDrop.amount, resourceDrop.dropChance);
            Debug.Log($"{gameObject.name} ���: {resourceDrop.amount} {resourceDrop.resourceType} Ȯ��: {resourceDrop.dropChance * 100}%");
        }
    }
}
