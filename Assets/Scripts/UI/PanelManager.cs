using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private bool state;

    public void TurnOffNextFrame(GameObject obj) => StartCoroutine(NextFrame(obj));

    private IEnumerator NextFrame(GameObject obj)
    {
        if (!panel.activeSelf) { yield break; }
        
        yield return new WaitForSeconds(.01f);

        state = false;
        panel.SetActive(state);
    }

    public void TogglePanel()
    {
        state = !state;
        panel.SetActive(state);
    }
}
