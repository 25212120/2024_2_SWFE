using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public Transform target; // 캐릭터의 Transform
    public float moveSpeed = 10f; // 카메라 이동 속도 (탑뷰 모드에서)
    public float edgeThickness = 10f; // 화면 가장자리 두께 (탑뷰 모드에서 카메라 이동을 위한)
    public CinemachineVirtualCamera shoulderCam; // 3인칭 숄더뷰용 Cinemachine 카메라
    private Camera topViewCam; // 기본 카메라 (탑뷰용)
    public Vector3 _height; // 카메라의 현재 높이
    public float scrollSpeed = 2f; // 마우스 스크롤 속도
    public bool isTopView = true; // 현재 탑뷰 상태인지 여부
    public HighlightArea highlightArea; // 하이라이트 영역을 참조하는 변수

    void Start()
    {
        // 기본 카메라를 가져옴
        topViewCam = GetComponent<Camera>();

        // 카메라를 Orthographic 모드로 설정
        topViewCam.orthographic = true;
        topViewCam.orthographicSize = 10f; // 카메라의 줌 크기 (원하는 값으로 조절 가능)

        // 탑뷰 모드 시작 시 기본 카메라 위치 설정
        _height = target.position + new Vector3(0, 30, 0); // 기본 높이를 설정
        transform.position = _height;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // 숄더뷰 비활성화, 탑뷰 활성화
        shoulderCam.gameObject.SetActive(false);
        topViewCam.enabled = true;
        // 하이라이트 영역 비활성화 (초기 상태)
        if (highlightArea != null)
        {
            highlightArea.gameObject.SetActive(false); // 초기 상태에서 하이라이트 비활성화
        }
    }


    void Update()
    {
        // 스페이스바를 눌러 모드 전환
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleCameraMode();
        }

        // 탑뷰 모드에서 마우스 스크롤로 높이 변경
        if (isTopView)
        {
            MoveCameraWithMouseEdge(); // 마우스로 가장자리 이동
            MouseScroll(); // 마우스 스크롤에 따른 카메라 높이 변경
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


    // 카메라 모드 전환
    void ToggleCameraMode()
    {
        isTopView = !isTopView; // 탑뷰/숄더뷰 전환

        if (isTopView)
        {
            // 탑뷰 모드 활성화
            transform.rotation = Quaternion.Euler(90f, 0f, 0f); // 탑뷰 각도
            transform.position = _height; // 이전에 설정된 높이로 복귀
            shoulderCam.gameObject.SetActive(false); // 숄더뷰 비활성화
            topViewCam.enabled = true; // 탑뷰 카메라 활성화
        }
        else
        {
            // 숄더뷰 모드 활성화
            shoulderCam.gameObject.SetActive(true); // 숄더뷰 활성화
            topViewCam.enabled = false; // 탑뷰 카메라 비활성화
        }
    }

    // 탑뷰 모드에서 마우스가 화면 가장자리에 닿으면 카메라 이동
    void MoveCameraWithMouseEdge()
    {
        Vector3 moveDirection = Vector3.zero;

        // 화면 가장자리에 마우스가 있을 때 이동 방향 설정
        if (Input.mousePosition.x <= edgeThickness)
        {
            moveDirection.x = -1; // 왼쪽으로 이동
        }
        else if (Input.mousePosition.x >= Screen.width - edgeThickness)
        {
            moveDirection.x = 1; // 오른쪽으로 이동
        }

        if (Input.mousePosition.y <= edgeThickness)
        {
            moveDirection.z = -1; // 아래로 이동
        }
        else if (Input.mousePosition.y >= Screen.height - edgeThickness)
        {
            moveDirection.z = 1; // 위로 이동
        }

        // 카메라 이동 (탑뷰 모드에서)
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.z) * moveSpeed * Time.deltaTime;
    }

    // 마우스 스크롤에 따른 카메라 높이 변경
    void MouseScroll()
    {
        // 마우스 휠 입력 값 (위쪽 스크롤은 양수, 아래쪽 스크롤은 음수)
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // 스크롤 입력이 있을 경우 카메라의 높이(_height)를 변경
        if (scrollInput != 0f)
        {
            _height.y += scrollInput * -scrollSpeed; // 스크롤 입력에 따라 높이를 변경
            _height.y = Mathf.Clamp(_height.y, 5f, 100f); // 최소/최대 높이를 제한

            // 카메라 위치를 업데이트
            transform.position = new Vector3(transform.position.x, _height.y, transform.position.z);
        }
    }
}
