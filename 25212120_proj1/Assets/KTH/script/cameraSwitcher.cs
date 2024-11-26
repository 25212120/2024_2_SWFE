using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public Transform target; // ĳ������ Transform
    public float moveSpeed = 10f; // ī�޶� �̵� �ӵ� (ž�� ��忡��)
    public float edgeThickness = 10f; // ȭ�� �����ڸ� �β� (ž�� ��忡�� ī�޶� �̵��� ����)
    public CinemachineVirtualCamera shoulderCam; // 3��Ī ������ Cinemachine ī�޶�
    private Camera topViewCam; // �⺻ ī�޶� (ž���)
    public Vector3 _height; // ī�޶��� ���� ����
    public float scrollSpeed = 2f; // ���콺 ��ũ�� �ӵ�
    public bool isTopView = true; // ���� ž�� �������� ����
    public HighlightArea highlightArea; // ���̶���Ʈ ������ �����ϴ� ����

    void Start()
    {
        // �⺻ ī�޶� ������
        topViewCam = GetComponent<Camera>();

        // ī�޶� Orthographic ���� ����
        topViewCam.orthographic = true;
        topViewCam.orthographicSize = 10f; // ī�޶��� �� ũ�� (���ϴ� ������ ���� ����)

        // ž�� ��� ���� �� �⺻ ī�޶� ��ġ ����
        _height = target.position + new Vector3(0, 30, 0); // �⺻ ���̸� ����
        transform.position = _height;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // ����� ��Ȱ��ȭ, ž�� Ȱ��ȭ
        shoulderCam.gameObject.SetActive(false);
        topViewCam.enabled = true;
        // ���̶���Ʈ ���� ��Ȱ��ȭ (�ʱ� ����)
        if (highlightArea != null)
        {
            highlightArea.gameObject.SetActive(false); // �ʱ� ���¿��� ���̶���Ʈ ��Ȱ��ȭ
        }
    }


    void Update()
    {
        // �����̽��ٸ� ���� ��� ��ȯ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleCameraMode();
        }

        // ž�� ��忡�� ���콺 ��ũ�ѷ� ���� ����
        if (isTopView)
        {
            MoveCameraWithMouseEdge(); // ���콺�� �����ڸ� �̵�
            MouseScroll(); // ���콺 ��ũ�ѿ� ���� ī�޶� ���� ����
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleHighlightArea();
        }
    }
    void ToggleHighlightArea()
    {
        if (highlightArea != null)
        {
            bool isActive = highlightArea.gameObject.activeSelf;
            if (isActive)
            {
                highlightArea.DeactivateHighlightArea();
                highlightArea.gameObject.SetActive(false);
            }

            else
            {
                highlightArea.ActivateHighlightArea();
                highlightArea.gameObject.SetActive(true);
            }
            
        }
    }


    // ī�޶� ��� ��ȯ
    void ToggleCameraMode()
    {
        isTopView = !isTopView; // ž��/����� ��ȯ

        if (isTopView)
        {
            // ž�� ��� Ȱ��ȭ
            transform.rotation = Quaternion.Euler(90f, 0f, 0f); // ž�� ����
            transform.position = _height; // ������ ������ ���̷� ����
            shoulderCam.gameObject.SetActive(false); // ����� ��Ȱ��ȭ
            topViewCam.enabled = true; // ž�� ī�޶� Ȱ��ȭ
        }
        else
        {
            // ����� ��� Ȱ��ȭ
            shoulderCam.gameObject.SetActive(true); // ����� Ȱ��ȭ
            topViewCam.enabled = false; // ž�� ī�޶� ��Ȱ��ȭ
        }
    }

    // ž�� ��忡�� ���콺�� ȭ�� �����ڸ��� ������ ī�޶� �̵�
    void MoveCameraWithMouseEdge()
    {
        Vector3 moveDirection = Vector3.zero;

        // ȭ�� �����ڸ��� ���콺�� ���� �� �̵� ���� ����
        if (Input.mousePosition.x <= edgeThickness)
        {
            moveDirection.x = -1; // �������� �̵�
        }
        else if (Input.mousePosition.x >= Screen.width - edgeThickness)
        {
            moveDirection.x = 1; // ���������� �̵�
        }

        if (Input.mousePosition.y <= edgeThickness)
        {
            moveDirection.z = -1; // �Ʒ��� �̵�
        }
        else if (Input.mousePosition.y >= Screen.height - edgeThickness)
        {
            moveDirection.z = 1; // ���� �̵�
        }

        // ī�޶� �̵� (ž�� ��忡��)
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.z) * moveSpeed * Time.deltaTime;
    }

    // ���콺 ��ũ�ѿ� ���� ī�޶� ���� ����
    void MouseScroll()
    {
        // ���콺 �� �Է� �� (���� ��ũ���� ���, �Ʒ��� ��ũ���� ����)
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // ��ũ�� �Է��� ���� ��� ī�޶��� ����(_height)�� ����
        if (scrollInput != 0f)
        {
            _height.y += scrollInput * -scrollSpeed; // ��ũ�� �Է¿� ���� ���̸� ����
            _height.y = Mathf.Clamp(_height.y, 5f, 100f); // �ּ�/�ִ� ���̸� ����

            // ī�޶� ��ġ�� ������Ʈ
            transform.position = new Vector3(transform.position.x, _height.y, transform.position.z);
        }
    }
}
