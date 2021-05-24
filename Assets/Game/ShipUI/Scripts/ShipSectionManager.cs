using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSectionManager : MonoBehaviour
{
    [NonSerialized]
    public GameObject powerScreenPanel; // The panel the power diversion UI should sit in
    public GameObject sectionContainer; // The parent gameobject of the ship sections

    [NonSerialized]
    public bool PowerDiverted;

    private ShipSection[] sections;

    public void init()
    {
        // Move the ship sections onto the UI
        sectionContainer.transform.SetParent(powerScreenPanel.transform);
        sectionContainer.transform.localPosition = Vector3.zero;
        sectionContainer.transform.localRotation = Quaternion.Euler(Vector3.zero);
        sectionContainer.transform.localScale = Vector3.one;
        sectionContainer.SetActive(false);
        updateSections();
    }

    // Update the list of ship sections - should only need to be called once
    public void updateSections()
    {
        sections = sectionContainer.GetComponentsInChildren<ShipSection>();
        foreach (ShipSection section in sections)
        {
            section.init(powerScreenPanel.GetComponent<SectionUIManager>());
        }
    }

    // Calculate the base value for a stat across all ship sections
    public float calculateBaseStatTotal(ShipStat stat)
    {
        float total = 0f;
        foreach (ShipSection section in sections)
        {
            total += section.calculateStat(stat);
        }
        return total;
    }

    public void resetWeapons()
    {
        foreach (ShipSection section in sections)
        {
            section.resetWeapons();
        }
    }
}
