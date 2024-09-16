using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.animator.SetFloat("Speed", 0);
    }

    public override void Update()
    {
        if (player.movement.sqrMagnitude > 0.1f)
        {
            player.SetState(new MoveState(player));
        }
        else if (Input.GetMouseButtonDown(0))
        {
            player.SetState(new AttackState(player));
        }
    }

    public override void Exit() { }
}
