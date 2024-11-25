using System.Collections.Generic;
using UnityEngine;

public class MonsterHPManager : MonoBehaviour
{
    // ���͵��� �����ϱ� ���� ����Ʈ
    [SerializeField] // �ν����Ϳ��� ����Ʈ Ȯ�� ����
    private List<Monster_1> monsters = new List<Monster_1>();

    void Start()
    {
        // ���� �ִ� ��� ����_1�� ã�� ����Ʈ�� �߰��մϴ�.
        Monster_1[] foundMonsters = FindObjectsOfType<Monster_1>();
        foreach (Monster_1 monster in foundMonsters)
        {
            if (!monsters.Contains(monster))
            {
                monsters.Add(monster);
            }
        }
    }

    void Update()
    {
        // �� ������ ���� ü���� Ȯ���ϰų� ������ �� �ֽ��ϴ�.
        for (int i = monsters.Count - 1; i >= 0; i--)
        {
            if (monsters[i] == null)
            {
                // ����Ʈ���� ���ŵ� ������Ʈ ����
                monsters.RemoveAt(i);
                continue;
            }

            if (monsters[i].GetCurrentHP() <= 0)
            {
                Debug.Log($"Monster {monsters[i].name} is dead and will be removed from the list.");
                monsters.RemoveAt(i); // ������ ü���� 0 �����̸� ����Ʈ���� ����
            }
        }
    }

    // Ư�� ������ HP�� �ҷ����� �Լ�
    public float GetMonsterHP(Monster_1 monster)
    {
        if (monsters.Contains(monster))
        {
            return monster.GetCurrentHP();
        }
        else
        {
            Debug.LogWarning("�ش� ���ʹ� ����Ʈ�� �������� �ʽ��ϴ�.");
            return -1; // ���� �� -1 ��ȯ
        }
    }
}
