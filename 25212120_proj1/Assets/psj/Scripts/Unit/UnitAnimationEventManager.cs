using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitAnimationEventManager : MonoBehaviour
{
    public Collider meleeWeapon;

    public void EnableWeaponCollider()
    {
        meleeWeapon.enabled = true;
    }

    public void DisableWeaponCollider()
    {
        meleeWeapon.enabled = false;
    }
}
