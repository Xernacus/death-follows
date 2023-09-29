using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    // Update this if starting scene is changed 
    [SerializeField] private string startingScene = "Assets\\Scenes\\Test";

    public void StartGame()
    {
        SceneManager.LoadScene(startingScene);
    }
}
