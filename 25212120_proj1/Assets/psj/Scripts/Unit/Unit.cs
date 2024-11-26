using UnityEngine;
using UnityEngine.AI;

public abstract class Unit : MonoBehaviour
{
    [Header("Components")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform targetEnemy;
    public BaseUnit unitStat;

    [Header("Position Data")]
    public Vector3 savedPosition;
    public bool isMove = false;

    [Header("Unit Parameters")]
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float attackSpeed = 1f;
    public float maxDistanceFromSavedPosition = 20f;

    [HideInInspector]
    public UnitStateMachine stateMachine;
    [HideInInspector]
    public bool canDetectEnemy = true;
    [HideInInspector]
    public float attackCooldown = 0f;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        unitStat = GetComponent<BaseUnit>();
        stateMachine = GetComponent<UnitStateMachine>();
    }

    protected virtual void Start()
    {
        GameManager.instance.AddUnit(gameObject);

        agent.updatePosition = false;
        agent.updateRotation = false;
        savedPosition = transform.position;
        stateMachine.PushState(UnitStateType.Idle);
        InitializeUnitParameters();
    }

    protected virtual void Update()
    {
        HPCheck();



        if (canDetectEnemy)
        {
            DetectEnemy();
        }

        if (targetEnemy != null && targetEnemy.gameObject.layer == LayerMask.NameToLayer("Dead"))
        {
            targetEnemy = null;
        }

        if (targetEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, targetEnemy.position);
            float threshold = 1f;
            if (distance >= threshold)
            {
                agent.SetDestination(targetEnemy.transform.position);
            }
        }
    }

    protected abstract void InitializeUnitParameters();
    protected abstract void PerformAttack();

    public void MoveToPosition(Vector3 position)
    {
        isMove = true;
        agent.SetDestination(position);
        stateMachine.ChangeState(UnitStateType.Move);
    }

    public void DetectEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy") && collider.gameObject.layer != LayerMask.NameToLayer("Dead"))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = collider.transform;
                }
            }
        }

        if (closestEnemy != null)
        {
            targetEnemy = closestEnemy;

            if (isMove)
            {
                savedPosition = transform.position;
            }

            stateMachine.ChangeState(UnitStateType.Chase);
        }
    }

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

    public void HPCheck()
    {
        if (unitStat.GetCurrntHP() <= 0)
        {
            stateMachine.PushState(UnitStateType.Die);
        }
    }

}