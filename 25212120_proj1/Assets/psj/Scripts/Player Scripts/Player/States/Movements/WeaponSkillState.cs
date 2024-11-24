using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WeaponSkillState : BaseState<PlayerStateType>
{
    private Animator animator;
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private PlayerStat playerStat;
    private Rigidbody rb;
    private MonoBehaviour monoBehaviour;

    public WeaponSkillState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Rigidbody rb, Animator animator, Transform playerTransform, MonoBehaviour monoBehaviour, PlayerStat playerStat)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.animator = animator;
        this.playerInputManager = inputManager;
        this.rb = rb;
        this.monoBehaviour = monoBehaviour;
        this.playerStat = playerStat;
    }

    private bool finishedWeaponSkill = false;

    private float chargeTime = 2f;
    private float chargedTime = 0f;

    private LayerMask enemyLayer;

    public override void EnterState()
    {
        playerInputManager.isPerformingAction = true;
        PerformWeaponSkill(playerInputManager.currentRightHandIndex);
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    public override void UpdateState()
    {
        if (playerInputManager.currentRightHandIndex == 3)
        {
            chargedTime += Time.deltaTime;
            RotatePlayerTowardsMouse();
        }
        if (chargedTime >= chargeTime) Debug.Log("ChargeFinished");
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()    
    {
        finishedWeaponSkill = false;
        chargedTime = 0f;
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
                ChargeShot_Bow();
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
                    playerInputManager.isPerformingAction = false;
                    stateManager.PopState();
                }
                break;
            case 1:
                if (stateInfo.IsTag("WeaponSkill") && stateInfo.normalizedTime >= 0.95f)
                {
                    playerInputManager.wantToCheckGround = true;
                    animator.SetTrigger("finishedWeaponSkill");
                    playerInputManager.isPerformingAction = false;

                    stateManager.PopState();
                }
                break;
            case 2:
                if (stateInfo.IsTag("WeaponSkill") && stateInfo.normalizedTime > 0.95f)
                {
                    animator.SetTrigger("finishedWeaponSkill");
                    playerInputManager.isPerformingAction = false;

                    stateManager.PopState();
                }
                break;
            case 3:
                if (playerInputManager.chargeInput == false)
                {
                    if (chargedTime >= chargeTime)
                    {
                        // 화살발사
                        animator.SetTrigger("finishedCharging");
                    }

                    if (stateInfo.IsTag("WeaponSkill") && stateInfo.normalizedTime >= 0.99f)
                    {
                        animator.SetTrigger("finishedWeaponSkill");
                        playerInputManager.isPerformingAction = false;
                        stateManager.PopState();
                    }
                    else if (stateInfo.IsTag("Charge") || stateInfo.IsTag("Charge Start"))
                    {
                        animator.SetTrigger("finishedWeaponSkill");
                        playerInputManager.isPerformingAction = false;
                        stateManager.PopState();
                    }

                }
                break;
        }
    }

    private void Defend_SwordShield()
    {
        monoBehaviour.StartCoroutine(Defend());
        // 도균이의 전투시스템이 완성될 때 로직 추가
    }

    private IEnumerator Defend()
    {
        playerInputManager.isDefending = true;

        yield return new WaitForSeconds(3f);
        playerInputManager.isDefending = false;

        finishedWeaponSkill = true;

    }

    private void JumpAttack_SingleTwoHandedSword()
    {
        monoBehaviour.StartCoroutine(jumpAtk());
    }

    private IEnumerator jumpAtk()
    {
        yield return new WaitForSeconds(1f);
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, 4f, enemyLayer);

        foreach (Collider collider in hitColliders)
        {
            if (collider != null)
            {
                BaseMonster enemyStat = collider.GetComponent<BaseMonster>();
                playerStat.Attack(enemyStat);

                Transform enemyTransform = collider.GetComponent<Transform>();
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                NavMeshAgent agent = collider.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.enabled = false;
                }
                float distance = Vector3.Distance(playerTransform.position, enemyTransform.position);
                if (distance <= 4f)
                {
                    Vector3 knockbackDirection = (enemyTransform.position - playerTransform.position).normalized;
                    if (rb != null)
                    {
                        rb.AddForce(knockbackDirection * 40f, ForceMode.Impulse);
                    }
                }
                if (agent != null)
                {
                    agent.enabled = true;
                }
            }
        }
    }

    private void Rage_DoubleSwords()
    {
        // 애니메이션 (이펙트)
        BuffController buffController = playerInputManager.GetComponent<BuffController>();
        Buff rage = new Buff(5f, 0f, 0f);
        monoBehaviour.StartCoroutine(RageBuff(buffController, rage, 10f));
       
        // 스탯딸깍
    }

    private IEnumerator RageBuff(BuffController buffController, Buff buff, float time)
    {
        buffController.ApplyBuff(buff);
        yield return new WaitForSeconds(time);
        buffController.RemoveBuff(buff);
    }

    private void ChargeShot_Bow()
    {
        // 이펙트
    }


    private void RotatePlayerTowardsMouse()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        Vector3 direction = mouseWorldPosition - playerTransform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float rotationSpeed = 50f;
            playerTransform.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    private Vector3 GetMouseWorldPosition()
    {
        if (Camera.main == null)
        {
            return Vector3.zero;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Environment")))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
}
