using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    [Header("Components")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform target;
    public Monster_1 enemyStat;

    [Header("Position Data")]
    public bool isMove = false;

    [Header("Enemy Parameters")]
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float attackSpeed = 1f;

    [HideInInspector]
    public EnemyStateMachine stateMachine;
    [HideInInspector]
    public float attackCooldown = 0f;
    [HideInInspector]
    public bool canDetect = true;

    public bool canAttack = false;


    public GameObject core;

    protected virtual void Awake()
    {
        // Get required components
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyStat = GetComponent<Monster_1>();
        stateMachine = GetComponent<EnemyStateMachine>();
    }

    protected virtual void Start()
    {
        GameManager.instance.AddEnemy(gameObject);

        core = GameManager.instance.core;

        agent.updatePosition = false;
        agent.updateRotation = false;
        stateMachine.PushState(EnemyStateType.Chase);


        InitializeEnemyParameters();
    }

    protected virtual void Update()
    {

        HPCheck();

        if (canDetect)
        {
            DetectedBaseOnDistance();
        }

        if (target != null && agent.isActiveAndEnabled == true)
        {
            agent.SetDestination(target.transform.position);
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < attackRange) canAttack = true;
            else canAttack = false;
        }
    }


    private void HPCheck()
    {
        if (enemyStat.GetCurrentHP() <= 0)
        {
            stateMachine.PushState(EnemyStateType.Die);
        }
    }

    protected abstract void InitializeEnemyParameters();
    protected abstract void PerformAttack();

    public void Attack()
    {
        if (attackCooldown <= 0f)
        {
            PerformAttack();
            attackCooldown = 1f / attackSpeed;
        }
        else
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    // layer == Dead인 것들 제외하고, 우선순위 detect, target 설정까지 무조건 완료
    public void DetectedBaseOnDistance()
    {
        // 기본값: Core를 기본 타겟으로 설정
        target = core.transform;

        // 감지 범위 내의 Collider 가져오기
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);

        // 가장 가까운 대상 초기화
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in hitColliders)
        {
            // Layer가 "Dead"가 아닌 경우만 검사
            if (collider.gameObject.layer != LayerMask.NameToLayer("Dead"))
            {
                if (collider.gameObject != gameObject && (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("unit") || collider.gameObject.CompareTag("tower") || collider.gameObject.CompareTag("Core")))
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);

                    // 가장 가까운 대상 업데이트
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = collider.gameObject;
                    }
                }
            }
        }

        // 가장 가까운 대상을 타겟으로 설정
        if (closestTarget != null)
        {
            target = closestTarget.transform;
        }
        else
        {
            target = core.transform;
        }
    }


    //임시로 core의 위치가 (0,0,0)이라고 가정하겠음

}