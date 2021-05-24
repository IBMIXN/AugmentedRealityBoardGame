using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShipData : MonoBehaviour
{
    public Dictionary<ShipStat, float> MaxStats { get; } = new Dictionary<ShipStat, float>();
    private Dictionary<ShipStat, float> currentStats = new Dictionary<ShipStat, float>();

    private float baseHP;
    [SerializeField]
    private ShipTier shipTier;
    public ShipTier ShipTier
    {
        get { return shipTier; }
        private set { shipTier = value; }
    }

    public ShipSectionManager SectionManager { get; private set; }

    public void updateSectionManager()
    {
        SectionManager = gameObject.GetComponent<ShipSectionManager>();
    }

    public void init()
    {
        baseHP = 80 + 90 * (float)shipTier;
        SectionManager.init();

        // Ensure the dictionary has an entry for each stat
        foreach (ShipStat stat in Enum.GetValues(typeof(ShipStat)))
        {
            MaxStats.Add(stat, 0f);
        }
        recalculateAllStats();
        currentStats.Add(ShipStat.HP, MaxStats[ShipStat.HP]);
        currentStats.Add(ShipStat.Shields, MaxStats[ShipStat.Shields]);
        currentStats.Add(ShipStat.Armour, MaxStats[ShipStat.Armour]);
        currentStats.Add(ShipStat.Speed, MaxStats[ShipStat.Speed]);
    }

    public float getCurrentStatValue(ShipStat stat)
    {
        return currentStats[stat];
    }

    public void setCurrentStatValue(ShipStat stat, float value)
    {
        currentStats[stat] = Mathf.Clamp(value, 0, MaxStats[stat]);
        
    }

    public void setCurrentStatValueToMax(ShipStat stat)
    {
        currentStats[stat] = MaxStats[stat];
    }

    public void addToCurrentStatValue(ShipStat stat, float value)
    {
        setCurrentStatValue(stat, currentStats[stat] + value);
    }

    public float getCurrentDurability()
    {
        return currentStats[ShipStat.Shields] + currentStats[ShipStat.Armour] + currentStats[ShipStat.HP];
    }

    public float getMaxDurability()
    {
        return MaxStats[ShipStat.Shields] + MaxStats[ShipStat.Armour] + MaxStats[ShipStat.HP];
    }

    // Calculates the values of a given stat and updates the stat dictionary
    public float recalculateStat(ShipStat stat)
    {
        float value = 0f;
        // Some stats have a base value that needs to be added to the value from the sections
        switch (stat)
        {
            case ShipStat.HP:
                value += baseHP;
                break;
        }
        value += SectionManager.calculateBaseStatTotal(stat);
        MaxStats[stat] = value;
        return value;
    }

    // Calculates the values of all of the ships stats
    public void recalculateAllStats()
    {
        foreach (ShipStat stat in Enum.GetValues(typeof(ShipStat)))
        {
            float oldVal = MaxStats[stat];
            float newVal = recalculateStat(stat);
            if (oldVal != newVal && currentStats.ContainsKey(stat))
            {
                if (stat == ShipStat.Speed)
                {
                    // Speed just gets added on
                    addToCurrentStatValue(stat, (newVal - oldVal));
                }
                else
                {
                    // The current stat value should scale proportionally with the change
                    // If a ship is at 50% HP, then increasing or decreasing the max should preserve this percentage
                    setCurrentStatValue(stat, getCurrentStatValue(stat) * (newVal / oldVal));
                }
            }
        }
    }
}
