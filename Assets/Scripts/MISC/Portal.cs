using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) { return; }
        //    collision.GetComponent<PlayerController>().currentState = PlayerState.TELEPORTING_OUT;
        
        GameController.NextLevel();
    }
}
