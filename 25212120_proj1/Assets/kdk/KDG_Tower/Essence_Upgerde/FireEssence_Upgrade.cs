using System.Collections;
using UnityEngine;

public class FireEssence_Upgarde : MonoBehaviour
{
    public float burnDamage = 5f;      // 매초마다 주는 화상 데미지
    public float burnDuration = 5f;    // 화상 효과 지속 시간
    private bool isBurning = false;    // 화상 효과 중복 방지 

    private void OnTriggerEnter(Collider other)
    {
        BaseMonster targetMonster = other.GetComponent<BaseMonster>();

        if (targetMonster != null && !isBurning)
        {
            // 화상 효과가 아직 적용되지 않은 경우에만 시작
            StartCoroutine(ApplyBurnDamage(targetMonster));
        }
    }

    private IEnumerator ApplyBurnDamage(BaseMonster targetMonster)
    {
        isBurning = true;  // 화상 효과 시작

        float elapsedTime = 0f;

        while (elapsedTime < burnDuration)
        {

            targetMonster.TakeDamage(burnDamage);


            elapsedTime += 1f;
            yield return new WaitForSeconds(1f);
        }

        isBurning = false;  // 화상 효과 종료
    }
}
