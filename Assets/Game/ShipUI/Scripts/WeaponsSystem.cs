using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsSystem : MonoBehaviour
{
    [SerializeField]
    private float range;
    public float Range
    {
        get { return range; }
        set { range = value; }
    }
    public string displayName = "weapon_name";
    [SerializeField]
    private Color firedColor;
    private Color initialColor;

    private bool fired = false;
    public bool Fired
    {
        get { return fired; }
        set
        {
            gameObject.GetComponent<Image>().color = (value) ? firedColor : initialColor;
            fired = value;
        }
    }

    public ShipSystem getShipSystem()
    {
        return gameObject.GetComponent<ShipSystem>();
    }

    // If the weapons are clicked alert the section ui manager
    public void alertWeaponsClicked()
    {
        if (!fired)
        {
            getShipSystem().sectionUI.mouseClickedOnWeapons(this);
        }
    }

    private void Awake()
    {
        initialColor = gameObject.GetComponent<Image>().color;
    }
}
