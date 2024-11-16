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

    public override void EnterState()
    {
        animator.SetTrigger("F_Key_Pressed");
        playerInputManager.isPerformingAction = true;
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
        playerInputManager.isPerformingAction = true;
        animator.ResetTrigger("F_Key_Pressed");
    }

    public override void CheckTransitions()
    {
        // 이후 자원마다 상호작용 시간 정해질 시 loop time 활성화하고 transition 조건 재설정
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        float normalizedTime = stateInfo.normalizedTime % 1;

        if (stateInfo.IsName("Interaction") == true && normalizedTime >= 0.95f)
        {
            stateManager.PopState();
        }
    }

    // 1. 상호작용 방향으로 캐릭터 회전
    // 2. 자원 종류마다 상호작용 시간 다르게 적용
}