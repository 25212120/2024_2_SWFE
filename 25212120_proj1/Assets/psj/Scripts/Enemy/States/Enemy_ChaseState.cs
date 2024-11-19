using UnityEngine;

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
        if (enemy.target == null)
        {
            enemy.target = enemy.core.transform;
        }

        animator.SetBool("move", true);
        enemy.agent.isStopped = false;
    }

    public override void UpdateState()
    {
        enemy.agent.SetDestination(enemy.target.position);

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
        float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.target.position);
        if (distanceToTarget <= enemy.attackRange)
        {
            stateManager.ChangeState(EnemyStateType.Attack);
        }
    }
}