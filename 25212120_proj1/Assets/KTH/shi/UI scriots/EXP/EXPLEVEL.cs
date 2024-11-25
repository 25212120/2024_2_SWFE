using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EXPLEVEL : MonoBehaviour
{
    [SerializeField] private PlayerStat player;
    [SerializeField] private TextMeshProUGUI Level;

    // Update is called once per frame
    void Update()
    {
        Level.text = (player.GetLevel()).ToString();
    }
}
