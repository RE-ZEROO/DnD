using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneButton : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        SceneController.Instance.LoadScene(sceneName);
    }

    public void ChangeSceneWithoutLoadingScreen(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void DeactivateGameOverCanvas()
    {
        SceneController.Instance.ToggleGameOverCanvas(false);
    }

    public void ExitGame()
    {
        AudioManager.Instance.SaveSettings();
        Application.Quit();
    }
}
