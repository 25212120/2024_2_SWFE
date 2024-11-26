using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EXPLEVEL : MonoBehaviour
{
    [SerializeField] public PlayerStat player;
    [SerializeField] private TextMeshProUGUI Level;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        Level.text = (player.GetLevel()).ToString();
    }
}
