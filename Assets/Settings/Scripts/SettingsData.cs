using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class SettingsData 
{
    public float fx;
    public float music;
    public int res;
    public int quality;

    // Start is called before the first frame update
    public SettingsData(SettingsManager setMan)
    {
        this.fx = setMan.FxVolumeSlider.value;
        this.music = setMan.MusicVolumeSlider.value;
        this.res = setMan.ResSelector.value;
        this.quality = setMan.QualitySelector.value;
    }
}
