using UnityEngine;
using Cinemachine;
using System.Collections;

public class SolarBeam_MagicState : BaseState<PlayerStateType>
{
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Animator animator;
    private MonoBehaviour monoBehaviour;

    public SolarBeam_MagicState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Transform playerTransform, MonoBehaviour monoBehaviour, Animator animator)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.playerInputManager = inputManager;
        this.monoBehaviour = monoBehaviour;
        this.animator = animator;
    }

    public override void EnterState()
    {
        
    }

    public override void UpdateState()
    {

    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
    }

    public override void CheckTransitions()
    {
    }
}