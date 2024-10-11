using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerStateMachine : StateManager<PlayerStateType>
{
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Animator animator;
    private Rigidbody rb;

    public bool isPerformingAction = false;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        animator = GetComponent<Animator>();
        playerTransform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        // 지속 State
        States.Add(PlayerStateType.Idle, new IdleState(PlayerStateType.Idle, this, playerInputManager, rb, animator));
        States.Add(PlayerStateType.Walk, new WalkState(PlayerStateType.Idle, this, playerInputManager, rb, animator));
        States.Add(PlayerStateType.Sprint, new SprintState(PlayerStateType.Idle, this, playerInputManager, rb, animator));

        // 단발성 State
        States.Add(PlayerStateType.Dash, new DashState(PlayerStateType.Idle, this, playerInputManager, rb, animator, playerTransform));
        States.Add(PlayerStateType.Jump, new JumpState(PlayerStateType.Idle, this, playerInputManager, rb, animator, playerTransform));
/*        States.Add(PlayerStateType.Attack_1, new Attack1State(this));
        States.Add(PlayerStateType.Attack_2, new Attack2State(this));
        States.Add(PlayerStateType.Attack_3, new Attack3State(this));
        States.Add(PlayerStateType.Interaction, new InteractionState(this));*/

        // 상태들 전부 추가

        PushState(PlayerStateType.Idle);

    }

}
