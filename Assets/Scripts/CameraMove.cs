using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMove : MonoBehaviour
{
    public List<GameObject> cameralist;
    Camera mainCam;
    Vector3 mainCamPos;
    float mX;
    float maxMX;
    float minMX;

    int cameraNum = 0;

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

        if( Input.GetKeyDown(KeyCode.Q))
        {
            QCameraInput();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            //ECameraInput();
        }
    }

    void QCameraInput()
    {

        if( cameralist.Count-1 <= cameraNum)
        {
            cameralist[cameraNum].gameObject.SetActive(false);
            cameraNum = 0;
            cameralist[cameraNum].gameObject.SetActive(true);
            Debug.Log(cameraNum);
        }
        else
        {
            cameralist[cameraNum].gameObject.SetActive(false);
            cameraNum++;
            cameralist[cameraNum].gameObject.SetActive(true);
            Debug.Log(cameraNum);
        }
    }
    void ECameraInput()
    {

        if (cameralist.Count - 1 > 0)
        {
            cameralist[cameraNum].gameObject.SetActive(false);
            cameraNum = cameralist.Count - 1;
            cameralist[cameraNum].gameObject.SetActive(true);
            Debug.Log(cameraNum);
        }
        else if(cameralist.Count - 1 <= cameraNum)
        {
            cameralist[cameraNum].gameObject.SetActive(false);
            cameraNum--;
            cameralist[cameraNum].gameObject.SetActive(true);
            Debug.Log(cameraNum);
        }
    }
}
