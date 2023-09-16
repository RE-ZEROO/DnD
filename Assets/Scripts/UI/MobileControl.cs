using UnityEngine;
using UnityEngine.Device;

public class MobileControl : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(UnityEngine.Device.Application.isMobilePlatform);
    }
}
