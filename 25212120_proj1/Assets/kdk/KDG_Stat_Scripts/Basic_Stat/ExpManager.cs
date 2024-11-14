using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ExpManager : Singleton<ExpManager>
{
    [Header("����ġ ��")]
    [SerializeField] private Image mExpCurrentBar;


    [SerializeField] private PlayerStat playerStat; // PlayerStat �ν��Ͻ� ����

    /// �÷��̾��� ���� ����ġ

    public float ExpCurrent { private set; get; }

    
    /// �÷��̾��� ���� �ִ� ����ġ
    
    public float ExpMax { private set; get; } = 100;

    private Coroutine? mCoUpdateExpBarFill;

    public void AddExp(float amount)
    {
        float expPrev = ExpCurrent;
        ExpCurrent += amount;

    }

    
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

                playerStat.LevelUp();
            }

            yield return null;
        }
    }
    
    
}
    