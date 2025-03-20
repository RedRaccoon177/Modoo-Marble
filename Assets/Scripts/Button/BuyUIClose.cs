using UnityEngine;
using UnityEngine.UI;

public class BuyUIClose : MonoBehaviour
{
    Button closeBtn;

    private void Awake()
    {
        closeBtn = GetComponent<Button>();
        closeBtn.onClick.AddListener(() =>UIManagerP.instance.OffBuyUIPanel());
    }
}
