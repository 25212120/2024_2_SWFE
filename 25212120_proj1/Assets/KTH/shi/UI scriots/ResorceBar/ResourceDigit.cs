using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceDigit : MonoBehaviour
{
    public TextMeshProUGUI wood; // UI �ؽ�Ʈ ������Ʈ ����
    public TextMeshProUGUI stone; // UI �ؽ�Ʈ ������Ʈ ����
    public TextMeshProUGUI metal; // UI �ؽ�Ʈ ������Ʈ ����
    public TextMeshProUGUI crystal; // UI �ؽ�Ʈ ������Ʈ ����
    public TextMeshProUGUI essence1; // UI �ؽ�Ʈ ������Ʈ ����
    public TextMeshProUGUI essence2; // UI �ؽ�Ʈ ������Ʈ ����
    public TextMeshProUGUI essence3; // UI �ؽ�Ʈ ������Ʈ ����
    public TextMeshProUGUI essence4; // UI �ؽ�Ʈ ������Ʈ ����
    public TextMeshProUGUI money; // UI �ؽ�Ʈ ������Ʈ ����

    private MaterialManager materialManager;

    void Start()
    {
        // MaterialManager ������Ʈ ��������
        materialManager = GetComponent<MaterialManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
        // ������ �ʵ�鿡 ���ؼ��� �����ϰ� Ȯ��

        // �ڿ��� ����� �ʱ�ȭ�Ǿ��� ��쿡�� ������Ʈ ����
        if (materialManager != null && money != null && wood != null && stone != null)
        {
            // �ڿ� ���� ��������
            int MONEY = materialManager.GetResource(MaterialManager.ResourceType.Money);
            int WOOD = materialManager.GetResource(MaterialManager.ResourceType.Wood);
            int STONE = materialManager.GetResource(MaterialManager.ResourceType.Stone);
            int METAL = materialManager.GetResource(MaterialManager.ResourceType.Metal);
            int CRYSTAL = materialManager.GetResource(MaterialManager.ResourceType.Crystal);
            int WE = materialManager.GetResource(MaterialManager.ResourceType.WoodEssence);
            int IE = materialManager.GetResource(MaterialManager.ResourceType.IceEssence);
            int FE = materialManager.GetResource(MaterialManager.ResourceType.FireEssence);
            int SE = materialManager.GetResource(MaterialManager.ResourceType.SandEssence);

            // UI ������Ʈ
            money.text = MONEY.ToString();
            wood.text = WOOD.ToString();
            stone.text = STONE.ToString();
            metal.text = METAL.ToString();
            crystal.text = CRYSTAL.ToString();
            essence1.text = WE.ToString();
            essence2.text = IE.ToString();
            essence3.text = FE.ToString();
            essence4.text = SE.ToString();
        }

        
    }

}
