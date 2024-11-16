using UnityEngine;

public class ExpManager : Singleton<ExpManager>
{
    [SerializeField] private PlayerStat playerStat; // PlayerStat 인스턴스 참조
    [SerializeField] private EquipmentInventory equipmentInventory; // EquipmentInventory 참조

    /// 플레이어의 현재 경험치
    public float ExpCurrent { private set; get; }

    /// 플레이어의 현재 최대 경험치
    public float ExpMax { private set; get; } = 100;

    // 경험치를 추가하는 함수
    public void AddExp(float amount)
    {
        // 이전 경험치를 저장하여, 얼마나 경험치가 올랐는지 디버그에 표시할 수 있도록 함
        float expPrev = ExpCurrent;
        ExpCurrent += amount;

        // 경험치가 올랐을 때, 얼마나 올랐는지 디버그 출력
        Debug.Log($"경험치가 {amount}만큼 올랐습니다. 현재 경험치: {ExpCurrent}/{ExpMax}");

        // 경험치가 최대치 이상으로 증가했을 때
        if (ExpCurrent >= ExpMax)
        {
            // 레벨업 처리
            while (ExpCurrent >= ExpMax)
            {
                ExpCurrent -= ExpMax;  // 남은 경험치를 현재 최대 경험치보다 작은 값으로 만듦
                ExpMax *= 2.0f;        // 최대 경험치를 2배로 증가
                playerStat.LevelUp();  // 레벨업 함수 호출

                // 레벨업 했을 때 디버그 메시지 출력
                int newLevel = playerStat.GetLevel();  // playerStat에서 레벨을 가져옴
                Debug.Log($"레벨업! 새로운 레벨: {newLevel}");  // 레벨업 시 새로운 레벨을 디버그에 출력
            }
        }
    }
}
