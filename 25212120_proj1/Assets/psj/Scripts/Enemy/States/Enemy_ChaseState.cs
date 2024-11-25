using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_ChaseState : BaseState<EnemyStateType>
{
    private Transform enemyTransform;
    private MonoBehaviour monoBehaviour;
    private Animator animator;
    private Enemy enemy;
    private Monster_1 enemyStat;
    private Rigidbody rb;

    public Enemy_ChaseState(EnemyStateType key, StateManager<EnemyStateType> stateManager, Transform enemyTransform, Animator animator, Rigidbody rb, MonoBehaviour monoBehaviour, Monster_1 enemyStat, Enemy enemy) : base(key, stateManager)
    {
        this.enemyTransform = enemyTransform;
        this.enemyStat = enemyStat;
        this.enemy = enemy;
        this.monoBehaviour = monoBehaviour;
        this.animator = animator;
        this.rb = rb;
    }

    private Coroutine pathUpdateCoroutine;

    public override void EnterState()
    {
        enemy.DetectBasedOnPriority();

        pathUpdateCoroutine = monoBehaviour.StartCoroutine(UpdatePathRoutine());

        animator.SetBool("move", true);
        enemy.canDetect = true;
        enemy.agent.isStopped = false;
    }

    public override void UpdateState()
    {
        if (enemy.target != null)
        {
            Vector3 direction = (enemy.agent.steeringTarget - enemy.transform.position).normalized;
            direction.y = 0;

            float speed = enemy.agent.speed;

            enemy.transform.position += direction * speed * Time.deltaTime;

            if (direction != Vector3.zero)
            {
                float rotationSpeed = enemy.agent.angularSpeed;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

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

    private IEnumerator UpdatePathRoutine()
    {
        while (true)
        {
            if (enemy.agent.isActiveAndEnabled && enemy.target != null)
            {
                enemy.agent.SetDestination(enemy.target.position);
            }
            yield return new WaitForSeconds(1f); // 경로 재계산 주기 설정 (예: 0.5초마다)
        }
    }
}