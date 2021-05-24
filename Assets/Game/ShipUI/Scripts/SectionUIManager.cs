using System;
using System.Collections.Generic;
using UnityEngine;

public class SectionUIManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject selectedShip;
    [SerializeField]
    private GameObject beamCanvas;
    [SerializeField]
    private GameObject weaponInfoPanel;

    private ShipPowerCore selectedCore;

    [NonSerialized]
    public SectionButtonManager buttonManager;

    public void hideBeams()
    {
        PowerBeam[] beams = beamCanvas.GetComponentsInChildren<PowerBeam>();
        foreach (PowerBeam beam in beams)
        {
            beam.gameObject.SetActive(false);
        }
    }

    public void resetSelection()
    {
        if (buttonManager.buttonState == PowerButton.ButtonType.PowerDiversion)
        {
            selectedCore = null;
            powerDiverted();
        }
    }

    public void mouseReleasedOnSystem(ShipSystem system)
    {
        if (buttonManager.buttonState == PowerButton.ButtonType.PowerDiversion && selectedCore)
        {
            selectedCore.BoostTarget = system;
            selectedCore.powerBeam.transform.SetParent(beamCanvas.transform, false);
            selectedCore.powerBeam.drawBetweeen(
                selectedCore.transform.position, selectedCore.BoostTarget.transform.position);
            selectedCore.powerBeam.gameObject.SetActive(true);
            selectedCore = null;
            statChange();
            powerDiverted();
        }
    }

    public void mouseClickedOnWeapons(WeaponsSystem system)
    {
        if (buttonManager.buttonState == PowerButton.ButtonType.FireScreen && !system.Fired)
        {
            weaponInfoPanel.GetComponentInChildren<WeaponInfoDisplay>().showWeaponInfo(system);
            weaponInfoPanel.SetActive(true);
        }
    }

    public void mouseDownOnPC(ShipPowerCore core)
    {
        if (buttonManager.buttonState == PowerButton.ButtonType.PowerDiversion)
        {
            selectedCore = core;
            powerDiverted();
        }
    }

    public void mouseUpOnPC(ShipPowerCore core)
    {
        if (buttonManager.buttonState == PowerButton.ButtonType.PowerDiversion && selectedCore)
        {
            if (selectedCore.BoostTarget)
            {
                selectedCore.powerBeam.gameObject.SetActive(false);
                selectedCore.BoostTarget = null;
                statChange();
            }
            selectedCore = null;
        }
    }

    private void statChange()
    {
        selectedShip.GetComponent<ShipData>().recalculateAllStats();
        GetComponentInChildren<StatsDisplay>().animateAllStatChanges(selectedShip.GetComponent<ShipData>().MaxStats);
    }

    private void powerDiverted()
    {
        selectedShip.GetComponent<ShipSectionManager>().PowerDiverted = true;
    }
}