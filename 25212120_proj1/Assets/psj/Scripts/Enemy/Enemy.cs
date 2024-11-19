using UnityEditor;
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
        agent.updatePosition = false;
        agent.updateRotation = false;
        stateMachine.PushState(EnemyStateType.Chase);
        InitializeEnemyParameters();
    }

    protected virtual void Update()
    {

        HPCheck();

        if (target != null && target.gameObject.layer == LayerMask.NameToLayer("Dead"))
        {
            target = null; // 죽은 적을 타겟에서 제외
        }

        if (canDetect)
        {
            DetectBasedOnPriority();
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

    public void DetectBasedOnPriority()
    {
        string[] priorityTags = new string[] { "unit", "tower", "Player", "Core" };

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);

        GameObject detectedTarget = null;
        int highestPriority = priorityTags.Length;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.layer != LayerMask.NameToLayer("Dead"))
            {
                string tag = collider.tag;

                int priorityIndex = System.Array.IndexOf(priorityTags, tag);

                if (priorityIndex != -1 && priorityIndex < highestPriority)
                {

                    float distance = Vector3.Distance(transform.position, collider.transform.position);

                    if (priorityIndex < highestPriority || (priorityIndex == highestPriority && distance < closestDistance))
                    {
                        highestPriority = priorityIndex;
                        closestDistance = distance;
                        detectedTarget = collider.gameObject;
                    }
                }

                if (detectedTarget != null)
                {
                    this.target = detectedTarget.transform;
                }
            }
        }
    }

    //임시로 core의 위치가 (0,0,0)이라고 가정하겠음
    public GameObject core;
}