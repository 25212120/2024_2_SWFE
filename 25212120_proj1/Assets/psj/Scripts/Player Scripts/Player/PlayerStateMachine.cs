using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateMachine : StateManager<PlayerStateType>
{
    private PlayerInputManager playerInputManager;
    private Rigidbody rb;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        rb = GetComponent<Rigidbody>();

        // ���� State
        States.Add(PlayerStateType.Idle, new IdleState(PlayerStateType.Idle, this, playerInputManager, rb));
        States.Add(PlayerStateType.Walk, new WalkState(PlayerStateType.Idle, this, playerInputManager, rb));
        States.Add(PlayerStateType.Sprint, new SprintState(PlayerStateType.Idle, this, playerInputManager, rb));

        // �ܹ߼� State
        States.Add(PlayerStateType.Roll, new RollState(PlayerStateType.Idle, this, playerInputManager, rb));
/*        States.Add(PlayerStateType.Attack_1, new Attack1State(this));
        States.Add(PlayerStateType.Attack_2, new Attack2State(this));
        States.Add(PlayerStateType.Attack_3, new Attack3State(this));
        States.Add(PlayerStateType.Interaction, new InteractionState(this));*/

        // ���µ� ���� �߰�

        PushState(PlayerStateType.Idle);

    }

}
