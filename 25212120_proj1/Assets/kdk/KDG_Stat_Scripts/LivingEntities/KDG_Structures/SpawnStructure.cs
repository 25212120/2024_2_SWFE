using UnityEngine;
public class SpawnStructure : BaseStructure
{
    public Unit_SpawnManager unitSpawnManager;

    void Start()
    {
        // Unit_SpawnManager가 제대로 할당되었는지 확인
        unitSpawnManager = GetComponent<Unit_SpawnManager>();

        if (unitSpawnManager == null)
        {
            Debug.LogError("Unit_SpawnManager가 이 타워에 할당되지 않았습니다.");
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }
}
