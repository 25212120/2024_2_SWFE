using UnityEngine;

public class MoveState : PlayerState
{
    public MoveState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.animator.SetFloat("Speed", 1); // 이동 시작 애니메이션
    }

    public override void Update()
    {
        // 이동 중이라면 상태 유지, 아니면 Idle 상태로 전환
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
        // 필요시 상태 종료 시 할 동작을 추가할 수 있습니다.
    }
}
