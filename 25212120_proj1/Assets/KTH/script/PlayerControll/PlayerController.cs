using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 2000f; // ȸ�� �ӵ�
    public Animator animator;
    private Rigidbody rb;
    public Vector3 movement;
    private Quaternion targetRotation; // ��ǥ ȸ��
    private bool isMoving = false; // �̵� ���� üũ

    private PlayerState currentState;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        targetRotation = transform.rotation; // ���� ȸ���� �ʱ� ��ǥ ȸ������ ����
        SetState(new IdleState(this));
    }

    void Update()
    {
        HandleInput(); // �Է� ó�� �޼��� ȣ��
        currentState.Update();

        // �̵� �ִϸ��̼� �ӵ� ����
        animator.SetFloat("Speed", movement.magnitude);

        // �̵� ���� ���� ȸ���ϵ��� ó��
        if (isMoving)
        {
            RotateTowardsTarget();
        }
    }

    void FixedUpdate()
    {
        // �̵� ó��
        if (isMoving)
        {
            MovePlayer();
        }
    }

    void HandleInput()
    {
        movement = Vector3.zero; // �ʱ�ȭ
        isMoving = false; // �⺻������ �̵� �ƴ�

        // �Է¿� ���� �̵� �� ��ǥ ȸ�� ����
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) // ��
        {
            movement = new Vector3(1, 0, 0); // ���� �Ʒ� �밢��
            SetTargetRotation(Quaternion.Euler(0, 90, 0));
        }
        else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) // �� ��
        {
            movement = new Vector3(0, 0, 1); // ������ �Ʒ� �밢��
            SetTargetRotation(Quaternion.Euler(0, 0, 0));
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) // �� ��
        {
            movement = new Vector3(-1, 0, 0); // ������ �� �밢��
            SetTargetRotation(Quaternion.Euler(0, 270, 0));
        }
        else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W)) // �� ��
        {
            movement = new Vector3(0, 0, -1); // ���� �� �밢��
            SetTargetRotation(Quaternion.Euler(0, 180, 0));
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movement = new Vector3(1, 0, -1); // ���� �̵� ����
            SetTargetRotation(Quaternion.Euler(0, 135, 0));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movement = new Vector3(1, 0, 1); // �Ʒ��� �̵� ����
            SetTargetRotation(Quaternion.Euler(0, 45, 0));
        }
        else if (Input.GetKey(KeyCode.W))
        {
            movement = new Vector3(-1, 0, -1); // ���� �̵� ����
            SetTargetRotation(Quaternion.Euler(0, 225, 0));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            movement = new Vector3(-1, 0, 1); // ������ �̵� ����
            SetTargetRotation(Quaternion.Euler(0, 315, 0));
        }
    }

    private void SetTargetRotation(Quaternion newRotation)
    {
        targetRotation = newRotation;
        isMoving = true; // ȸ�� ���� �� ��� �̵� ����
    }

    private void MovePlayer()
    {
        // �̵� ó�� (Rigidbody�� MovePosition ���)
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    private void RotateTowardsTarget()
    {
        // ���� ȸ������ ��ǥ ȸ������ �ε巴�� ȸ��
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
