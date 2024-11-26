using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EXPLEVEL : MonoBehaviour
{
    [SerializeField] public PlayerStat player;
    [SerializeField] private TextMeshProUGUI Level;

    private void Awake()
    {
        StartCoroutine(initializePlayer());
    }

    IEnumerator initializePlayer()
    {
        while (GameManager.instance.player == null)
        {
            yield return null;
        }
        player = GameManager.instance.player.GetComponent<PlayerStat>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            Level.text = (player.GetLevel()).ToString();
        }
    }
}
