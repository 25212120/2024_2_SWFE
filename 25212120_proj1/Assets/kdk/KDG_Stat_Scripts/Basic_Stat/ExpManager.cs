using UnityEngine;

public class ExpManager : Singleton<ExpManager>
{
    [SerializeField] private PlayerStat playerStat; // PlayerStat �ν��Ͻ� ����
    [SerializeField] private EquipmentInventory equipmentInventory; // EquipmentInventory ����

    /// �÷��̾��� ���� ����ġ
    public float ExpCurrent { private set; get; }

    /// �÷��̾��� ���� �ִ� ����ġ
    public float ExpMax { private set; get; } = 100;

    // ����ġ�� �߰��ϴ� �Լ�
    public void AddExp(float amount)
    {
        // ���� ����ġ�� �����Ͽ�, �󸶳� ����ġ�� �ö����� ����׿� ǥ���� �� �ֵ��� ��
        float expPrev = ExpCurrent;
        ExpCurrent += amount;

        // ����ġ�� �ö��� ��, �󸶳� �ö����� ����� ���
        Debug.Log($"����ġ�� {amount}��ŭ �ö����ϴ�. ���� ����ġ: {ExpCurrent}/{ExpMax}");

        // ����ġ�� �ִ�ġ �̻����� �������� ��
        if (ExpCurrent >= ExpMax)
        {
            // ������ ó��
            while (ExpCurrent >= ExpMax)
            {
                ExpCurrent -= ExpMax;  // ���� ����ġ�� ���� �ִ� ����ġ���� ���� ������ ����
                ExpMax *= 2.0f;        // �ִ� ����ġ�� 2��� ����
                playerStat.LevelUp();  // ������ �Լ� ȣ��

                // ������ ���� �� ����� �޽��� ���
                int newLevel = playerStat.GetLevel();  // playerStat���� ������ ������
                Debug.Log($"������! ���ο� ����: {newLevel}");  // ������ �� ���ο� ������ ����׿� ���
            }
        }
    }
}
