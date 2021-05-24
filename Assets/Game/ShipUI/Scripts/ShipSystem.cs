using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSystem : MonoBehaviour
{
    public SystemID id;
    public ShipStat stat;
    public float baseValue = 0f;
    public float boostIncrease = 0f;
    [HideInInspector]
    public float currentValue = 0f;
    [HideInInspector]
    public SectionUIManager sectionUI;

    private void Awake()
    {
        currentValue = baseValue;
    }

    // When the mouse is released over this object alert the section ui manager
    public void alertMouseReleased()
    {
        sectionUI.mouseReleasedOnSystem(this);
    }
}
