using UnityEngine;
using UnityEngine.AI;

public abstract class Unit : MonoBehaviour
{
    [Header("Components")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform targetEnemy;

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
        // Get required components
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        stateMachine = GetComponent<UnitStateMachine>();

        // Ensure this GameObject has the "unit" tag for UnitController detection
        if (gameObject.tag != "unit")
        {
            gameObject.tag = "unit";
            Debug.LogWarning($"Set missing 'unit' tag on {gameObject.name}");
        }
    }

    protected virtual void Start()
    {
        agent.updatePosition = false;
        agent.updateRotation = false;
        savedPosition = transform.position;
        stateMachine.PushState(UnitStateType.Idle);
        InitializeUnitParameters();
    }

    protected virtual void Update()
    {
        if (canDetectEnemy)
        {
            DetectEnemy();
        }
    }

    // Abstract method that each unit type must implement
    protected abstract void InitializeUnitParameters();
    protected abstract void PerformAttack();

    // Public method called by UnitController
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
            if (collider.CompareTag("Enemy"))
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
}