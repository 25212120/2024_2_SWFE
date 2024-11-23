using UnityEngine;
public class SpawnStructure : BaseStructure
{
    public Unit_SpawnManager unitSpawnManager;

    void Start()
    {
        // Unit_SpawnManager�� ����� �Ҵ�Ǿ����� Ȯ��
        unitSpawnManager = GetComponent<Unit_SpawnManager>();

        if (unitSpawnManager == null)
        {
            Debug.LogError("Unit_SpawnManager�� �� Ÿ���� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }
}
