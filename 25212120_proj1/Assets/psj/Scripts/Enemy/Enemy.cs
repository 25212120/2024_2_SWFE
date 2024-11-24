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

    // layer == Dead인 것들 제외하고, 우선순위 detect, target 설정까지 무조건 완료
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

                // 감지한 오브젝트의 tag가 priorityTags에 있는지 확인하고, 있다면 index를 반환하고, 없다면 -1을 반환함
                int priorityIndex = System.Array.IndexOf(priorityTags, tag);

                // 감지한 오브젝트의 tag가 priorityTags에 존재하고, 
                if (priorityIndex != -1 && priorityIndex < highestPriority)
                {

                    // 태그가 같은 오브젝트들에 대하여, 가장 가까운 오브젝트를 반환함.
                    float distance = Vector3.Distance(transform.position, collider.transform.position);

                    if (priorityIndex < highestPriority || (priorityIndex == highestPriority && distance < closestDistance))
                    {
                        highestPriority = priorityIndex;
                        closestDistance = distance;
                        detectedTarget = collider.gameObject;
                    }
                    
                }
                // {} 감지한 오브젝트의 tag가 priorityTags에 존재하지 않는다.
            }
        }
        //모든 오브젝트들에 대한 검사 완료
        if (detectedTarget == null)
        {
            target = core.transform;
        }
        else
        {
            target = detectedTarget.transform;
        }
    }

    //임시로 core의 위치가 (0,0,0)이라고 가정하겠음
    public GameObject core;

}