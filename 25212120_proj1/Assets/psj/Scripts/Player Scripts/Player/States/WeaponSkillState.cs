using System.Collections;
using UnityEngine;

public class WeaponSkillState : BaseState<PlayerStateType>
{
    private Animator animator;
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Rigidbody rb;
    private MonoBehaviour monoBehaviour;


    public WeaponSkillState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Rigidbody rb, Animator animator, Transform playerTransform, MonoBehaviour monoBehaviour)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.animator = animator;
        this.playerInputManager = inputManager;
        this.rb = rb;
        this.monoBehaviour = monoBehaviour;
    }

    private bool finishedWeaponSkill = false;

    public override void EnterState()
    {
        PerformWeaponSkill(playerInputManager.currentRightHandIndex);
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()    
    {
        finishedWeaponSkill = false;
    }

    public override void CheckTransitions()
    {
        FinishWeaponSkill(playerInputManager.currentRightHandIndex);
    }

    private void PerformWeaponSkill(int weaponIndex)
    {
        switch (weaponIndex)
        {
            case 0:
                Defend_SwordShield();
                break;
            case 1:
                playerInputManager.wantToCheckGround = false;
                JumpAttack_SingleTwoHandedSword();
                break;
            case 2:
                Rage_DoubleSwords();
                break;
            case 3:
                // 활 무기스킬
                break;
        }
    }

    private void FinishWeaponSkill(int weaponIndex)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        switch (weaponIndex)
        {
            case 0:
                if (stateInfo.IsTag("WeaponSkill") && finishedWeaponSkill == true)
                {
                    animator.SetTrigger("finishedWeaponSkill");
                    stateManager.PopState();
                }
                break;
            case 1:
                if (stateInfo.IsTag("WeaponSkill") && stateInfo.normalizedTime >= 1.0f)
                {
                    animator.SetTrigger("finishedWeaponSkill");
                    stateManager.PopState();
                }
                break;
            case 2:
                if (stateInfo.IsTag("WeaponSkill") && stateInfo.normalizedTime > 0.95f)
                {
                    playerInputManager.wantToCheckGround = true;
                    animator.SetTrigger("finishedWeaponSkill");
                    stateManager.PopState();
                }
                break;
            case 3:
                // 활 무기 스킬
                break;
        }
    }

    private void Defend_SwordShield()
    {
        // 애니메이션
        monoBehaviour.StartCoroutine(Defend());
        // 도균이의 전투시스템이 완성될 때 로직 추가
    }

    private IEnumerator Defend()
    {
        yield return new WaitForSeconds(3f);
        finishedWeaponSkill = true;
    }

    private void JumpAttack_SingleTwoHandedSword()
    {
        // 애니메이션
        
    }

    private void Rage_DoubleSwords()
    {
        // 애니메이션
    }

/*    private void ()_Bow()
    {

    }*/

}
