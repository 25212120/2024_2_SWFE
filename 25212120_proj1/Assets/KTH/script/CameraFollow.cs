using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 추적할 캐릭터의 Transform
    public Vector3 offset = new Vector3(7f, 7f, 7f); // 카메라의 기본 오프셋
    public float smoothSpeed = 0.2f; // SmoothDamp의 smoothTime
    public float zoomSpeed = 10f; // 줌 인/아웃 속도
    public float minFOV = 20f; // 최소 FOV
    public float maxFOV = 60f; // 최대 FOV
    public float cameraMoveSpeed = 0.1f; // 마우스 이동에 따른 카메라 이동 속도
    public float maxMoveSpeed = 10f; // 최대 이동 속도
    public float edgeThickness = 10f; // 화면 가장자리의 두께

    private Camera cam;
    private bool isCameraLocked = true;
    private Vector3 unlockedPosition;
    private Vector3 velocity = Vector3.zero; // SmoothDamp를 위한 속도 변수

    void Start()
    {
        cam = GetComponent<Camera>();
        unlockedPosition = transform.position;
    }

    void Update()
    {
        // 마우스 스크롤 입력에 따라 줌 인/아웃 조절
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cam.fieldOfView -= scroll * zoomSpeed;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }

        // 스페이스 키 입력으로 카메라 고정/해제 토글
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCameraLocked = !isCameraLocked; // 고정 상태를 토글
            if (!isCameraLocked)
            {
                // 고정 해제 시 현재 위치를 기준 위치로 설정
                unlockedPosition = transform.position;
            }
        }

        // 고정 해제 상태에서 화면 가장자리에 마우스가 닿았을 때 카메라 이동
        if (!isCameraLocked && IsMouseOnScreenEdge())
        {
            MoveCameraWithMouse();
        }
    }

    void LateUpdate()
    {
        if (isCameraLocked && target != null)
        {
            // 고정 상태일 때 타겟의 위치에서 오프셋을 적용한 목표 위치 계산
            Vector3 desiredPosition = target.position + offset;

            // SmoothDamp를 사용하여 부드럽게 이동
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        }
        else
        {
            // 고정 해제 상태에서는 unlockedPosition으로 즉각적으로 이동
            transform.position = unlockedPosition;
        }

        // 고정된 회전 유지
        transform.rotation = Quaternion.Euler(30, 225, 0);
    }

    // 고정 해제 상태에서 카메라 이동을 적용
    private void MoveCameraWithMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Vector3 direction = (mousePosition - screenCenter).normalized;

        // 카메라의 이동 속도를 계산
        Vector3 move = new Vector3(direction.x, 0, direction.y) * cameraMoveSpeed * Time.deltaTime * 100f;
        Vector3 clampedMove = Vector3.ClampMagnitude(-move, maxMoveSpeed * Time.deltaTime);

        // unlockedPosition 업데이트
        unlockedPosition += clampedMove;
    }

    // 마우스가 화면 가장자리에 닿았는지 감지하는 함수
    private bool IsMouseOnScreenEdge()
    {
        return Input.mousePosition.x <= edgeThickness || Input.mousePosition.x >= Screen.width - edgeThickness ||
               Input.mousePosition.y <= edgeThickness || Input.mousePosition.y >= Screen.height - edgeThickness;
    }
}
