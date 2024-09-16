using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator animator;
    private Rigidbody rb;
    public Vector3 movement;

    private PlayerState currentState;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        SetState(new IdleState(this));
    }

    void Update()
    {
        HandleInput(); // 입력 처리 메서드 호출
        currentState.Update();
    }

    void FixedUpdate()
    {
        // 이동 처리 (Rigidbody의 MovePosition 사용)
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void HandleInput()
    {
        movement = Vector3.zero; // 초기화

        // A: 왼쪽 이동
        if (Input.GetKey(KeyCode.A))
        {
            movement = new Vector3(1, 0, -1); // 왼쪽 이동 벡터
            transform.rotation = Quaternion.Euler(0, 135, 0); // Y축 45도 회전
        }
        // D: 오른쪽 이동
        else if (Input.GetKey(KeyCode.D))
        {
            movement = new Vector3(-1, 0, 1); // 오른쪽 이동 벡터
            transform.rotation = Quaternion.Euler(0, 315, 0); // Y축 225도 회전
        }
        // W: 위쪽 이동
        else if (Input.GetKey(KeyCode.W))
        {
            movement = new Vector3(-1, 0, -1); // 위쪽 이동 벡터
            transform.rotation = Quaternion.Euler(0, 225, 0); // Y축 315도 회전
        }
        // S: 아래쪽 이동
        else if (Input.GetKey(KeyCode.S))
        {
            movement = new Vector3(1, 0, 1); // 아래쪽 이동 벡터
            transform.rotation = Quaternion.Euler(0, 45, 0); // Y축 135도 회전
        }
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
