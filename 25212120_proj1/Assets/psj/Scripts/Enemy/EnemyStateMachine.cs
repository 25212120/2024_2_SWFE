using UnityEditor.Build.Content;
using UnityEngine;

public class EnemyStateMachine : StateManager<EnemyStateType>
{
    private Transform enemyTransform;
    private Animator animator;
    private Enemy enemy;
    private Rigidbody rb;
    private MonoBehaviour monoBehaviour;
    private Monster_1 enemyStat;

    private void Awake()
    {
        enemyStat = GetComponent<Monster_1>();
        enemyTransform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemy = GetComponent<Enemy>();
        monoBehaviour = GetComponent<MonoBehaviour>();


        States.Add(EnemyStateType.Chase , new Enemy_ChaseState(EnemyStateType.Chase, this, enemyTransform, animator, rb, monoBehaviour, enemyStat, enemy));
        States.Add(EnemyStateType.Attack , new Enemy_AttackState(EnemyStateType.Attack, this, enemyTransform, animator, rb, monoBehaviour, enemyStat, enemy));
        States.Add(EnemyStateType.Die , new Enemy_DieState(EnemyStateType.Die, this, animator, monoBehaviour, enemyStat));
        States.Add(EnemyStateType.Idle , new Enemy_IdleState(EnemyStateType.Idle, this, animator,enemy));
    }

    protected override void Update()
    {
        base.Update();
        Debug.Log(gameObject + " : " + CurrentState.ToString());
    }


}
