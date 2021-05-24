using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider FxVolumeSlider;
    public Slider MusicVolumeSlider;
    public Dropdown ResSelector;
    public Dropdown QualitySelector;

    private Resolution[] _res;

    private int _selRes;
    private int _selQuality;

    public void LoadResolutions()
    {
        List<string> resOptions = new List<string>();

        ResSelector.ClearOptions();

        foreach (Resolution res in _res)
            resOptions.Add(res.width + " x " + res.height);

        ResSelector.AddOptions(resOptions);

        ResSelector.value = _selRes;
        ResSelector.RefreshShownValue();
    }

    public Resolution getRes()
    {
        return _res[_selRes];
    }

    public void setRes(int resIndex)
    {
        Resolution res = _res[resIndex];
        _selRes = resIndex;

        Screen.SetResolution(res.width, res.height, true);
    }

    public int getQuality()
    {
        return _selQuality;
    }

    public void setQuality(int qualityIndex)
    {
        _selQuality = qualityIndex;

        QualitySettings.SetQualityLevel(_selQuality);
    }

    public void Save()
    {
        IOSettings.SaveSettings(this);
    }

    public void Load()
    {
        // Set resolutions to all currently supported (shouldn't change
        // for mobile devices so don't have to worry about saved index 
        // mismatches
        _res = Screen.resolutions;

        // Load settings
        SettingsData data = IOSettings.LoadSettings();

        // Set fx volume from settings
        FxVolumeSlider.value = data.fx;

        // Set music volume from settings
        MusicVolumeSlider.value = data.music;

        // Set resolution from settings
        _selRes = data.res;
        ResSelector.value = _selRes;

        // Set quality from settings
        _selQuality = data.quality;
        QualitySelector.value = _selQuality;
    }
}
