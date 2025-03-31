using UnityEngine;

public class isInputBlockedSc : MonoBehaviour
{
    private bool isInputBlocked = false; // 키 입력을 차단할지 여부

    // Update is called once per frame
    void Update()
    {
        // 해당 오브젝트가 활성화되어 있을 때만 키 입력을 막음
        if (gameObject.activeSelf)
        {
            isInputBlocked = true;  // 키 입력 차단
        }
        else
        {
            isInputBlocked = false; // 키 입력 허용
        }

        if (isInputBlocked)
        {
            // 키 입력이 차단된 상태에서는 아무 코드도 실행하지 않음
            return;
        }

        // 입력이 차단되지 않았을 때 실행되는 코드
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed!");
        }

        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("W key is being held down");
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            Debug.Log("A key released");
        }
    }
}
