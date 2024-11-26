using UnityEngine;

public class Enemy_HitState : BaseState<EnemyStateType>
{
    private Animator animator;
    private Enemy enemy;

    public Enemy_HitState(EnemyStateType key, StateManager<EnemyStateType> stateManager, Animator animator, Enemy enemy)
            : base(key, stateManager)
    {
        this.animator = animator;
        this.enemy = enemy;
    }

    public override void EnterState()
    {
        animator.SetTrigger("getHit");
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        animator.ResetTrigger("getHit");
        enemy.isHit = false;
    }

    public override void CheckTransitions()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsTag("Hit") && stateInfo.normalizedTime >= 0.7f)
        {
            stateManager.PopState();
        }
    }
}
