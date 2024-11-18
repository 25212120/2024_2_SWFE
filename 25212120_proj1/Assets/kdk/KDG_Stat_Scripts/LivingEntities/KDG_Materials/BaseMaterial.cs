
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMaterial : MonoBehaviour
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
    protected virtual void Awake()
    {
        materialManager = GetComponent<MaterialManager>();
    }

    private Coroutine dieCoroutine;

    public bool finished = false;
    public bool WaitSuccess = false;

    public virtual void MaterialDie(float waittime)
    {
        if (dieCoroutine != null)
        {
            StopCoroutine(dieCoroutine);
        }
        WaitSuccess = true;  // 코루틴 실행 전 True로 설정
        dieCoroutine = StartCoroutine(DieAfterWait(waittime));
    }

    // 코루틴: waittime 후에 Die 호출
    private IEnumerator DieAfterWait(float waittime)
    {
        float elapsedTime = 0f; // 경과 시간 추적

        // WaitSuccess가 false로 변경되거나 시간이 초과될 때까지 대기
        while (elapsedTime < waittime)
        {
            if (!WaitSuccess) // WaitSuccess가 false로 변경되면 즉시 취소
            {
                CancelMaterialDie();
                yield break;
            }

            elapsedTime += Time.deltaTime; // 경과 시간 누적
            yield return null; // 다음 프레임까지 대기
        }

        // 대기 완료 후 Die 실행
        finished = true;
    }

    public virtual void Die()
    {
        Destroy(gameObject);  
        DropResources();  // 자원 드랍
    }

    // 자원 드랍 처리
    private void DropResources()
    {
        foreach (var resourceDrop in resourceDrops)
        {
            MaterialManager.Instance.GainResourceWithChance(resourceDrop.resourceType, resourceDrop.amount, resourceDrop.dropChance);
            Debug.Log($"{gameObject.name} 드랍: {resourceDrop.amount} {resourceDrop.resourceType} 확률: {resourceDrop.dropChance * 100}%");
        }
    }

    // 피격 시 호출
    public void CancelMaterialDie()
    {
        if (dieCoroutine != null)
        {
            StopCoroutine(dieCoroutine);
            dieCoroutine = null;  
            Debug.Log("MaterialDie 취소됨");
        }
        WaitSuccess = false; // 코루틴을 취소하면 상태를 False로 설정
    }

    // WaitSuccess를 변경하여 코루틴을 취소
    public void SetWaitSuccess_False()
    {
        WaitSuccess = false;
        if (!WaitSuccess && dieCoroutine != null)
        {
            CancelMaterialDie();
        }
    }
}
