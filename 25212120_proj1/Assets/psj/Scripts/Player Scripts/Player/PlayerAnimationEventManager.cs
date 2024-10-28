using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAnimationEventManager : MonoBehaviour
{
    public Rigidbody rb;
    public PlayerInputManager playerInputManager;
    public Transform playerTransform;
    public Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInputManager = GetComponent<PlayerInputManager>();
        playerTransform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }
    public void MovingWhileAttacking()
    {
        rb.AddForce(playerTransform.forward * 6f, ForceMode.Impulse);
    }
    public void MovingWhileAttacking_DoubleSwords()
    {
        rb.AddForce(playerTransform.forward * 7f, ForceMode.Impulse);
    }
    public void BackStepWhileAttacking_Bow()
    {
        rb.AddForce(playerTransform.forward * -15f, ForceMode.Impulse);
    }
    public void JumpAttackStart()
    {
        rb.useGravity = false;
        rb.AddForce(playerTransform.up * 15f, ForceMode.Impulse);
    }
    public void JumpAttackEnd()
    {
        rb.useGravity = true;
        rb.AddForce(playerTransform.up * -15f, ForceMode.Impulse);
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
        Debug.Log("ROTATED");
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

}
