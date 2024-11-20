using UnityEngine;

public class PlayerAnimationEventManager : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInputManager playerInputManager;
    private Transform playerTransform;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInputManager = GetComponent<PlayerInputManager>();
        playerTransform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }
    public void MovingWhileAttacking()
    {
        rb.AddForce(playerTransform.forward * 11f, ForceMode.Impulse);
    }
    public void MovingWhileAttacking_DoubleSwords()
    {
        rb.AddForce(playerTransform.forward * 9f, ForceMode.Impulse);
    }
    public void BackStepWhileAttacking_Bow()
    {
        rb.AddForce(playerTransform.forward * -15f, ForceMode.Impulse);
    }
    public void ChargedShotBakStep_Bow()
    {
        rb.AddForce(playerTransform.forward * -30f, ForceMode.Impulse);
    }
    public void JumpAttackStart()
    {
        rb.useGravity = false;
        rb.AddForce(playerTransform.up * 10f, ForceMode.Impulse);
    }
    public void JumpAttackEnd()
    {
        rb.useGravity = true;
        rb.AddForce(playerTransform.up * -25f, ForceMode.Impulse);
    }
    public void DisableWeapon()
    {
        playerInputManager.rightHand_Weapons[playerInputManager.previousRightHandIndex].SetActive(false);
        playerInputManager.leftHand_Weapons[playerInputManager.previousLeftHandIndex].SetActive(false);
    }
    public void EnableWeapon()
    {
        playerInputManager.rightHand_Weapons[playerInputManager.currentRightHandIndex].SetActive(true);
        playerInputManager.leftHand_Weapons[playerInputManager.currentLeftHandIndex].SetActive(true);
    }
    public void RotatePlayerTowardsMouse()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        Vector3 direction = mouseWorldPosition - playerTransform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float rotationSpeed = 100f;
            playerTransform.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    public Vector3 GetMouseWorldPosition()
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

    public void EnableWeaponCollider()
    {
        Collider collider = playerInputManager.rightHand_Weapons[playerInputManager.currentRightHandIndex].GetComponent<Collider>();
        collider.enabled = true;
    }
    public void EnableWeaponCollider1()
    {
        Collider collider = playerInputManager.rightHand_Weapons[playerInputManager.currentLeftHandIndex].GetComponent<Collider>();
        collider.enabled = true;
    }

    public void DisableWeaponCollider()
    {
        Collider collider = playerInputManager.rightHand_Weapons[playerInputManager.currentRightHandIndex].GetComponent<Collider>();
        collider.enabled = false;
    }

    public void DisableWeaponCollider1()
    {
        Collider collider = playerInputManager.leftHand_Weapons[playerInputManager.currentLeftHandIndex].GetComponent<Collider>();
        collider.enabled = true;
    }

}
