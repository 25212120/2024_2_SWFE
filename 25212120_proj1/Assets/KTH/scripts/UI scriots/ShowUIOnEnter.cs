using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowUIOnEnter : MonoBehaviour
{
    public GameObject uiPanel; // UI â�� ��Ÿ�� GameObject
    public string targetTag = "Target"; // ��� �±�
    public float detectionRadius = 5.0f; // ���� �ݰ�
    public GameObject radiusEffect; // ���� �ݰ��� ��Ÿ�� ����Ʈ ������Ʈ (��ƼŬ ��)
    public GameObject[] weaponButtons; // ���� ���� ��ư��
    public GameObject[] weaponUIPanels; // ���⺰ UI �гε� (Ŭ�� �� Ȱ��ȭ�� UI��)
    
    private GameObject player; // �÷��̾� ������Ʈ

    private PlayerInputManager playerInputManager;
    private GameObject selectedButton; // ���� ���õ� ��ư�� ��Ÿ��
    private int selectedWeaponIndex = -1; // ���õ� ������ �ε���

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // �÷��̾ ã��
        playerInputManager = player.GetComponent<PlayerInputManager>();

        if (uiPanel != null)
        {
            uiPanel.SetActive(false); // ���� �� UI â ��Ȱ��ȭ
        }

        if (radiusEffect != null)
        {
            radiusEffect.SetActive(false); // ���� �� ���� �ݰ� ����Ʈ ��Ȱ��ȭ
        }

        

        // ��� ���� UI �г� ��Ȱ��ȭ
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
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(targetTag);
        bool isWithinRadius = false;

        foreach (GameObject target in targetObjects)
        {
            float distance = Vector3.Distance(player.transform.position, target.transform.position);

            if (distance <= detectionRadius)
            {
                if (uiPanel != null)
                {
                    uiPanel.SetActive(true); // �÷��̾ ��� ������Ʈ�� ��������� UI â Ȱ��ȭ
                }

                if (radiusEffect != null)
                {
                    radiusEffect.transform.position = target.transform.position; // ����Ʈ�� ��� ��ġ�� �̵�
                    radiusEffect.SetActive(true); // ���� �ݰ� ����Ʈ Ȱ��ȭ
                }

                isWithinRadius = true;
                break; // ���� �ݰ濡 �ִ� ������Ʈ�� �߰ߵǸ� �� �̻� �ݺ����� ����
            }
        }

        if (!isWithinRadius)
        {
            if (uiPanel != null)
            {
                uiPanel.SetActive(false); // �÷��̾ ���� �ݰ� ���� ������ UI â ��Ȱ��ȭ
            }

            if (radiusEffect != null)
            {
                radiusEffect.SetActive(false); // ���� �ݰ� ����Ʈ ��Ȱ��ȭ
            }

            

            // ��� ���� UI �г� ��Ȱ��ȭ
            foreach (GameObject panel in weaponUIPanels)
            {
                panel.SetActive(false);
            }
        }
    }

    private void OnWeaponButtonClick(GameObject button)
    {


        // ��� ���� UI �г� ��Ȱ��ȭ
        foreach (GameObject panel in weaponUIPanels)
        {
            panel.SetActive(false);
        }

        // ���õ� ��ư�� �ε��� ã��
        int weaponIndex = System.Array.IndexOf(weaponButtons, button);
        if (weaponIndex >= 0 && playerInputManager != null)
        {
            

            // �ش� ������ UI �г� Ȱ��ȭ
            if (weaponIndex < weaponUIPanels.Length)
            {
                weaponUIPanels[weaponIndex].SetActive(true);
            }

            
        }
    }
}
