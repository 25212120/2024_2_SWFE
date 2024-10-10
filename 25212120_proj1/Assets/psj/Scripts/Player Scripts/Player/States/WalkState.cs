using UnityEditor.Rendering;
using UnityEngine;

public class WalkState : BaseState<PlayerStateType>
{
    private PlayerStateMachine player;
    private PlayerInputManager playerInputManager;
    private Rigidbody rb;

    public WalkState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Rigidbody rb) : base(key, stateManager)
    {
        this.playerInputManager = inputManager;
        this.rb = rb;
    }

    public override void EnterState()
    {
        Debug.Log("Entered WalkState");
        // Walk 애니메이션 실행
    }

    public override void UpdateState()
    {
        // 걷기 동작 구현
        // 예: 플레이어 이동 로직
    }

    public override void FixedUpdateState()
    {
        // 물리 업데이트마다 실행되는 코드
        MovePlayer();
    }

    public override void ExitState()
    {
        Debug.Log("Exiting WalkState");
        // Walking 상태에서 나올 때 실행되는 코드
    }

    public override void CheckTransitions()
    {
        if (playerInputManager.moveInput == Vector2.zero)
        {
            stateManager.ChangeState(PlayerStateType.Idle);
        }
        else if (playerInputManager.leftShiftKey_Pressed == true)
        {
            stateManager.ChangeState(PlayerStateType.Sprint);
        }
    }

    private float walkSpeed = 15f;

    void MovePlayer()
    {
        float horizontalInput = playerInputManager.moveInput.x;
        float verticalInput = playerInputManager.moveInput.y;
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
        rb.AddForce(movement.normalized * walkSpeed);
    }
}
