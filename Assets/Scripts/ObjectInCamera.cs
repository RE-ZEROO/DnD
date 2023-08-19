using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInCamera : MonoBehaviour
{
    private Camera mainCam;
    private Plane[] cameraFrustrum;
    private Collider2D coll;
    private RoomInstance roomInstance;

    public static bool enemiesInRoom;

    private void Start()
    {
        mainCam = Camera.main;
        roomInstance = GetComponent<RoomInstance>();
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    {
        var bounds = coll.bounds;
        cameraFrustrum = GeometryUtility.CalculateFrustumPlanes(mainCam);

        if (GeometryUtility.TestPlanesAABB(cameraFrustrum, bounds))
        {
            roomInstance.isCurrentRoom = true;
            //rend.material.color = Color.green;

            if (GetComponentInChildren<EnemyController>() != null)
                enemiesInRoom = true;
            else
                enemiesInRoom = false;

            //foreach (Transform child in transform)
            //    child.gameObject.SetActive(true);
        }
        else if (!GeometryUtility.TestPlanesAABB(cameraFrustrum, bounds))
        {
            roomInstance.isCurrentRoom = false;
            //rend.material.color = Color.red;

            //foreach (Transform child in transform)
            //    child.gameObject.SetActive(false);
        }
    }

    /*public bool ObjectInRoom()
    {
        var bounds = coll.bounds;
        cameraFrustrum = GeometryUtility.CalculateFrustumPlanes(mainCam);

        if (GeometryUtility.TestPlanesAABB(cameraFrustrum, bounds))
        {
            //rend.material.color = Color.green;
            return true;
        }
        else if (!GeometryUtility.TestPlanesAABB(cameraFrustrum, bounds))
        {
            return false;
        }

        return false;
    }*/
}
