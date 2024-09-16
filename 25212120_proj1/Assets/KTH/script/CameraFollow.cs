using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // ������ ĳ������ Transform
    public Vector3 offset = new Vector3(7f, 7f, 7f); // ī�޶��� �⺻ ������
    public float smoothSpeed = 0.2f; // SmoothDamp�� smoothTime
    public float zoomSpeed = 10f; // �� ��/�ƿ� �ӵ�
    public float minFOV = 20f; // �ּ� FOV
    public float maxFOV = 60f; // �ִ� FOV
    public float cameraMoveSpeed = 0.1f; // ���콺 �̵��� ���� ī�޶� �̵� �ӵ�
    public float maxMoveSpeed = 10f; // �ִ� �̵� �ӵ�
    public float edgeThickness = 10f; // ȭ�� �����ڸ��� �β�

    private Camera cam;
    private bool isCameraLocked = true;
    private Vector3 unlockedPosition;
    private Vector3 velocity = Vector3.zero; // SmoothDamp�� ���� �ӵ� ����

    void Start()
    {
        cam = GetComponent<Camera>();
        unlockedPosition = transform.position;
    }

    void Update()
    {
        // ���콺 ��ũ�� �Է¿� ���� �� ��/�ƿ� ����
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cam.fieldOfView -= scroll * zoomSpeed;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }

        // �����̽� Ű �Է����� ī�޶� ����/���� ���
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCameraLocked = !isCameraLocked; // ���� ���¸� ���
            if (!isCameraLocked)
            {
                // ���� ���� �� ���� ��ġ�� ���� ��ġ�� ����
                unlockedPosition = transform.position;
            }
        }

        // ���� ���� ���¿��� ȭ�� �����ڸ��� ���콺�� ����� �� ī�޶� �̵�
        if (!isCameraLocked && IsMouseOnScreenEdge())
        {
            MoveCameraWithMouse();
        }
    }

    void LateUpdate()
    {
        if (isCameraLocked && target != null)
        {
            // ���� ������ �� Ÿ���� ��ġ���� �������� ������ ��ǥ ��ġ ���
            Vector3 desiredPosition = target.position + offset;

            // SmoothDamp�� ����Ͽ� �ε巴�� �̵�
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        }
        else
        {
            // ���� ���� ���¿����� unlockedPosition���� �ﰢ������ �̵�
            transform.position = unlockedPosition;
        }

        // ������ ȸ�� ����
        transform.rotation = Quaternion.Euler(30, 225, 0);
    }

    // ���� ���� ���¿��� ī�޶� �̵��� ����
    private void MoveCameraWithMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Vector3 direction = (mousePosition - screenCenter).normalized;

        // ī�޶��� �̵� �ӵ��� ���
        Vector3 move = new Vector3(direction.x, 0, direction.y) * cameraMoveSpeed * Time.deltaTime * 100f;
        Vector3 clampedMove = Vector3.ClampMagnitude(-move, maxMoveSpeed * Time.deltaTime);

        // unlockedPosition ������Ʈ
        unlockedPosition += clampedMove;
    }

    // ���콺�� ȭ�� �����ڸ��� ��Ҵ��� �����ϴ� �Լ�
    private bool IsMouseOnScreenEdge()
    {
        return Input.mousePosition.x <= edgeThickness || Input.mousePosition.x >= Screen.width - edgeThickness ||
               Input.mousePosition.y <= edgeThickness || Input.mousePosition.y >= Screen.height - edgeThickness;
    }
}
