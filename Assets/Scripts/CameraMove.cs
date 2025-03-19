using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraMove : MonoBehaviour
{
    Camera mainCam;
    public GameObject target;
    Vector3 mainCamPos;
    float mX;
    float mY;
    private void Start()
    {
        mX = 0;
        mY = 0;
        mainCam = Camera.main;
    }
    private void Update()
    {
        mX = Input.GetAxis("Mouse X");

        if (Input.GetKey(KeyCode.A))
        {
            if (mX>0)
            {
                mainCamPos = new Vector3 (0, 4.7f, mX);
            }
            transform.LookAt(target.transform);
            mainCam.transform.position = mainCamPos;
        }
    }
}
