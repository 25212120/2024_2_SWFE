using System.Collections;
using UnityEngine;

public class Enemy_IdleState : BaseState<EnemyStateType>
{
    private Animator animator;
    private Enemy enemy;

    public Enemy_IdleState(EnemyStateType key, StateManager<EnemyStateType> stateManager, Animator animator, Enemy enemy) : base(key, stateManager)
    {
        this.animator = animator;
        this.enemy = enemy;
    }

    public override void EnterState()
    {
        enemy.agent.isStopped = true;
        animator.SetBool("move", false);
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        enemy.agent.isStopped = false;
    }

    public override void CheckTransitions()
    {
    }
}