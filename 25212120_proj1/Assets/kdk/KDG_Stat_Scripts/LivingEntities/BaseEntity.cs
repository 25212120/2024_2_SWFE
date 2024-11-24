using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    [Header("��ü�� ���� ����")]
    [SerializeField] public StatData statData;
    public delegate void DamageEventHandler(float damage);
    public event DamageEventHandler TakeDamageEvent;
    protected virtual void Awake()
    {
        statData.InitStatData();
    }

    // Debug ��
    protected virtual void Update()
    {
        Debug.Log(statData.HpCurrent + " : " + gameObject.name);
    }
    // Debug ��

    public virtual void TakeDamage(float damage)
    {
        // ������ ����� ������ ó��
        float effectiveDamage = damage - statData.DefenseCurrent;
        effectiveDamage = Mathf.Max(effectiveDamage, 0); // �ּ� �������� 0
        if (statData.ModifyCurrentHp(-effectiveDamage))
        {
            Die();
        }
        // ������ �̺�Ʈ ȣ��
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
