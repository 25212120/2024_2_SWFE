using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    [Header("∞¥√º¿« Ω∫≈» ¡§∫∏")]
    [SerializeField] protected StatData statData;

    protected virtual void Awake()
    {
        statData.InitStatData();
    }


    public virtual void TakeDamage(float damage)
    {
        if (statData.ModifyCurrentHp(-damage))
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
}
