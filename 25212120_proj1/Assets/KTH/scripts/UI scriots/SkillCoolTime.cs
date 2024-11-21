using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldown : MonoBehaviour
{
    public Image cooldownImage; // UI Image component representing cooldown
    public float cooldownTime = 5.0f; // Skill cooldown time in seconds
    private float cooldownTimer = 0.0f;
    private bool isOnCooldown = false;

    void Update()
    {
        // If skill is on cooldown, update the timer and UI
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                cooldownTimer = 0;
                isOnCooldown = false;
            }
            UpdateCooldownUI();
        }
    }

    // Function to use the skill
    public void UseSkill()
    {
        if (!isOnCooldown)
        {
            // Trigger skill logic here (e.g., damage enemies, etc.)
            Debug.Log("Skill used!");

            // Start cooldown
            cooldownTimer = cooldownTime;
            isOnCooldown = true;
        }
        else
        {
            Debug.Log("Skill is on cooldown!");
        }
    }

    // Update the cooldown UI based on remaining time
    void UpdateCooldownUI()
    {
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = cooldownTimer / cooldownTime;
        }
    }
}

