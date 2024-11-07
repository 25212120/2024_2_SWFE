using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 30f;  // 최대 체력
    public float currentHealth;     // 현재 체력

    void Start()
    {
        currentHealth = maxHealth;  // 시작 시 현재 체력을 최대 체력으로 설정
    }

    // 데미지를 받는 함수
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 사망 처리 함수
    void Die()
    {
        // 사망 효과나 애니메이션 추가 가능
        Destroy(gameObject);  // 오브젝트 삭제
    }
}
