using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPowerCore : MonoBehaviour
{
    [HideInInspector]
    public SectionUIManager powerDivertor;
    [HideInInspector]
    public PowerBeam powerBeam;

    private ShipSystem boostTarget;
    public ShipSystem BoostTarget
    {
        get { return boostTarget; }
        set
        {
            if (boostTarget)
            {
                boostTarget.currentValue -= boostTarget.boostIncrease;
            }
            boostTarget = value;
            if (boostTarget)
            {
                boostTarget.currentValue += boostTarget.boostIncrease;
            }
        }
    }

    public void Awake()
    {
        powerBeam = GetComponentInChildren<PowerBeam>(true);
    }

    // When the power core is shown, show the beam as well
    public void OnEnable()
    {
        if (boostTarget && powerDivertor)
        {
            powerBeam.gameObject.SetActive(true);
        }
    }

    // When the mouse is released over this object alert the power screen panel.
    public void alertMouseDown()
    {
        powerDivertor.mouseDownOnPC(this);
    }

    // When the mouse is released over this object alert the power screen panel.
    public void alertMouseUp()
    {
        powerDivertor.GetComponent<SectionUIManager>().mouseUpOnPC(this);
    }
}
