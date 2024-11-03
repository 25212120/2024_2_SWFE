using TMPro;
using UnityEngine;

public class StatManager : Singleton<StatManager>
{
    public static bool IsStatDialogEnable { private set; get; } = false;

    [Header("�÷��̾��� ����")]
    //[SerializeField] private PlayerStatController mPlayerStatController;
    //PlayerStatController ���� ���� ����

    // ���� ����
    [Header("����, ��������Ʈ")]
    [SerializeField] private TextMeshProUGUI mLvLabel; // ���� ���̺�
    [SerializeField] private TextMeshProUGUI mStatPtLabel; // ��������Ʈ ���̺�
    [HideInInspector] public int CurrentStatPoint { private set; get; } = 1; // ���� ���� ����Ʈ (1�� �����ϵ��� �ǵ�)

    [Space(30)]
    [Header("����(����)")]
    [SerializeField] private TextMeshProUGUI mHpCurrentLabel; // ���� ü�� ���̺�
    [SerializeField] private TextMeshProUGUI mMpCurrentLabel; // ���� ���� ���̺�
    [SerializeField] private TextMeshProUGUI mAttackCurrentLabel; // ���� ���� ���̺�
    [SerializeField] private TextMeshProUGUI mSpeedCurrentLabel; // ���� �ӵ� ���̺�
    [SerializeField] private TextMeshProUGUI mDefenseCurrentLabel; // ���� ���� ���̺�

    [Space(30)]
    [Header("����(�ִ�)")]
    [SerializeField] private TextMeshProUGUI mHpMaxLabel; // �ִ� ü�� ���̺�
    [SerializeField] private TextMeshProUGUI mMpMaxLabel; // �ִ� ���� ���̺�

    private void Awake()
    {
        // �ʱ�ȭ�� ���� Ȱ��ȭ���� ����
        StatManager.IsStatDialogEnable = false;
    }

    private void Update()
    {
        
    }

    public void LevelUp()
    {
        //mPlayerStatController.StatData.UpgradeBaseStat(StatType.LEVEL); // ���� 1 ����
        CurrentStatPoint += 2; 
    }
 
}
