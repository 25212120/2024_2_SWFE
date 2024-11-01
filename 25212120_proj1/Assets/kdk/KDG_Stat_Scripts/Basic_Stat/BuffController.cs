using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffStat
{
    public float attack;
    public float movementSpeed;
    public float defense;

    public void Reset()
    {
        attack = 0f;
        movementSpeed = 0f;
        defense = 0f;
    }
}

public class BuffController
{
    public BuffStat BuffStat { get; private set; } = new BuffStat();

    private List<Buff> activeBuffs = new List<Buff>();

    public void ApplyBuff(Buff newBuff)
    {
        activeBuffs.Add(newBuff);
        UpdateBuffStats();
    }

    public void RemoveBuff(Buff buffToRemove)
    {
        if (activeBuffs.Remove(buffToRemove))
        {
            UpdateBuffStats();
        }
    }

    private void UpdateBuffStats()
    {
        BuffStat.Reset();
        foreach (var buff in activeBuffs)
        {
            BuffStat.attack += buff.attack;
            BuffStat.movementSpeed += buff.movementSpeed;
            BuffStat.defense += buff.defense;
        }
    }
}

[System.Serializable]
public class Buff
{
    public float attack;
    public float movementSpeed;
    public float defense;

    public Buff(float attack, float movementSpeed, float defense)
    {
        this.attack = attack;
        this.movementSpeed = movementSpeed;
        this.defense = defense;
    }
}
