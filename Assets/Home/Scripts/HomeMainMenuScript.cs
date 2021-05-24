using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class HomeMainMenuScript : MonoBehaviour
{
    public SettingsManager setMan;

    public void Start()
    {
        setMan.Load();

        Resolution active = setMan.getRes();

        Screen.SetResolution(active.width, active.height, true);

        QualitySettings.SetQualityLevel(setMan.getQuality());
    }

    public void StartGame()
    {
        // Load the next level (should be Game scene with build index of 1)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        // Quit application on button press
        Application.Quit();
    }
}
