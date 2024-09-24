using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 2000f; // 회전 속도
    public Animator animator;
    private Rigidbody rb;
    public Vector3 movement;
    private Quaternion targetRotation; // 목표 회전
    private bool isMoving = false; // 이동 여부 체크

    private PlayerState currentState;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        targetRotation = transform.rotation; // 현재 회전을 초기 목표 회전으로 설정
        SetState(new IdleState(this));
    }

    void Update()
    {
        HandleInput(); // 입력 처리 메서드 호출
        currentState.Update();

        // 이동 애니메이션 속도 설정
        animator.SetFloat("Speed", movement.magnitude);

        // 이동 중일 때만 회전하도록 처리
        if (isMoving)
        {
            RotateTowardsTarget();
        }
    }

    void FixedUpdate()
    {
        // 이동 처리
        if (isMoving)
        {
            MovePlayer();
        }
    }

    void HandleInput()
    {
        movement = Vector3.zero; // 초기화
        isMoving = false; // 기본값으로 이동 아님

        // 입력에 따라 이동 및 목표 회전 설정
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) // 좌
        {
            movement = new Vector3(1, 0, 0); // 왼쪽 아래 대각선
            SetTargetRotation(Quaternion.Euler(0, 90, 0));
        }
        else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) // 우 상
        {
            movement = new Vector3(0, 0, 1); // 오른쪽 아래 대각선
            SetTargetRotation(Quaternion.Euler(0, 0, 0));
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) // 우 상
        {
            movement = new Vector3(-1, 0, 0); // 오른쪽 위 대각선
            SetTargetRotation(Quaternion.Euler(0, 270, 0));
        }
        else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W)) // 좌 상
        {
            movement = new Vector3(0, 0, -1); // 왼쪽 위 대각선
            SetTargetRotation(Quaternion.Euler(0, 180, 0));
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movement = new Vector3(1, 0, -1); // 왼쪽 이동 벡터
            SetTargetRotation(Quaternion.Euler(0, 135, 0));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movement = new Vector3(1, 0, 1); // 아래쪽 이동 벡터
            SetTargetRotation(Quaternion.Euler(0, 45, 0));
        }
        else if (Input.GetKey(KeyCode.W))
        {
            movement = new Vector3(-1, 0, -1); // 위쪽 이동 벡터
            SetTargetRotation(Quaternion.Euler(0, 225, 0));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            movement = new Vector3(-1, 0, 1); // 오른쪽 이동 벡터
            SetTargetRotation(Quaternion.Euler(0, 315, 0));
        }
    }

    private void SetTargetRotation(Quaternion newRotation)
    {
        targetRotation = newRotation;
        isMoving = true; // 회전 설정 후 즉시 이동 시작
    }

    private void MovePlayer()
    {
        // 이동 처리 (Rigidbody의 MovePosition 사용)
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    private void RotateTowardsTarget()
    {
        // 현재 회전에서 목표 회전까지 부드럽게 회전
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void SetState(PlayerState state)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = state;
        currentState.Enter();
    }
}
