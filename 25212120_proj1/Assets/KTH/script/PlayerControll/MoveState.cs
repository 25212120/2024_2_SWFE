using UnityEngine;

public class MoveState : PlayerState
{
    public MoveState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.animator.SetFloat("Speed", 1); // �̵� ���� �ִϸ��̼�
    }

    public override void Update()
    {
        // �̵� ���̶�� ���� ����, �ƴϸ� Idle ���·� ��ȯ
        if (player.movement.sqrMagnitude <= 0.1f)
        {
            player.SetState(new IdleState(player));
        }
        else if (Input.GetMouseButtonDown(0))
        {
            player.SetState(new AttackState(player));
        }
    }

    public override void Exit() 
    {
        // �ʿ�� ���� ���� �� �� ������ �߰��� �� �ֽ��ϴ�.
    }
}
