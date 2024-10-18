using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerStateMachine : StateManager<PlayerStateType>
{
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private PlayerCoolDownManager playerCoolDownmanager;
    private Animator animator;
    private Rigidbody rb;

    public bool isPerformingAction = false;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        playerCoolDownmanager = GetComponent<PlayerCoolDownManager>();
        animator = GetComponent<Animator>();
        playerTransform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        // 지속 State
        States.Add(PlayerStateType.Idle, new IdleState(PlayerStateType.Idle, this, playerInputManager, rb, animator));
        States.Add(PlayerStateType.Walk, new WalkState(PlayerStateType.Walk, this, playerInputManager, rb, animator));
        States.Add(PlayerStateType.Sprint, new SprintState(PlayerStateType.Sprint, this, playerInputManager, rb, animator));

        // 단발성 State
        States.Add(PlayerStateType.Dash, new DashState(PlayerStateType.Dash, this, playerInputManager, rb, animator, playerTransform, playerCoolDownmanager));
        States.Add(PlayerStateType.Jump, new JumpState(PlayerStateType.Jump, this, playerInputManager, rb, animator, playerTransform));
        States.Add(PlayerStateType.Attack, new AttackState(PlayerStateType.Attack, this, playerInputManager, rb, animator, playerTransform));
        States.Add(PlayerStateType.Interaction, new InteractionState(PlayerStateType.Interaction, this, playerInputManager, rb, animator, playerTransform));

        // 상태들 전부 추가

        PushState(PlayerStateType.Idle);

    }

}
