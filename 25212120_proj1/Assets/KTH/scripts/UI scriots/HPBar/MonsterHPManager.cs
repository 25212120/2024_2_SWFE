using System.Collections.Generic;
using UnityEngine;

public class MonsterHPManager : MonoBehaviour
{
    // 몬스터들을 관리하기 위한 리스트
    [SerializeField] // 인스펙터에서 리스트 확인 가능
    private List<Monster_1> monsters = new List<Monster_1>();

    void Start()
    {
        // 씬에 있는 모든 몬스터_1을 찾고 리스트에 추가합니다.
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
        // 각 몬스터의 현재 체력을 확인하거나 조작할 수 있습니다.
        for (int i = monsters.Count - 1; i >= 0; i--)
        {
            if (monsters[i] == null)
            {
                // 리스트에서 제거된 오브젝트 정리
                monsters.RemoveAt(i);
                continue;
            }

            if (monsters[i].GetCurrentHP() <= 0)
            {
                Debug.Log($"Monster {monsters[i].name} is dead and will be removed from the list.");
                monsters.RemoveAt(i); // 몬스터의 체력이 0 이하이면 리스트에서 제거
            }
        }
    }

    // 특정 몬스터의 HP를 불러오는 함수
    public float GetMonsterHP(Monster_1 monster)
    {
        if (monsters.Contains(monster))
        {
            return monster.GetCurrentHP();
        }
        else
        {
            Debug.LogWarning("해당 몬스터는 리스트에 존재하지 않습니다.");
            return -1; // 오류 시 -1 반환
        }
    }
}
