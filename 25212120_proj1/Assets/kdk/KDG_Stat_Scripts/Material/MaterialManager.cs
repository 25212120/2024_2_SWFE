using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    // 자원 타입 정의
    public enum ResourceType
    {
        Money,       // 재화(돈)
        Wood,        // 나무
        Stone,       // 돌
        Metal,       // 금속
        Crystal,     // 크리스탈
        WoodEssence, // 에센스
        IceEssence,
        FireEssence,
        SandEssence
    }

    // 자원 보유량을 저장할 딕셔너리
    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    public static MaterialManager Instance { get; private set; }

    // 초기화
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 변경 시에도 객체 유지
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 객체를 파괴
        }

        // 자원 초기화
        foreach (ResourceType resource in System.Enum.GetValues(typeof(ResourceType)))
        {
            resources[resource] = 0;  // 초기 자원 값 0으로 설정
        }
    }


    // 자원 조회 함수
    public int GetResource(ResourceType resourceType)
    {
        return resources.ContainsKey(resourceType) ? resources[resourceType] : 0;
    }

    public void GainResource(ResourceType resourceType, int amount)
    {
        if (amount > 0)
        {
            resources[resourceType] += amount;
            Debug.Log($"{resourceType}를 {amount}만큼 획득했습니다. 현재 보유량: {resources[resourceType]}");
        }
        else
        {
            Debug.LogWarning("획득할 자원의 양은 0보다 커야 합니다.");
        }
    }

    // 자원 획득 함수 (확률 적용)
    public void GainResourceWithChance(ResourceType resourceType, int amount, float dropChance)
    {
        // 확률에 맞게 자원 획득
        if (Random.value <= dropChance)
        {
            if (amount > 0)
            {
                resources[resourceType] += amount;
                Debug.Log($"{resourceType}를 {amount}만큼 획득했습니다. 현재 보유량: {resources[resourceType]}");
            }
            else
            {
                Debug.LogWarning("획득할 자원의 양은 0보다 커야 합니다.");
            }
        }
        else
        {
            Debug.Log($"{resourceType} 획득에 실패했습니다. 확률: {dropChance * 100}%");
        }
    }

    // 자원 소모 함수
    public bool ConsumeResource(ResourceType resourceType, int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("소모할 자원의 양은 0보다 커야 합니다.");
            return false;
        }

        if (resources.ContainsKey(resourceType) && resources[resourceType] >= amount)
        {
            resources[resourceType] -= amount;
            Debug.Log($"{resourceType}를 {amount}만큼 소모했습니다. 현재 보유량: {resources[resourceType]}");
            return true;
        }
        else
        {
            Debug.LogWarning($"{resourceType}가 부족합니다. 현재 보유량: {resources[resourceType]}");
            return false;
        }
    }

    // 자원 정보 출력 (디버그용)
    public void PrintAllResources()
    {
        foreach (var resource in resources)
        {
            Debug.Log($"{resource.Key}: {resource.Value}");
        }
    }
}
