using TMPro;
using UnityEngine;

public class StatManager : Singleton<StatManager>
{
    public static bool IsStatDialogEnable { private set; get; } = false;

    [Header("�÷��̾��� ����")]
    [SerializeField] private PlayerStat mPlayerStat;
    //PlayerStatController ���� ���� ����

    // ���� ����
    [Header("����, ��������Ʈ")]
    [SerializeField] private TextMeshProUGUI mLvLabel; // ���� ���̺�
    
    [Space(30)]
    [Header("����(����)")]
    [SerializeField] private TextMeshProUGUI mHpCurrentLabel; // ���� ü�� ���̺�
    [SerializeField] private TextMeshProUGUI mAttackCurrentLabel; // ���� ���� ���̺�
    [SerializeField] private TextMeshProUGUI mSpeedCurrentLabel; // ���� �ӵ� ���̺�
    [SerializeField] private TextMeshProUGUI mDefenseCurrentLabel; // ���� ���� ���̺�

    [Space(30)]
    [Header("����(�ִ�)")]
    [SerializeField] private TextMeshProUGUI mHpMaxLabel; // �ִ� ü�� ���̺�
    
    

    private void Update()
    {
        
    }


}
