using UnityEngine;
using UnityEngine.UI;

public class UISwap : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel1; // ù ��° UI �г�
    [SerializeField] private GameObject uiPanel2; // �� ��° UI �г�
    [SerializeField] private Button button1; // ù ��° ��ư
    [SerializeField] private Button button2; // �� ��° ��ư

    private void Start()
    {
        // ��ư Ŭ�� �̺�Ʈ�� �޼��� ����
        button1.onClick.AddListener(() => ActivatePanel(uiPanel1, uiPanel2));
        button2.onClick.AddListener(() => ActivatePanel(uiPanel2, uiPanel1));
    }

    private void ActivatePanel(GameObject panelToActivate, GameObject panelToDeactivate)
    {
        panelToActivate.SetActive(true); // Ȱ��ȭ
        panelToDeactivate.SetActive(false); // ��Ȱ��ȭ
    }
}
