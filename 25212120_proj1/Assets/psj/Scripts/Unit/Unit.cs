using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    // ���� ����
    public NavMeshAgent agent;
    public Animator animator;
    public Transform targetEnemy;
    public Vector3 savedPosition;

    public bool isMove = false;

    [HideInInspector]
    public UnitStateMachine stateMachine;

    // ���� ������ ������
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float maxDistanceFromSavedPosition = 20f;
    public float attackSpeed = 1f;

    // ���� ����
    [HideInInspector]
    public bool canDetectEnemy = true;
    [HideInInspector]
    public float attackCooldown = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        stateMachine = GetComponent<UnitStateMachine>();
    }

    private void Start()
    {
        agent.updatePosition = false;
        agent.updateRotation = false;
        savedPosition = transform.position;
        stateMachine.PushState(UnitStateType.Idle);
    }

    private void Update()
    {
        if (canDetectEnemy)
        {
            DetectEnemy();
        }
    }

    // ���� �Լ���



    // �̵� ��� ó��
    public void MoveToPosition(Vector3 position)
    {
        agent.SetDestination(position);
        stateMachine.ChangeState(UnitStateType.Move);
    }

    // ���� ����� �� Ž��
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

            if (isMove == true)
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
            Debug.Log($"{gameObject.name}��(��) {targetEnemy.name}��(��) �����մϴ�.");
            attackCooldown = 1f / attackSpeed; 
            animator.SetTrigger("attack");
        }
        else
        {
            attackCooldown -= Time.deltaTime;
        }
    }
}
