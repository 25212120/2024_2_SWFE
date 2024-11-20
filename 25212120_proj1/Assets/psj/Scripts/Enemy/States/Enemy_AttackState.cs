using UnityEditor.Rendering;
using UnityEngine;

public class Enemy_AttackState : BaseState<EnemyStateType>
{
    private Transform enemyTransform;
    private MonoBehaviour monoBeahviour;
    private Animator animator;
    private Enemy enemy;
    private Monster_1 enemyStat;
    private Rigidbody rb;

    public Enemy_AttackState(EnemyStateType key, StateManager<EnemyStateType> stateManager, Transform enemyTransform, Animator animator, Rigidbody rb, MonoBehaviour monoBehaviour, Monster_1 enemyStat, Enemy enemy) : base(key, stateManager)
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
        animator.SetTrigger("attack");

        enemy.agent.isStopped = true;
    }

    public override void UpdateState()
    {
        if (enemy.target == null || enemy.target.Equals(null))
        {
            enemy.canDetect = true;
        }

        if (enemy.target != null)
        {
            enemy.canDetect = false;
            Vector3 direction = (enemy.target.position - enemy.transform.position).normalized;
            direction.y = 0;
            enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, Quaternion.LookRotation(direction), 1440f * Time.deltaTime);
            enemy.Attack();
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        animator.ResetTrigger("attack");
        enemy.attackCooldown = 1f / enemy.attackSpeed;
        enemy.canDetect = true;
        enemy.agent.isStopped = true;
    }

    public override void CheckTransitions()
    {
        if (enemy.canAttack == false)
        {
            stateManager.ChangeState(EnemyStateType.Chase);
        }
    }
}