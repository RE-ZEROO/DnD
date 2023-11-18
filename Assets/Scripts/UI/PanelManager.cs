using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private bool state;

    void Start()
    {
        state = panel.activeSelf;    
    }

    public void TogglePanel()
    {
        state = !state;
        panel.SetActive(state);
    }

    public void TogglePausePanel()
    {
        state = !state;
        panel.SetActive(state);

        if (state == true)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}
