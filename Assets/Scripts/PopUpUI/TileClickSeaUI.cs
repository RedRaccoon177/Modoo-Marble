using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileClickSea : MonoBehaviour
{
    [Header("°ü±¤Áö ÀÌ¸§")] public TextMeshProUGUI AreaName;

    [Header("°ü±¤Áö °¡°Ý")] public TextMeshProUGUI GroundPrice;

    [Header("¼ÒÀ¯ °¹¼ö ÅëÇà·á")]
    public TextMeshProUGUI Visit1;
    public TextMeshProUGUI Visit2;
    public TextMeshProUGUI Visit3;
    public TextMeshProUGUI Visit4;

    [Header("ÃÑ ÅëÇà·á")]
    public TextMeshProUGUI Visit5;

    public TextMeshProUGUI VisitNumber;
    public TextMeshProUGUI CurrentToll;
    [Header("´Ý±â ¹öÆ°")] public Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(()=>UIManagerP.instance.OffClickUI());
        UIManagerP.instance._clickChangeDataSea += SetTileData;
    }

    public void SetTileData(TileController data)
    {
        AreaName.text = data._tileName;
        GroundPrice.text = data._tileLandPrice.ToString();

        Visit1.text = data._tileLandToll.ToString();
        Visit2.text = data._tilePensionToll.ToString();
        Visit3.text = data._tileCondoToll.ToString();
        Visit4.text = data._tileHotelToll.ToString();

        Visit5.text = (data._tileLandToll + data._tilePensionToll + data._tileCondoToll + data._tileHotelToll).ToString();

        //VisitNumber.text = data.< ¹æ¹®È½¼ö >.ToString();
        //CurrentToll.text = data.< ÅëÇà·á >.ToString();
    }
}
