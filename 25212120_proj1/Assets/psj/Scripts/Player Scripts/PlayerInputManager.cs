using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Profiling.Memory.Experimental;
using Photon.Pun;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] public GameObject[] rightHand_Weapons;
    [SerializeField] public GameObject[] leftHand_Weapons;
    [SerializeField] public RuntimeAnimatorController[] player_animControllers;
    [SerializeField] public GameObject[] magicRangeSprites;
    public GameObject magicRangeSprite;
    public Queue<string> inputQueue = new Queue<string>();
    public int IndexSwapTo = 0;
    public int currentRightHandIndex;
    public int currentLeftHandIndex;
    public int previousRightHandIndex;
    public int previousLeftHandIndex;
    public PlayerStateType magic1;
    public PlayerStateType magic2;
    public int currentMagicIndex;
    // CheckGround를 호출여부를 결정할 수 있음
    public bool wantToCheckGround = true;
    public bool isCollidingHorizontally = false;
    public bool isDead = false;
    public bool isDefending = false;
    // 데미지 주는 쪽에서 isHit = true해주면 hit animation재생가능
    public bool isHit = false;
    public bool canSwap = false;

    [Header("Magic Spawn Points")]
    [SerializeField] public Vector3[] magicSpawnPoints;

    // ChangeState인 경우 (  )Key_Pressed 변수를 설정하고 is(   )는 State 스크립트 내부적으로 변경
    [Header("Player Movement Inputs")]
    public Vector2 moveInput;
    public bool leftShiftKey_Pressed = false;

    [Header("Player Action Inputs")]
    public bool leftButton_Pressed = false;
    public bool magicInput = false;
    public bool chargeInput = false;

    // PushState인 경우 is(    )만 만들고 Stata 스크립트 내부적으로 변경
    [Header("Player Action Handlers")]
    public bool isGrounded = true;
    public bool isAttacking = false;
    public bool isPerformingAction = false;

    public Vector3 magicPoint;

    private Rigidbody rb;
    private Animator animator;
    private Transform playerTransform;
    private PlayerMovement playerInput;
    private PlayerStat playerStat;
    private PlayerCoolDownManager playerCoolDown;
    private EquipmentInventory equipmentInventory;

    private StateManager<PlayerStateType> stateManager;

    public GameObject dim;

    private void Awake()
    {
        playerInput = new PlayerMovement();

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerTransform = GetComponent<Transform>();
        playerStat = GetComponent<PlayerStat>();
        playerCoolDown = GetComponent<PlayerCoolDownManager>();
        equipmentInventory = GetComponent<EquipmentInventory>();

        stateManager = GetComponent<StateManager<PlayerStateType>>();

        currentRightHandIndex = 0;
        currentLeftHandIndex = 0;

        // magic init (for test)
        Magic1Swap(PlayerStateType.PoisonFog_MagicState);
        Magic2Swap(PlayerStateType.DrainField_MagicState);
    }

    private void Start()
    {
        GenerateSpawnPoints();
        PhotonNetwork.SerializationRate = 60;
    }

    private void Update()
    {
        //Debug.Log(dim.activeSelf);
        //Debug.Log("LeftButton_Pressed : " + leftButton_Pressed);
        //Debug.Log(isPeformingAction);
        //GetHitCheck();
        /*
        hpCheck();
        */
        leftButton_Pressed = false;
    }
    private void FixedUpdate()
    {
        if (wantToCheckGround)
        {
            GroundCheck();
        }
    }

    private void OnEnable()
    {

        //if (photonView.IsMine == false) return;

        playerInput.Enable();

        playerInput.PlayerMove.Move.performed += OnMovePerformed;
        playerInput.PlayerMove.Move.canceled += OnMoveCanceled;

        playerInput.PlayerMove.Sprint.performed += OnSprintPerformed;
        playerInput.PlayerMove.Sprint.canceled += OnSprintCanceled;

        playerInput.PlayerAction.Dash.performed += OnDashPerformed;

        playerInput.PlayerAction.Jump.performed += OnJumpPerformed;

        playerInput.PlayerAction.Attack.performed += OnAttackPerformed;

        playerInput.PlayerAction.WeaponSkill.performed += OnWeaponSkillPerformed;
        playerInput.PlayerAction.Charge.performed += OnChargePerformed;
        playerInput.PlayerAction.Charge.canceled += OnChargeCanceled;

        playerInput.PlayerAction.Interaction.performed += OnInteractionPerformed;
        playerInput.PlayerAction.Interaction.canceled += OnInteractionCanceled;

        playerInput.WeaponSwap.SwordAndShield.performed += OnSwapToSwordAndShieldPerformed;
        playerInput.WeaponSwap.SingleTwoHandeSword.performed += OnSwapToSingleTwoHandSwordPerformed;
        playerInput.WeaponSwap.DoubleSwords.performed += OnSwapToDoubleSwordsPerformed;
        playerInput.WeaponSwap.BowAndArrow.performed += OnSwapToBowAndArrowPerformed;

        playerInput.PlayerMagic.Magic1.performed += OnMagic1Performed;
        playerInput.PlayerMagic.Magic2.performed += OnMagic2Performed;
    }
    private void OnDisable()
    {
        // **************************************************
        //if(photonView.IsMine == false) return;
        // **************************************************

        playerInput.Disable();

        playerInput.PlayerMove.Move.performed -= OnMovePerformed;
        playerInput.PlayerMove.Move.canceled -= OnMoveCanceled;

        playerInput.PlayerMove.Sprint.performed -= OnSprintPerformed;
        playerInput.PlayerMove.Sprint.canceled -= OnSprintCanceled;

        playerInput.PlayerAction.Dash.performed -= OnDashPerformed;

        playerInput.PlayerAction.Jump.performed -= OnJumpPerformed;

        playerInput.PlayerAction.Attack.performed -= OnAttackPerformed;

        playerInput.PlayerAction.WeaponSkill.performed -= OnWeaponSkillPerformed;
        playerInput.PlayerAction.Charge.performed -= OnChargePerformed;
        playerInput.PlayerAction.Charge.canceled -= OnChargeCanceled;

        playerInput.PlayerAction.Interaction.performed -= OnInteractionPerformed;

        playerInput.WeaponSwap.SwordAndShield.performed -= OnSwapToSwordAndShieldPerformed;
        playerInput.WeaponSwap.SingleTwoHandeSword.performed -= OnSwapToSingleTwoHandSwordPerformed;
        playerInput.WeaponSwap.DoubleSwords.performed -= OnSwapToDoubleSwordsPerformed;
        playerInput.WeaponSwap.BowAndArrow.performed -= OnSwapToSwordAndShieldPerformed;

        playerInput.PlayerMagic.Magic1.performed -= OnMagic1Performed;
        playerInput.PlayerMagic.Magic2.performed -= OnMagic2Performed;
    }

    public GameObject spawnPrefab;
    private int angleStep = 20;
    private float radius = 30f;

    void GenerateSpawnPoints()
    {
        // 360도를 각도 간격으로 나눔
        for (int angle = 0; angle < 360; angle += angleStep)
        {
            // 각도를 라디안으로 변환
            float radian = angle * Mathf.Deg2Rad;

            // 원 위의 좌표 계산
            Vector3 spawnPosition = new Vector3(
                Mathf.Cos(radian) * radius,
                0f, // Y축은 0으로 고정 (2D 평면 상)
                Mathf.Sin(radian) * radius
            );

            // 스폰 포인트 생성
            GameObject instanatiatedSpawner = Instantiate(spawnPrefab, spawnPosition + transform.position, Quaternion.identity);
            instanatiatedSpawner.transform.SetParent(transform, false);
        }
    }

    private void hpCheck()
    {
        float currentHp = playerStat.GetCurrentHP();
        if (currentHp <= 0)
        {
            stateManager.PushState(PlayerStateType.Die);
        }
    }
    private void GroundCheck()
    {
        RaycastHit hit;
        float rayDistance = 0.5f;
        Vector3 origin = playerTransform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(origin, Vector3.down, out hit, rayDistance))
        {
            // 땅에 닿음
            if (hit.collider != null && hit.collider.CompareTag("Ground"))
            {
                if (isGrounded == false)
                {
                    isGrounded = true;
                    animator.SetBool("isInAir", false);
                }
            }
        }
        else
        {
            if (isGrounded)
            {
                isGrounded = false;
                animator.SetBool("isInAir", true);
            }
        }
    }
    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        if (isPerformingAction == false)
        {
            moveInput = ctx.ReadValue<Vector2>();
            animator.SetBool("moveInput", true);
        }
    }
    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
        animator.SetBool("moveInput", false);
    }
    private void OnSprintPerformed(InputAction.CallbackContext ctx)
    {
        leftShiftKey_Pressed = true;
        animator.SetBool("leftShiftKey_Pressed", true);
    }
    private void OnSprintCanceled(InputAction.CallbackContext ctx)
    {
        leftShiftKey_Pressed = false;
        animator.SetBool("leftShiftKey_Pressed", false);
    }
    private void OnDashPerformed(InputAction.CallbackContext ctx)
    {
        if (playerCoolDown.CanDash(currentRightHandIndex) && isGrounded && !isPerformingAction && !isAttacking)
        {
            stateManager.PushState(PlayerStateType.Dash);
        }
    } 
    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        if (isGrounded && !isPerformingAction && !isAttacking)
        {
            stateManager.PushState(PlayerStateType.Jump);
        }
    }

    private void GetHitCheck()
    {
        if (isHit == true)
        {
            stateManager.PushState(PlayerStateType.Hit);
        }
    }
    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {

        if (isGrounded && !isPerformingAction)
        {
            leftButton_Pressed = true;

            if (!isAttacking)
            {
                stateManager.PushState(PlayerStateType.Attack);
                animator.SetTrigger("leftButton_Pressed");
            }
            else if (inputQueue.Count < 1)
            {
                inputQueue.Enqueue("NextCombo");
            }
        }
    }
    private void OnWeaponSkillPerformed(InputAction.CallbackContext ctx)
    {
        if (playerCoolDown.CanUseWeaponSkill(currentRightHandIndex) && isGrounded && !isPerformingAction)
        {
            if (currentLeftHandIndex != 3)
            {
                stateManager.PushState(PlayerStateType.WeaponSkill);
                animator.SetTrigger("rightButton_Pressed");
            }
        }
    }
    private void OnChargePerformed(InputAction.CallbackContext ctx)
    {
        if (currentLeftHandIndex == 3 && isPerformingAction == false)
        {
            chargeInput = true;
            stateManager.PushState(PlayerStateType.WeaponSkill);
            animator.SetTrigger("rightButton_Pressed");
        }
    }
    private void OnChargeCanceled(InputAction.CallbackContext ctx)
    {
        if (currentLeftHandIndex == 3)  chargeInput = false;
    }
    private void OnInteractionPerformed(InputAction.CallbackContext ctx)
    {
        if (isGrounded && !isPerformingAction && !isAttacking)
        {
            stateManager.PushState(PlayerStateType.Interaction);
        }
    }

    public bool interactionFinished = false;
    private void OnInteractionCanceled(InputAction.CallbackContext ctx)
    {
            stateManager.ChangeState(PlayerStateType.Idle);
    }
    private void OnSwapToSwordAndShieldPerformed(InputAction.CallbackContext ctx)
    {
        if (canSwap == false) return;

        if (isGrounded && !isPerformingAction && !isAttacking)
        {
            IndexSwapTo = 0;
            stateManager.PushState(PlayerStateType.WeaponSwap);
            equipmentInventory.Equip(equipmentInventory.availableEquipments[currentRightHandIndex]);
        }
    }
    private void OnSwapToSingleTwoHandSwordPerformed(InputAction.CallbackContext ctx)
    {
        if (canSwap == false) return;

        if (isGrounded && !isPerformingAction && !isAttacking)
        {
            IndexSwapTo = 1;
            stateManager.PushState(PlayerStateType.WeaponSwap);
            equipmentInventory.Equip(equipmentInventory.availableEquipments[currentRightHandIndex]);
        }
    }
    private void OnSwapToDoubleSwordsPerformed(InputAction.CallbackContext ctx)
    {
        if (canSwap == false) return;

        if (isGrounded && !isPerformingAction && !isAttacking)
        {
            IndexSwapTo = 2;
            stateManager.PushState(PlayerStateType.WeaponSwap);
            equipmentInventory.Equip(equipmentInventory.availableEquipments[currentRightHandIndex]);
        }
    }
    private void OnSwapToBowAndArrowPerformed(InputAction.CallbackContext ctx)
    {
        if (canSwap == false) return;

        if (isGrounded && !isPerformingAction && !isAttacking)
        {
            IndexSwapTo = 3;
            stateManager.PushState(PlayerStateType.WeaponSwap);
            equipmentInventory.Equip(equipmentInventory.availableEquipments[currentRightHandIndex]);
        }
    }

    private void OnMagic1Performed(InputAction.CallbackContext ctx)
    {
        //if (isGrounded && !isPerformingAction && !isAttacking && playerCoolDown.CanUseMagic(magic1))
        if (isGrounded && !isPerformingAction && !isAttacking)
        {
            Debug.Log("in");
            currentMagicIndex = 1;
            PushByMagicCase(magic1);
        }
    }

    private void OnMagic2Performed(InputAction.CallbackContext ctx)
    {
        if (isGrounded && !isPerformingAction && !isAttacking && playerCoolDown.CanUseMagic(magic2))
        {
            currentMagicIndex = 2;
            PushByMagicCase(magic2);
        }
    }

    public void Magic1Swap(PlayerStateType type)
    {
        if (magic1 == type || magic2 == type) return;
        magic1 = type;
    }
    public void Magic2Swap(PlayerStateType type)
    {
        if (magic1 == type || magic2 == type) return;
        magic2 = type;
    }


    private void PushByMagicCase(PlayerStateType magicIndex)
    {
        switch (magicIndex)
        {
            // Fire
            case PlayerStateType.FireBall_MagicState:
                Debug.Log("Fireball In");
                stateManager.PushState(PlayerStateType.Scope_MagicState);
                break;
            case PlayerStateType.Meteor_MagicState:
                stateManager.PushState(PlayerStateType.Scope_MagicState);
                break;
            // Earth
            case PlayerStateType.RockFall_MagicState:
                stateManager.PushState(PlayerStateType.RockFall_MagicState);
                break;
            case PlayerStateType.EarthQuake_MagicState:
                stateManager.PushState(PlayerStateType.EarthQuake_MagicState);
                break;
            // Water
            case PlayerStateType.IceSpear_MagicState:
                stateManager.PushState(PlayerStateType.Scope_MagicState);
                break;
            case PlayerStateType.Storm_MagicState:
                stateManager.PushState(PlayerStateType.Storm_MagicState);
                break;
            // Plant
            case PlayerStateType.PoisonFog_MagicState:
                stateManager.PushState(PlayerStateType.Scope_MagicState);
                break;
            case PlayerStateType.DrainField_MagicState:
                stateManager.PushState(PlayerStateType.DrainField_MagicState);
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
            foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 normal = contact.normal;

            if (Mathf.Abs(normal.y) < 0.1f)
            {
                isCollidingHorizontally = true;
                break;
            }
        }
    }


    public int GetElement1Index()
    {
        switch (magic1)
        {
            case PlayerStateType.PoisonFog_MagicState:
                return 0;
            case PlayerStateType.DrainField_MagicState:
                return 0;
            case PlayerStateType.FireBall_MagicState:
                return 1;
            case PlayerStateType.Meteor_MagicState:
                return 1;
            case PlayerStateType.IceSpear_MagicState:
                return 2;
            case PlayerStateType.Storm_MagicState:
                return 2;
            case PlayerStateType.RockFall_MagicState:
                return 3;
            case PlayerStateType.EarthQuake_MagicState:
                return 3;
            default:
                return -1;
        }
    }
    
    public int GetElement2Index()
    {
        switch (magic2)
        {
            case PlayerStateType.PoisonFog_MagicState:
                return 0;
            case PlayerStateType.DrainField_MagicState:
                return 0;
            case PlayerStateType.FireBall_MagicState:
                return 1;
            case PlayerStateType.Meteor_MagicState:
                return 1;
            case PlayerStateType.IceSpear_MagicState:
                return 2;
            case PlayerStateType.Storm_MagicState:
                return 2;
            case PlayerStateType.RockFall_MagicState:
                return 3;
            case PlayerStateType.EarthQuake_MagicState:
                return 3;
            default:
                return -1;
        }
    }

}

