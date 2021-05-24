using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInfoDisplay : MonoBehaviour
{
    public GameObject weaponNameText;
    public GameObject weaponInfoList;

    public WeaponsSystem selectedSystem;

    public void showWeaponInfo(WeaponsSystem system)
    {
        string weaponInfo = StatNames.ToString(system.getShipSystem().stat) + "\n\n";
        weaponInfo += system.getShipSystem().currentValue.ToString() + "\n\n";
        weaponInfo += system.Range.ToString();

        weaponInfoList.GetComponent<Text>().text = weaponInfo;
        weaponNameText.GetComponent<Text>().text = system.displayName;

        selectedSystem = system;
    }
}
