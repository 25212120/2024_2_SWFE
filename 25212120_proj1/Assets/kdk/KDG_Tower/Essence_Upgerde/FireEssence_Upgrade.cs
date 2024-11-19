using System.Collections;
using UnityEngine;

public class FireEssence_Upgarde : MonoBehaviour
{
    public float burnDamage = 5f;      // ���ʸ��� �ִ� ȭ�� ������
    public float burnDuration = 5f;    // ȭ�� ȿ�� ���� �ð�
    private bool isBurning = false;    // ȭ�� ȿ�� �ߺ� ���� 

    private void OnTriggerEnter(Collider other)
    {
        BaseMonster targetMonster = other.GetComponent<BaseMonster>();

        if (targetMonster != null && !isBurning)
        {
            // ȭ�� ȿ���� ���� ������� ���� ��쿡�� ����
            StartCoroutine(ApplyBurnDamage(targetMonster));
        }
    }

    private IEnumerator ApplyBurnDamage(BaseMonster targetMonster)
    {
        isBurning = true;  // ȭ�� ȿ�� ����

        float elapsedTime = 0f;

        while (elapsedTime < burnDuration)
        {

            targetMonster.TakeDamage(burnDamage);


            elapsedTime += 1f;
            yield return new WaitForSeconds(1f);
        }

        isBurning = false;  // ȭ�� ȿ�� ����
    }
}
