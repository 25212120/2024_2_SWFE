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

        agent.updatePosition = false;
        agent.updateRotation = false;
        stateMachine.PushState(EnemyStateType.Chase);

        //test
        core = GameObject.FindGameObjectWithTag("Core");
        //test

        InitializeEnemyParameters();
    }

    protected virtual void Update()
    {

        HPCheck();

        if (canDetect)
        {
            DetectBasedOnPriority();
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
    public void DetectBasedOnPriority()
    {
        string[] priorityTags = new string[] { "unit", "tower", "Player" };

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);

        GameObject detectedTarget = null;
        int highestPriority = priorityTags.Length;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.layer != LayerMask.NameToLayer("Dead"))
            {
                string tag = collider.tag;

                // ������ ������Ʈ�� tag�� priorityTags�� �ִ��� Ȯ���ϰ�, �ִٸ� index�� ��ȯ�ϰ�, ���ٸ� -1�� ��ȯ��
                int priorityIndex = System.Array.IndexOf(priorityTags, tag);

                // ������ ������Ʈ�� tag�� priorityTags�� �����ϰ�, 
                if (priorityIndex != -1 && priorityIndex < highestPriority)
                {

                    // �±װ� ���� ������Ʈ�鿡 ���Ͽ�, ���� ����� ������Ʈ�� ��ȯ��.
                    float distance = Vector3.Distance(transform.position, collider.transform.position);

                    if (priorityIndex < highestPriority || (priorityIndex == highestPriority && distance < closestDistance))
                    {
                        highestPriority = priorityIndex;
                        closestDistance = distance;
                        detectedTarget = collider.gameObject;
                    }
                    
                }
                // {} ������ ������Ʈ�� tag�� priorityTags�� �������� �ʴ´�.
            }
        }
        //��� ������Ʈ�鿡 ���� �˻� �Ϸ�
        if (detectedTarget == null)
        {
            target = core.transform;
        }
        else
        {
            target = detectedTarget.transform;
        }
    }

    //�ӽ÷� core�� ��ġ�� (0,0,0)�̶�� �����ϰ���
    public GameObject core;

}