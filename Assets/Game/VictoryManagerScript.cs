using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class VictoryManagerScript : MonoBehaviour
{
    public GameObject VictoryPopup;
    public TMPro.TextMeshProUGUI VictoryPopupTitle;

    public AudioSource winSound;

    public void ShowVictory(int winningPlayer)
    {
        // Set winning player text, load volume for victory sound and play, and show the popup
        float fxVol = IOSettings.LoadSettings().fx;

        VictoryPopupTitle.text = "Player " + winningPlayer + " victory";
        VictoryPopup.SetActive(true);

        winSound.volume = fxVol;
        winSound.Play(0);
    }

    public void ReturnToMenu()
    {
        // Return to the main menu scene (should be 0)
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        // Restart the game through here
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
