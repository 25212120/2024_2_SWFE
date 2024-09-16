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
        HandleInput(); // �Է� ó�� �޼��� ȣ��
        currentState.Update();
    }

    void FixedUpdate()
    {
        // �̵� ó�� (Rigidbody�� MovePosition ���)
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void HandleInput()
    {
        movement = Vector3.zero; // �ʱ�ȭ

        // A: ���� �̵�
        if (Input.GetKey(KeyCode.A))
        {
            movement = new Vector3(1, 0, -1); // ���� �̵� ����
            transform.rotation = Quaternion.Euler(0, 135, 0); // Y�� 45�� ȸ��
        }
        // D: ������ �̵�
        else if (Input.GetKey(KeyCode.D))
        {
            movement = new Vector3(-1, 0, 1); // ������ �̵� ����
            transform.rotation = Quaternion.Euler(0, 315, 0); // Y�� 225�� ȸ��
        }
        // W: ���� �̵�
        else if (Input.GetKey(KeyCode.W))
        {
            movement = new Vector3(-1, 0, -1); // ���� �̵� ����
            transform.rotation = Quaternion.Euler(0, 225, 0); // Y�� 315�� ȸ��
        }
        // S: �Ʒ��� �̵�
        else if (Input.GetKey(KeyCode.S))
        {
            movement = new Vector3(1, 0, 1); // �Ʒ��� �̵� ����
            transform.rotation = Quaternion.Euler(0, 45, 0); // Y�� 135�� ȸ��
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
