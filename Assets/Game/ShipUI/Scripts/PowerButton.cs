using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerButton : MonoBehaviour
{
    public enum ButtonType { PowerDiversion, FireScreen, None }

    [HideInInspector]
    public GameObject statsDisplayObject;
    [HideInInspector]
    public GameObject shipSectionPanel;
    [HideInInspector]
    public GameObject selectedShip;
    [HideInInspector]
    public GameObject weaponInfoPanel;

    public ButtonType buttonType;
    public Color powerScreenBGColor;

    public void openWindow()
    {
        if (selectedShip)
        {
            // Display the ship's stats
            selectedShip.GetComponent<ShipData>().recalculateAllStats();
            statsDisplayObject.GetComponent<StatsDisplay>().BoostedStats = selectedShip.GetComponent<ShipData>().MaxStats;

            // Show the ships sections and the power screen
            selectedShip.GetComponent<ShipSectionManager>().sectionContainer.SetActive(true);
            shipSectionPanel.GetComponent<SectionUIManager>().resetSelection();
            shipSectionPanel.GetComponent<SectionUIManager>().selectedShip = selectedShip;
            shipSectionPanel.GetComponent<Image>().color = powerScreenBGColor;
            shipSectionPanel.SetActive(true);
        }
        else
        {
            Debug.Log("The power screen button has been pressed but no ship is selected.");
        }
    }

    // Hide the power screen and the ship modules
    public void closeWindow()
    {
        shipSectionPanel.GetComponent<SectionUIManager>().hideBeams();
        shipSectionPanel.SetActive(false);
        selectedShip.GetComponent<ShipSectionManager>().sectionContainer.SetActive(false);
        weaponInfoPanel.SetActive(false);
    }

    // Alert the button manager that this button has been clicked
    public void onClick()
    {
        GetComponentInParent<SectionButtonManager>().sectionButtonClicked(buttonType);
    }
}