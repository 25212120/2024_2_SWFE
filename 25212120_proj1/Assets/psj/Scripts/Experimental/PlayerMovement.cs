using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f; // ������ �ӵ�
    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        // Rigidbody2D ������Ʈ ��������
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // WASD Ű �Է� �ޱ�
        movement.x = Input.GetAxisRaw("Horizontal"); // A(-1)�� D(1) Ű �Է�
        movement.y = Input.GetAxisRaw("Vertical"); // W(1)�� S(-1) Ű �Է�
    }

    void FixedUpdate()
    {
        // Rigidbody2D�� ����Ͽ� ������Ʈ �̵�
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
