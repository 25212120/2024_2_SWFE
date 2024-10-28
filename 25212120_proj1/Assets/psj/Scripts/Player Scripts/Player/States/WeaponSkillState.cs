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
                // Ȱ ���⽺ų
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
                // Ȱ ���� ��ų
                break;
        }
    }

    private void Defend_SwordShield()
    {
        // �ִϸ��̼�
        monoBehaviour.StartCoroutine(Defend());
        // �������� �����ý����� �ϼ��� �� ���� �߰�
    }

    private IEnumerator Defend()
    {
        yield return new WaitForSeconds(3f);
        finishedWeaponSkill = true;
    }

    private void JumpAttack_SingleTwoHandedSword()
    {
        // �ִϸ��̼�
        
    }

    private void Rage_DoubleSwords()
    {
        // �ִϸ��̼�
    }

/*    private void ()_Bow()
    {

    }*/

}
