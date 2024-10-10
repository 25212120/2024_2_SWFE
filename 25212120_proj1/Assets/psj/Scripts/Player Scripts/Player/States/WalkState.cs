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
        // Walk �ִϸ��̼� ����
    }

    public override void UpdateState()
    {
        // �ȱ� ���� ����
        // ��: �÷��̾� �̵� ����
    }

    public override void FixedUpdateState()
    {
        // ���� ������Ʈ���� ����Ǵ� �ڵ�
        MovePlayer();
    }

    public override void ExitState()
    {
        Debug.Log("Exiting WalkState");
        // Walking ���¿��� ���� �� ����Ǵ� �ڵ�
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
