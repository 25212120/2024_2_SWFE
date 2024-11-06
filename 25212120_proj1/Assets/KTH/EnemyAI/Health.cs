using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 30f;  // �ִ� ü��
    public float currentHealth;     // ���� ü��

    void Start()
    {
        currentHealth = maxHealth;  // ���� �� ���� ü���� �ִ� ü������ ����
    }

    // �������� �޴� �Լ�
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ��� ó�� �Լ�
    void Die()
    {
        // ��� ȿ���� �ִϸ��̼� �߰� ����
        Destroy(gameObject);  // ������Ʈ ����
    }
}
