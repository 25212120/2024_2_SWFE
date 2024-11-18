
using System.Collections.Generic;
using UnityEngine;

public class BaseMaterial : BaseEntity
{
    private MaterialManager materialManager;

    [Header("Material �ڿ� ���")]
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

    public virtual void MaterialDie()
    {
        Die();
    }
    protected override void Awake()
    {
        base.Awake();
        materialManager = GetComponent<MaterialManager>();
    }
    protected override void Die()
    {
        base.Die();
        DropResources();
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
