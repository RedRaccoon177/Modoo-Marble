using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Camera mainCam;
    Vector3 mainCamPos;
    float mX;
    float maxMX;
    float minMX;
    private void Start()
    {
        mX = 0;
        maxMX = 2;
        minMX = -2;
        mainCam = Camera.main;
        //Cursor.lockState = CursorLockMode.Confined; // 마우스 화면 밖으로 못나가게
    }
    private void Update()
    {

        if (Input.GetKey(KeyCode.A))
        {
            mX += Input.GetAxis("Mouse X");
            if (minMX > mX)
            {
                mX = minMX;
            }
            else if (maxMX < mX)
            {
                mX = maxMX;
            }
            mainCamPos = new Vector3 (-mX, 4.2f, -mX);
            mainCam.transform.position = mainCamPos;
        }
    }
}
