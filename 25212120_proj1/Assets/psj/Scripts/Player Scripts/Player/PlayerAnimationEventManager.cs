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


}
