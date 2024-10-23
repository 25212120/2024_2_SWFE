using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponSwapState : BaseState<PlayerStateType>
{

    private Animator animator;
    private PlayerInputManager playerInputManager;

    public WeaponSwapState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Animator animator) : base(key, stateManager)
    {
        this.playerInputManager = inputManager;
        this.animator = animator;
    }

    private GameObject rightHand;
    private GameObject leftHand;

    public override void EnterState()
    {
        bool needSwap = CheckSwapTo();
        if (!needSwap)
        {
            stateManager.PopState();
            return;
        }
        playerInputManager.isSwapping = true;

        playerInputManager.previousRightHandIndex = playerInputManager.currentRightHandIndex;
        playerInputManager.previousLeftHandIndex = playerInputManager.currentLeftHandIndex;

        playerInputManager.currentRightHandIndex = playerInputManager.IndexSwapTo;
        playerInputManager.currentLeftHandIndex = playerInputManager.IndexSwapTo;

        animator.SetTrigger("SwapKey_Pressed");
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        animator.ResetTrigger("SwapKey_Pressed");
        animator.ResetTrigger("finishedSwapping");
        SaveAnimatorParameters();
        animator.runtimeAnimatorController = playerInputManager.player_animControllers[playerInputManager.currentRightHandIndex];
        ReapplyAnimatorParameters();
        playerInputManager.isSwapping = false;
    }

    public override void CheckTransitions()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        Debug.Log("it worked");

        if (stateInfo.IsTag("Swap") && stateInfo.normalizedTime >= 1.0f)
        {
            animator.SetTrigger("finishedSwapping");

            playerInputManager.currentRightHandIndex = playerInputManager.IndexSwapTo;
            playerInputManager.currentLeftHandIndex = playerInputManager.IndexSwapTo;

            stateManager.PopState();
        }
    }

    private bool CheckSwapTo()
    {
        if (playerInputManager.currentRightHandIndex == playerInputManager.IndexSwapTo) return false;

        switch (playerInputManager.IndexSwapTo)
        {
            case 0:
                SetHand(0);
                break;
            case 1:
                SetHand(1);
                break;
            case 2:
                SetHand(2);
                break;
                //case 4:
                //    break;   
                //case 5:
                //    break;         
        }

        return true;
    }

    private void SetHand(int index)
    {
        leftHand = playerInputManager.leftHand_Weapons[index];
        rightHand = playerInputManager.rightHand_Weapons[index];
    }

    private Dictionary<int, object> savedParameters = new Dictionary<int, object>();

    private void SaveAnimatorParameters()
    {
        savedParameters.Clear();
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    savedParameters[param.nameHash] = animator.GetBool(param.nameHash);
                    break;
                case AnimatorControllerParameterType.Float:
                    savedParameters[param.nameHash] = animator.GetFloat(param.nameHash);
                    break;
            }
        }
    }

    private void ReapplyAnimatorParameters()
    {
        foreach (var param in savedParameters)
        {
            if (animator.parameters.Any(p => p.nameHash == param.Key))
            {
                var animatorParam = animator.parameters.First(p => p.nameHash == param.Key);
                switch (animatorParam.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(param.Key, (bool)param.Value);
                        break;
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(param.Key, (float)param.Value);
                        break;
                }
            }
        }
    }
}