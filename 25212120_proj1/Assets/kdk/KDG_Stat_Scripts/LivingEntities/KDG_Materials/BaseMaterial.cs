
using System.Collections.Generic;
using UnityEngine;

public class BaseMaterial : BaseEntity
{
    private MaterialManager materialManager;

    [Header("Material 자원 드랍")]
    [SerializeField] protected List<ResourceDrop> resourceDrops = new List<ResourceDrop>();  // 자원 드랍 리스트

    [System.Serializable]
    public class ResourceDrop
    {
        public MaterialManager.ResourceType resourceType;  // 자원 타입
        public int amount;  // 자원 수량
        public float dropChance;  // 드랍 확률 (0~1)

        // 드랍 확률을 설정
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
        // 자원 드랍 처리
        foreach (var resourceDrop in resourceDrops)
        {
            MaterialManager.Instance.GainResourceWithChance(resourceDrop.resourceType, resourceDrop.amount, resourceDrop.dropChance);
            Debug.Log($"{gameObject.name} 드랍: {resourceDrop.amount} {resourceDrop.resourceType} 확률: {resourceDrop.dropChance * 100}%");
        }
    }
}
