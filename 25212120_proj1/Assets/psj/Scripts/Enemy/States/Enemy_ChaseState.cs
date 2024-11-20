using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_ChaseState : BaseState<EnemyStateType>
{
    private Transform enemyTransform;
    private MonoBehaviour monoBeahviour;
    private Animator animator;
    private Enemy enemy;
    private Monster_1 enemyStat;
    private Rigidbody rb;

    public Enemy_ChaseState(EnemyStateType key, StateManager<EnemyStateType> stateManager, Transform enemyTransform, Animator animator, Rigidbody rb, MonoBehaviour monoBehaviour, Monster_1 enemyStat, Enemy enemy) : base(key, stateManager)
    {
        this.enemyTransform = enemyTransform;
        this.enemyStat = enemyStat;
        this.enemy = enemy;
        this.monoBeahviour = monoBehaviour;
        this.animator = animator;
        this.rb = rb;
    }


    public override void EnterState()
    {
        enemy.DetectBasedOnPriority();

        animator.SetBool("move", true);
        enemy.canDetect = true;
        enemy.agent.isStopped = false;
    }

    public override void UpdateState()
    {
        Vector3 velocity = enemy.agent.desiredVelocity;
        velocity.y = 0;

        if (velocity.sqrMagnitude > 0.01f)
        {
            float rotationSpeed = 1000f;
            enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, Quaternion.LookRotation(velocity), rotationSpeed * Time.deltaTime);
            enemy.transform.position += velocity * Time.deltaTime;

            enemy.agent.nextPosition = enemy.transform.position;
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        animator.SetBool("move", false);
    }

    public override void CheckTransitions()
    {
        if (enemy.canAttack == true)
        {
            stateManager.ChangeState(EnemyStateType.Attack);
        }
    }
}