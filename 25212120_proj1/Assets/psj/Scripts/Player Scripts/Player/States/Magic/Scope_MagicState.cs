using UnityEngine;
using Cinemachine;
using System.Collections;

public class Scope_MagicState : BaseState<PlayerStateType>
{
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private MonoBehaviour monoBehaviour;

    public CinemachineVirtualCamera virtualCamera;

    public Scope_MagicState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Transform playerTransform, MonoBehaviour monoBehaviour)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.playerInputManager = inputManager;
        this.monoBehaviour = monoBehaviour;
    }

    private GameObject rangeSprite;
    private GameObject instantiatedSprite;

    private GameObject overlayObject;

    public override void EnterState()
    {
        playerInputManager.isPeformingAction = true;

        overlayObject = playerInputManager.dim;
        rangeSprite = playerInputManager.magicRangeSprite;

        Cursor.visible = false;
        instantiatedSprite = Object.Instantiate(rangeSprite);
        overlayObject.SetActive(true);
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
        overlayObject.SetActive(false);
        Cursor.visible = true;
    }

    public override void CheckTransitions()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Magic Performed");
            //stateManager.ChangeState(PlayerStateType.FireBall);
            playerInputManager.isPeformingAction = false;
            stateManager.PopState();
        }
/*        if (Input.GetKeyDown(KeyCode.T))
        {
            playerInputManager.isPeformingAction = false;
            Debug.Log("Magic Canceled");
            stateManager.PopState();
        }*/
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
}