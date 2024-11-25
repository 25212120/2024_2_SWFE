using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceDigit : MonoBehaviour
{
    public TextMeshProUGUI wood; // UI 텍스트 컴포넌트 연결
    public TextMeshProUGUI stone; // UI 텍스트 컴포넌트 연결
    public TextMeshProUGUI metal; // UI 텍스트 컴포넌트 연결
    public TextMeshProUGUI crystal; // UI 텍스트 컴포넌트 연결
    public TextMeshProUGUI essence1; // UI 텍스트 컴포넌트 연결
    public TextMeshProUGUI essence2; // UI 텍스트 컴포넌트 연결
    public TextMeshProUGUI essence3; // UI 텍스트 컴포넌트 연결
    public TextMeshProUGUI essence4; // UI 텍스트 컴포넌트 연결
    public TextMeshProUGUI money; // UI 텍스트 컴포넌트 연결

    private MaterialManager materialManager;

    void Start()
    {
        // MaterialManager 컴포넌트 가져오기
        materialManager = GetComponent<MaterialManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
        // 나머지 필드들에 대해서도 동일하게 확인

        // 자원이 제대로 초기화되었을 경우에만 업데이트 수행
        if (materialManager != null && money != null && wood != null && stone != null)
        {
            // 자원 정보 가져오기
            int MONEY = materialManager.GetResource(MaterialManager.ResourceType.Money);
            int WOOD = materialManager.GetResource(MaterialManager.ResourceType.Wood);
            int STONE = materialManager.GetResource(MaterialManager.ResourceType.Stone);
            int METAL = materialManager.GetResource(MaterialManager.ResourceType.Metal);
            int CRYSTAL = materialManager.GetResource(MaterialManager.ResourceType.Crystal);
            int WE = materialManager.GetResource(MaterialManager.ResourceType.WoodEssence);
            int IE = materialManager.GetResource(MaterialManager.ResourceType.IceEssence);
            int FE = materialManager.GetResource(MaterialManager.ResourceType.FireEssence);
            int SE = materialManager.GetResource(MaterialManager.ResourceType.SandEssence);

            // UI 업데이트
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
