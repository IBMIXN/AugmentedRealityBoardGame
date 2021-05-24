using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ShipSection : MonoBehaviour
{
    public Dictionary<SystemID, ShipSystem> systemsDict = new Dictionary<SystemID, ShipSystem>();
    [NonSerialized]
    public ShipPowerCore powerCore;

    public void init(SectionUIManager powerDivertor)
    {
        // Get all children of the object with a SystemIdentifier script, and add them to the dictionary.
        try
        {
            ShipSystem[] children = GetComponentsInChildren<ShipSystem>();
            foreach (ShipSystem child in children)
            {
                child.sectionUI = powerDivertor;
                systemsDict.Add(child.id, child);
            }
        }
        catch (ArgumentException)
        {
            Debug.LogWarning("Cannot add multiple systems of the same type to a Ship Section!");
        }

        // Get the PowerCore if there is one
        ShipPowerCore[] cores = GetComponentsInChildren<ShipPowerCore>();
        foreach (ShipPowerCore core in cores)
        {
            if (powerCore)
            {
                Debug.LogWarning("A Ship Section cannot not have multiple PowerCores!");
            }
            powerCore = core;
            powerCore.powerDivertor = powerDivertor;
        }
    }

    // Calculate the total for a stat in this section
    public float calculateStat(ShipStat stat)
    {
        float total = 0f;
        foreach (ShipSystem system in systemsDict.Values)
        {
            if (system.stat == stat)
            {
                total += system.currentValue;
            }
        }
        return total;
    }

    public void resetWeapons()
    {
        if (systemsDict.ContainsKey(SystemID.Weapons))
        {
            systemsDict[SystemID.Weapons].gameObject.GetComponent<WeaponsSystem>().Fired = false;
        }
    }
}