using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerStateMachine : StateManager<PlayerStateType>
{
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private PlayerCoolDownManager playerCoolDownmanager;
    private Animator animator;
    private Rigidbody rb;
    private MonoBehaviour monoBehaviour;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        playerCoolDownmanager = GetComponent<PlayerCoolDownManager>();
        animator = GetComponent<Animator>();
        playerTransform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        monoBehaviour = GetComponent<MonoBehaviour>();

        // 지속 State
        States.Add(PlayerStateType.Idle, new IdleState(PlayerStateType.Idle, this, playerInputManager, rb, animator));
        States.Add(PlayerStateType.Walk, new WalkState(PlayerStateType.Walk, this, playerInputManager, rb, animator));
        States.Add(PlayerStateType.Sprint, new SprintState(PlayerStateType.Sprint, this, playerInputManager, rb, animator));

        // 단발성 State
        States.Add(PlayerStateType.Dash, new DashState(PlayerStateType.Dash, this, playerInputManager, rb, animator, playerTransform, playerCoolDownmanager));
        States.Add(PlayerStateType.Jump, new JumpState(PlayerStateType.Jump, this, playerInputManager, rb, animator, playerTransform));
        States.Add(PlayerStateType.Attack, new AttackState(PlayerStateType.Attack, this, playerInputManager, rb, animator, playerTransform));
        States.Add(PlayerStateType.Interaction, new InteractionState(PlayerStateType.Interaction, this, playerInputManager, rb, animator, playerTransform));
        States.Add(PlayerStateType.WeaponSwap, new WeaponSwapState(PlayerStateType.WeaponSwap, this, playerInputManager, animator));
        States.Add(PlayerStateType.WeaponSkill, new WeaponSkillState(PlayerStateType.WeaponSkill, this, playerInputManager, rb, animator,playerTransform, monoBehaviour));
        States.Add(PlayerStateType.Scope_MagicState, new Scope_MagicState(PlayerStateType.Scope_MagicState, this, playerInputManager, playerTransform, monoBehaviour, animator));
        // 상태들 전부 추가

        //MagicState
        States.Add(PlayerStateType.FireBall_MagicState, new FireBall_MagicState(PlayerStateType.FireBall_MagicState, this, playerInputManager, playerTransform, monoBehaviour, animator));
        States.Add(PlayerStateType.Meteor_MagicState, new Meteor_MagicState(PlayerStateType.Meteor_MagicState, this, playerInputManager, playerTransform, monoBehaviour, animator));
        States.Add(PlayerStateType.SolarBeam_MagicState, new SolarBeam_MagicState(PlayerStateType.SolarBeam_MagicState, this, playerInputManager, playerTransform, monoBehaviour, animator));
        States.Add(PlayerStateType.Storm_MagicState, new Storm_MagicState(PlayerStateType.Storm_MagicState, this, playerInputManager, playerTransform, monoBehaviour, animator));
        States.Add(PlayerStateType.EarthQuake_MagicState, new EarthQuake_MagicState(PlayerStateType.EarthQuake_MagicState, this, playerInputManager, playerTransform, monoBehaviour, animator, rb));

        PushState(PlayerStateType.Idle);

    }

}
