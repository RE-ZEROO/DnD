using UnityEngine;
using UnityEngine.Device;

public class MobileControl : MonoBehaviour
{
    void Start()
    {
        if (UnityEngine.Device.Application.isMobilePlatform)
        {
            foreach(Transform child in transform)
                child.gameObject.SetActive(true);
        }
    }
}
