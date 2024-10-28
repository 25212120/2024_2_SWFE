using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ExpManager : Singleton<ExpManager>
{
    [Header("경험치 바")]
    [SerializeField] private Image mExpCurrentBar;

    
    /// 플레이어의 현재 경험치
    
    public float ExpCurrent { private set; get; }

    
    /// 플레이어의 현재 최대 경험치
    
    public float ExpMax { private set; get; } = 100;

    private Coroutine? mCoUpdateExpBarFill;

    public void AddExp(float amount)
    {
        float expPrev = ExpCurrent;
        ExpCurrent += amount;

    }

    /*
    private IEnumerator CoUpdateExpBarFill(float expPrev)
    {
        float process = 0f;

        while (process < 1f)
        {
            process += Time.deltaTime;

            expPrev = Mathf.Lerp(expPrev, ExpCurrent, process);
            mExpCurrentBar.fillAmount = expPrev / ExpMax;

            if (expPrev / ExpMax > 1f)
            {
                expPrev = 0f;
                process = 0f;
                ExpCurrent -= ExpMax;
                ExpMax *= 2.0f;

                StatManager.instance.LevelUp(); // 스텟메니저 레벨업 호출
            }

            yield return null;
        }
    }
    */
    /*
    public void DestroyEntity()
    {
        // 이미 파괴 대기 상태라면 리턴
        if (IsWaitDead)
            return;

        // 파괴 대기상태 활성화
        IsWaitDead = true;

        // 경험치 지급
        ExpManager.instance.AddExp(GiveExp); // 몬스터 스텟에서 처치 시 줄 exp 설정
    }*/
    
}
    