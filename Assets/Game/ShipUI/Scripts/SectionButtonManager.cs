using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TbsFramework.Grid;
using TbsFramework.Grid.GridStates;
using TbsFramework.Units;

public class SectionButtonManager : MonoBehaviour
{
    private Dictionary<PowerButton.ButtonType, PowerButton> buttons = new Dictionary<PowerButton.ButtonType, PowerButton>();
    [NonSerialized]
    public PowerButton.ButtonType buttonState = PowerButton.ButtonType.None;

    [NonSerialized]
    public CellGrid cellGrid;

    [SerializeField]
    private GameObject statsDisplayObject;
    [SerializeField]
    private GameObject shipSectionPanel;
    [SerializeField]
    private GameObject weaponInfoPanel;
    [SerializeField]
    private GameObject nextTurnPanel;

    private GameObject selectedShip;
    public Unit SelectedShipUnit { get; private set; }

    public void setSelectedShip(GameObject selectedShip, Unit selectedShipUnit)
    {
        this.selectedShip = selectedShip;
        this.SelectedShipUnit = selectedShipUnit;
        // The buttons should not be clickable if no ship is selected
        bool interactable = selectedShip;
        foreach (PowerButton button in buttons.Values)
        {
            getUiButton(button).interactable = interactable;
            button.selectedShip = selectedShip;
        }
        if (selectedShip)
        {
            disableDiversionIfDiverted();
        }
    }

    // Gets the Button component of the PowerButton's gameobject
    private Button getUiButton(PowerButton button)
    {
        return button.gameObject.GetComponent<Button>();
    }

    // Add the relavent child buttons to the dictionary
    private void Awake()
    {
        shipSectionPanel.GetComponent<SectionUIManager>().buttonManager = this;
        PowerButton[] childButtons = GetComponentsInChildren<PowerButton>(true);
        foreach (PowerButton button in childButtons)
        {
            button.statsDisplayObject = statsDisplayObject;
            button.shipSectionPanel = shipSectionPanel;
            button.weaponInfoPanel = weaponInfoPanel;
            buttons.Add(button.buttonType, button);
        }
        setSelectedShip(selectedShip, SelectedShipUnit); // Ensure the setter has run
    }

    public void sectionButtonClicked(PowerButton.ButtonType btnState)
    {
        PowerButton button = buttons[btnState];
        // Close window if open
        if (buttonState == btnState)
        {
            button.closeWindow();
            disableDiversionIfDiverted();
            buttonState = PowerButton.ButtonType.None;
            nextTurnPanel.SetActive(true);
            cellGrid.CellGridState = new CellGridStateUnitSelected(cellGrid, SelectedShipUnit);
        }
        // If another window is open, close it then open the chosen one
        else if (buttonState != PowerButton.ButtonType.None)
        {
            buttons[buttonState].closeWindow();
            disableDiversionIfDiverted();
            if (getUiButton(button).interactable)
            {
                cellGrid.CellGridState = new CellGridStateBlockInput(cellGrid);
                button.openWindow();
                buttonState = btnState;
                nextTurnPanel.SetActive(false);
            }
            else
            {
                buttonState = PowerButton.ButtonType.None;
                nextTurnPanel.SetActive(true);
                cellGrid.CellGridState = new CellGridStateUnitSelected(cellGrid, SelectedShipUnit);
            }
        }
        else
        {
            cellGrid.CellGridState = new CellGridStateBlockInput(cellGrid);
            button.openWindow();
            buttonState = btnState;
            nextTurnPanel.SetActive(false);
        }
    }

    public void fireButtonClicked()
    {
        WeaponsSystem weapon = weaponInfoPanel.GetComponent<WeaponInfoDisplay>().selectedSystem;
        if (weapon)
        {
            buttons[buttonState].closeWindow();
            buttonState = PowerButton.ButtonType.None;
            cellGrid.CellGridState = new CellGridStateFiring(cellGrid, SelectedShipUnit, weapon);
            nextTurnPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Fire button clicked but no weapon system assigned!");
        }
    }

    private void disableDiversionIfDiverted()
    {
        bool diverted = selectedShip.GetComponent<ShipSectionManager>().PowerDiverted;
        getUiButton(buttons[PowerButton.ButtonType.PowerDiversion]).interactable = !diverted;
    }

    public void resetShipSelection()
    {
        setSelectedShip(null, null);
    }
}