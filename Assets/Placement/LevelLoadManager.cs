using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoadManager : MonoBehaviour
{
    public string sceneName;
    private Vector3 position;
    private Quaternion rotation;
    private Vector3 scale;

    public void transitionToNextLevel(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
        Object.DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(sceneName);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == sceneName)
        {
            GameObject.FindGameObjectWithTag("OldCamera").SetActive(false);
            Transform newAnchor = GameObject.FindGameObjectWithTag("Anchor").transform;
            newAnchor.position = position;
            newAnchor.rotation = rotation;
            newAnchor.localScale = scale;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
