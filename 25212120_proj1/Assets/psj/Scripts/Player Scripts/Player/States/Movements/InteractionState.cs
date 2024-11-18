using System.Threading;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionState : BaseState<PlayerStateType>
{
    private Animator animator;
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Rigidbody rb;

    public InteractionState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Rigidbody rb, Animator animator, Transform playerTransform)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.animator = animator;
        this.playerInputManager = inputManager;
        this.rb = rb;
    }

    private GameObject targetResource;
    private Material_Test material;

    public override void EnterState()
    {
        animator.SetTrigger("F_Key_Pressed");
        targetResource = GetClosestResource();
        if (targetResource == null) stateManager.PopState();
        else
        {
            material = targetResource.GetComponent<Material_Test>();
            material.MaterialDie(4f);
            playerInputManager.isPerformingAction = true;
        }
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        // Interaction Logics
        animator.SetTrigger("finishedInteracting");
        material.SetWaitSuccess_False();
        playerInputManager.isPerformingAction = false;
        animator.ResetTrigger("F_Key_Pressed");
        material.Die();
    }

    public override void CheckTransitions()
    {
        if (material.finished == true)
        {
            material.finished = false;
            stateManager.PopState();
        }
    }

    public GameObject GetClosestResource()
    {
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, 4f);
        GameObject closestResource = null;
        float closestDistance = Mathf.Infinity;

        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Resource"))
            {
                float distance = Vector3.Distance(playerTransform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestResource = collider.gameObject;
                }
            }
        }

        return closestResource;
    }
}