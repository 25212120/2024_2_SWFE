
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMaterial : MonoBehaviour
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
    protected virtual void Awake()
    {
        materialManager = GetComponent<MaterialManager>();
    }

    private Coroutine dieCoroutine;  

    public bool WaitSuccess = false;
    public virtual void MaterialDie(float waittime)
    {
        if (dieCoroutine != null)
        {
            StopCoroutine(dieCoroutine);
        }
        WaitSuccess = true;  // �ڷ�ƾ ���� �� True�� ����
        dieCoroutine = StartCoroutine(DieAfterWait(waittime));
    }

    // �ڷ�ƾ: waittime �Ŀ� Die ȣ��
    private IEnumerator DieAfterWait(float waittime)
    {
        yield return new WaitForSeconds(waittime);  // waittime��ŭ ���

        // �ڷ�ƾ�� ���� ���̸� False�� �ٲ�� ���
        if (!WaitSuccess)
        {
            CancelMaterialDie();  // ���
            yield break;
        }

        Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);  
        DropResources();  // �ڿ� ���
    }

    // �ڿ� ��� ó��
    private void DropResources()
    {
        foreach (var resourceDrop in resourceDrops)
        {
            MaterialManager.Instance.GainResourceWithChance(resourceDrop.resourceType, resourceDrop.amount, resourceDrop.dropChance);
            Debug.Log($"{gameObject.name} ���: {resourceDrop.amount} {resourceDrop.resourceType} Ȯ��: {resourceDrop.dropChance * 100}%");
        }
    }

    // �ǰ� �� ȣ��
    public void CancelMaterialDie()
    {
        if (dieCoroutine != null)
        {
            StopCoroutine(dieCoroutine);
            dieCoroutine = null;  
            Debug.Log("MaterialDie ��ҵ�");
        }
        WaitSuccess = false; // �ڷ�ƾ�� ����ϸ� ���¸� False�� ����
    }

    // WaitSuccess�� �����Ͽ� �ڷ�ƾ�� ���
    public void SetWaitSuccess_False()
    {
        WaitSuccess = false;
        if (!WaitSuccess && dieCoroutine != null)
        {
            CancelMaterialDie();
        }
    }
}
