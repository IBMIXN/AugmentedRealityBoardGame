using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionScript : MonoBehaviour
{
    public TMPro.TextMeshProUGUI VersionDisplayLabel;

    void Start()
    {
        VersionDisplayLabel.text = "Version: " + Application.version;
    }
}
