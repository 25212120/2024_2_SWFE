using UnityEngine;
using Cinemachine;
using System.Collections;

public class Scope_MagicState : BaseState<PlayerStateType>
{
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Animator animator;
    private MonoBehaviour monoBehaviour;

    public CinemachineVirtualCamera virtualCamera;

    public Scope_MagicState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Transform playerTransform, MonoBehaviour monoBehaviour, Animator animator)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.playerInputManager = inputManager;
        this.monoBehaviour = monoBehaviour;
        this.animator = animator;
    }

    private GameObject rangeSprite;
    private GameObject instantiatedSprite;


    public override void EnterState()
    {
        playerInputManager.moveInput = Vector2.zero;
        animator.SetBool("moveInput", false);
        playerInputManager.isPerformingAction = true;

        rangeSprite = playerInputManager.magicRangeSprite;

        Cursor.visible = false;
        instantiatedSprite = Object.Instantiate(rangeSprite);
    }

    public override void UpdateState()
    {

    }

    public override void FixedUpdateState()
    {
        SpriteFollowCursor();
    }

    public override void ExitState()
    {
        Object.Destroy(instantiatedSprite);
        Cursor.visible = true;
    }

    public override void CheckTransitions()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                playerInputManager.magicPoint = hitInfo.point;
            }

            ChangeStateByMagicIndex(playerInputManager.currentMagicIndex);
            playerInputManager.isPerformingAction = false;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            playerInputManager.isPerformingAction = false;
            Debug.Log("Magic Canceled");
            stateManager.PopState();
        }
    }
    private void SpriteFollowCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 spritePosition = hitInfo.point;
            spritePosition.y += 0.1f;
            instantiatedSprite.transform.position = spritePosition;
        }
    }

    private void ChangeStateByMagicIndex(int magicIndex)
    {
        if (magicIndex == 1)    stateManager.ChangeState(playerInputManager.magic1);
        else if (magicIndex == 2)    stateManager.ChangeState(playerInputManager.magic2);
    }
}