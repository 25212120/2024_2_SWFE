using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFallNova : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyGameOBJ());
    }

    IEnumerator DestroyGameOBJ()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
