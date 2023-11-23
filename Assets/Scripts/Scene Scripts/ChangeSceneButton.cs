using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneButton : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        AudioManager.Instance.SaveSettings();
        PlayerPrefs.Save();
        SceneController.Instance.LoadScene(sceneName);
    }

    public void ChangeSceneWithoutLoadingScreen(string sceneName)
    {
        AudioManager.Instance.SaveSettings();
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneName);
    }

    public void DeactivateGameOverCanvas()
    {
        SceneController.Instance.ToggleGameOverCanvas(false);
    }

    public void ExitGame()
    {
        Debug.Log("Closed Application!");
        AudioManager.Instance.SaveSettings();
        PlayerPrefs.Save();
        Application.Quit();
    }
}
