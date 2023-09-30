using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    [SerializeField] private string nextScene = "Test";

    public void StartGame()
    {
        SceneManager.LoadScene(nextScene);
    }
}
