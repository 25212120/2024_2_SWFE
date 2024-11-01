using TMPro;
using UnityEngine;

public class StatManager : Singleton<StatManager>
{
    public static bool IsStatDialogEnable { private set; get; } = false;

    [Header("플레이어의 스탯")]
    //[SerializeField] private PlayerStatController mPlayerStatController;
    //PlayerStatController 추후 개발 예정

    // 레벨 영역
    [Header("레벨, 스탯포인트")]
    [SerializeField] private TextMeshProUGUI mLvLabel; // 레벨 레이블
    [SerializeField] private TextMeshProUGUI mStatPtLabel; // 스탯포인트 레이블
    [HideInInspector] public int CurrentStatPoint { private set; get; } = 1; // 현재 스탯 포인트 (1로 시작하도록 의도)

    [Space(30)]
    [Header("스탯(현재)")]
    [SerializeField] private TextMeshProUGUI mHpCurrentLabel; // 현재 체력 레이블
    [SerializeField] private TextMeshProUGUI mMpCurrentLabel; // 현재 마나 레이블
    [SerializeField] private TextMeshProUGUI mAttackCurrentLabel; // 현재 공격 레이블
    [SerializeField] private TextMeshProUGUI mSpeedCurrentLabel; // 현재 속도 레이블
    [SerializeField] private TextMeshProUGUI mDefenseCurrentLabel; // 현재 방어력 레이블

    [Space(30)]
    [Header("스탯(최대)")]
    [SerializeField] private TextMeshProUGUI mHpMaxLabel; // 최대 체력 레이블
    [SerializeField] private TextMeshProUGUI mMpMaxLabel; // 최대 마나 레이블

    private void Awake()
    {
        // 초기화시 전역 활성화상태 해제
        StatManager.IsStatDialogEnable = false;
    }

    private void Update()
    {
        
    }

    public void LevelUp()
    {
        //mPlayerStatController.StatData.UpgradeBaseStat(StatType.LEVEL); // 레벨 1 증가
        CurrentStatPoint += 2; 
    }
 
}
