using UnityEngine;
using UnityEngine.UI;

public class ClickUIClose : MonoBehaviour
{
    Button closeBtn;

    private void Awake()
    {
        closeBtn = GetComponent<Button>();
        closeBtn.onClick.AddListener(() => UIManagerP.instance.OffClickUIPanel());
    }
}
