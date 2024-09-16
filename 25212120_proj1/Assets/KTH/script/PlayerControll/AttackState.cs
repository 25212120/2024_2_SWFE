using UnityEngine;

public class AttackState : PlayerState
{
    public AttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.animator.SetBool("IsAttacking", true);
    }

    public override void Update()
    {
        if (!Input.GetMouseButton(0))
        {
            player.SetState(new IdleState(player));
        }
    }

    public override void Exit()
    {
        player.animator.SetBool("IsAttacking", false);
    }
}
