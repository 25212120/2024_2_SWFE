using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventManager : MonoBehaviour
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
