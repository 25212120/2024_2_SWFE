using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    [Header("객체의 스탯 정보")]
    [SerializeField] public StatData statData;
    public delegate void DamageEventHandler(float damage);
    public event DamageEventHandler TakeDamageEvent;
    protected virtual void Awake()
    {
        statData.InitStatData();
    }

    // Debug 용
    protected virtual void Update()
    {
        Debug.Log(statData.HpCurrent + " : " + gameObject.name);
    }
    // Debug 용

    public virtual void TakeDamage(float damage)
    {
        // 방어력을 고려한 데미지 처리
        float effectiveDamage = damage - statData.DefenseCurrent;
        effectiveDamage = Mathf.Max(effectiveDamage, 0); // 최소 데미지는 0
        if (statData.ModifyCurrentHp(-effectiveDamage))
        {
            Die();
        }
        // 데미지 이벤트 호출
        if (TakeDamageEvent != null)
        {
            TakeDamageEvent.Invoke(damage);
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        if (gameObject.CompareTag("Player") == false && gameObject.CompareTag("Enemy") == false && gameObject.CompareTag("unit") == false)
        {
            Destroy(gameObject);
        }
    }

}
