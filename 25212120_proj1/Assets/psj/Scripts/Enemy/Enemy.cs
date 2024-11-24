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

    // layer == Dead�� �͵� �����ϰ�, �켱���� detect, target �������� ������ �Ϸ�
    public void DetectedBaseOnDistance()
    {
        // �⺻��: Core�� �⺻ Ÿ������ ����
        target = core.transform;

        // ���� ���� ���� Collider ��������
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);

        // ���� ����� ��� �ʱ�ȭ
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in hitColliders)
        {
            // Layer�� "Dead"�� �ƴ� ��츸 �˻�
            if (collider.gameObject.layer != LayerMask.NameToLayer("Dead"))
            {
                if (collider.gameObject != gameObject && (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("unit") || collider.gameObject.CompareTag("tower") || collider.gameObject.CompareTag("Core")))
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);

                    // ���� ����� ��� ������Ʈ
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = collider.gameObject;
                    }
                }
            }
        }

        // ���� ����� ����� Ÿ������ ����
        if (closestTarget != null)
        {
            target = closestTarget.transform;
        }
        else
        {
            target = core.transform;
        }
    }


    //�ӽ÷� core�� ��ġ�� (0,0,0)�̶�� �����ϰ���

}