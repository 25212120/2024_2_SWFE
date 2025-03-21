using UnityEngine;
using UnityEngine.EventSystems;

public class AttackState : BaseState<PlayerStateType>
{
    private Animator animator;
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Rigidbody rb;

    public AttackState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Rigidbody rb, Animator animator, Transform playerTransform)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.animator = animator;
        this.playerInputManager = inputManager;
        this.rb = rb;
    }

    public override void EnterState()
    {
        animator.ResetTrigger("leftButton_Pressed");
        animator.ResetTrigger("finishedAttacking");

        playerInputManager.isAttacking = true;
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        animator.ResetTrigger("leftButton_Pressed");
        animator.ResetTrigger("NextCombo");
    }

    private bool noInput = true;

    public override void CheckTransitions()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = stateInfo.normalizedTime % 1;

        if (normalizedTime <= 0.9f)
        {
            if (playerInputManager.inputQueue.Count > 0)
            {
                noInput = false;
                string nextInput = playerInputManager.inputQueue.Dequeue();

                animator.SetTrigger(nextInput);
            }
            else
            {
                noInput = true;
            }
        }

        // 애니메이션 종료 시 처리
        if (normalizedTime > 0.93f && noInput)
        {
            playerInputManager.isAttacking = false;

            animator.SetTrigger("finishedAttacking");

            playerInputManager.inputQueue.Clear();

            stateManager.PopState();

        }
    }
}