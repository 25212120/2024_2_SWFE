using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;

public class ShowUIOnEnter : MonoBehaviour
{
    public GameObject uiPanel; // UI 창을 나타낼 GameObject
    public string targetTag = "core"; // 대상 태그
    public float detectionRadius = 5.0f; // 감지 반경
    public GameObject radiusEffect; // 감지 반경을 나타낼 이펙트 오브젝트 (파티클 등)
    public GameObject[] weaponButtons; // 무기 선택 버튼들
    public GameObject[] weaponUIPanels; // 무기별 UI 패널들 (클릭 시 활성화될 UI들)
    
    public PlayerInputManager playerInputManager;
    private GameObject selectedButton; // 현재 선택된 버튼을 나타냄
    private int selectedWeaponIndex = -1; // 선택된 무기의 인덱스
    private string playerName;

    private void Awake()
    {
        StartCoroutine(initializePlayer());
    }

    IEnumerator initializePlayer()
    {
        while (GameManager.instance.player == null)
        {
            yield return null;
        }
        playerInputManager = GameManager.instance.player.GetComponent<PlayerInputManager>();
    }


    void Start()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false); // 시작 시 UI 창 비활성화
        }

        if (radiusEffect != null)
        {
            radiusEffect.SetActive(false); // 시작 시 감지 반경 이펙트 비활성화
        }

        // 모든 무기 UI 패널 비활성화
        foreach (GameObject panel in weaponUIPanels)
        {
            panel.SetActive(false);
        }

        foreach (GameObject button in weaponButtons)
        {
            Button btn = button.GetComponent<Button>();
            btn.onClick.AddListener(() => OnWeaponButtonClick(button));
        }
    }

    void Update()
    {
        if (playerInputManager != null)
        {
            GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(targetTag);
            bool isWithinRadius = false;

            foreach (GameObject target in targetObjects)
            {
                float distance = Vector3.Distance(playerInputManager.transform.position, target.transform.position);

                if (distance <= detectionRadius)
                {
                    playerInputManager.canSwap = true;
                    if (uiPanel != null)
                    {
                        uiPanel.SetActive(true); // 플레이어가 대상 오브젝트에 가까워지면 UI 창 활성화
                    }

                    if (radiusEffect != null)
                    {
                        radiusEffect.transform.position = target.transform.position; // 이펙트를 대상 위치로 이동
                        radiusEffect.SetActive(true); // 감지 반경 이펙트 활성화
                    }

                    isWithinRadius = true;
                    break; // 감지 반경에 있는 오브젝트가 발견되면 더 이상 반복하지 않음
                }
            }

            if (!isWithinRadius)
            {
                if (uiPanel != null)
                {
                    uiPanel.SetActive(false); // 플레이어가 감지 반경 내에 없으면 UI 창 비활성화
                }

                if (radiusEffect != null)
                {
                    radiusEffect.SetActive(false); // 감지 반경 이펙트 비활성화
                }

                playerInputManager.canSwap = false;

                // 모든 무기 UI 패널 비활성화
                foreach (GameObject panel in weaponUIPanels)
                {
                    panel.SetActive(false);
                }
            }
        }
    }

    private void OnWeaponButtonClick(GameObject button)
    {


        // 모든 무기 UI 패널 비활성화
        foreach (GameObject panel in weaponUIPanels)
        {
            panel.SetActive(false);
        }

        // 선택된 버튼의 인덱스 찾기
        int weaponIndex = System.Array.IndexOf(weaponButtons, button);
        if (weaponIndex >= 0 && playerInputManager != null)
        {
            

            // 해당 무기의 UI 패널 활성화
            if (weaponIndex < weaponUIPanels.Length)
            {
                weaponUIPanels[weaponIndex].SetActive(true);
            }

            
        }
    }
}
