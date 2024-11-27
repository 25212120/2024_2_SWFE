using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    // �ڿ� Ÿ�� ����
    public enum ResourceType
    {
        Money,       // ��ȭ(��)
        Wood,        // ����
        Stone,       // ��
        Metal,       // �ݼ�
        Crystal,     // ũ����Ż
        WoodEssence, // ������
        IceEssence,
        FireEssence,
        SandEssence
    }

    // �ڿ� �������� ������ ��ųʸ�
    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    public static MaterialManager Instance { get; private set; }

    // �ʱ�ȭ
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ���� �ÿ��� ��ü ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� ��ü�� �ı�
        }

        // �ڿ� �ʱ�ȭ
        foreach (ResourceType resource in System.Enum.GetValues(typeof(ResourceType)))
        {
            resources[resource] = 0;  // �ʱ� �ڿ� �� 0���� ����
        }
    }


    // �ڿ� ��ȸ �Լ�
    public int GetResource(ResourceType resourceType)
    {
        return resources.ContainsKey(resourceType) ? resources[resourceType] : 0;
    }

    public void GainResource(ResourceType resourceType, int amount)
    {
        if (amount > 0)
        {
            resources[resourceType] += amount;
            Debug.Log($"{resourceType}�� {amount}��ŭ ȹ���߽��ϴ�. ���� ������: {resources[resourceType]}");
        }
        else
        {
            Debug.LogWarning("ȹ���� �ڿ��� ���� 0���� Ŀ�� �մϴ�.");
        }
    }

    // �ڿ� ȹ�� �Լ� (Ȯ�� ����)
    public void GainResourceWithChance(ResourceType resourceType, int amount, float dropChance)
    {
        // Ȯ���� �°� �ڿ� ȹ��
        if (Random.value <= dropChance)
        {
            if (amount > 0)
            {
                resources[resourceType] += amount;
                Debug.Log($"{resourceType}�� {amount}��ŭ ȹ���߽��ϴ�. ���� ������: {resources[resourceType]}");
            }
            else
            {
                Debug.LogWarning("ȹ���� �ڿ��� ���� 0���� Ŀ�� �մϴ�.");
            }
        }
        else
        {
            Debug.Log($"{resourceType} ȹ�濡 �����߽��ϴ�. Ȯ��: {dropChance * 100}%");
        }
    }

    // �ڿ� �Ҹ� �Լ�
    public bool ConsumeResource(ResourceType resourceType, int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("�Ҹ��� �ڿ��� ���� 0���� Ŀ�� �մϴ�.");
            return false;
        }

        if (resources.ContainsKey(resourceType) && resources[resourceType] >= amount)
        {
            resources[resourceType] -= amount;
            Debug.Log($"{resourceType}�� {amount}��ŭ �Ҹ��߽��ϴ�. ���� ������: {resources[resourceType]}");
            return true;
        }
        else
        {
            Debug.LogWarning($"{resourceType}�� �����մϴ�. ���� ������: {resources[resourceType]}");
            return false;
        }
    }

    // �ڿ� ���� ��� (����׿�)
    public void PrintAllResources()
    {
        foreach (var resource in resources)
        {
            Debug.Log($"{resource.Key}: {resource.Value}");
        }
    }
}
