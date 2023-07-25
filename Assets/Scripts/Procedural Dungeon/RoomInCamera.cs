using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInCamera : MonoBehaviour
{
    private Camera mainCam;
    private SpriteRenderer rend;
    private Plane[] cameraFrustrum;
    private BoxCollider2D coll;
    private RoomInstance roomInstance;
    

    private void Start()
    {
        mainCam = Camera.main;
        roomInstance = GetComponent<RoomInstance>();
        rend = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        var bounds = coll.bounds;
        cameraFrustrum = GeometryUtility.CalculateFrustumPlanes(mainCam);

        if (GeometryUtility.TestPlanesAABB(cameraFrustrum, bounds))
        {
            roomInstance.isCurrentRoom = true;
            //rend.material.color = Color.green;
        }
        else if(!GeometryUtility.TestPlanesAABB(cameraFrustrum, bounds))
        {
            roomInstance.isCurrentRoom = false;
            //rend.material.color = Color.red;
        }
    }
}
