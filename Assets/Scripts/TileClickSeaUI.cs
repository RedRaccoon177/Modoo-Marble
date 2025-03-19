using TMPro;
using UnityEngine;

public class TileClickSea : MonoBehaviour
{
    public TextMeshProUGUI AreaName;

    public TextMeshProUGUI GroundPrice;

    public TextMeshProUGUI Visit1;
    public TextMeshProUGUI Visit2;
    public TextMeshProUGUI Visit3;
    public TextMeshProUGUI Visit4;

    public TextMeshProUGUI VisitNumber;
    public TextMeshProUGUI CurrentToll;

    private void Start()
    {
        UIManagerP.instance._clickEvenetGround += SetTileData;
    }

    private void Start()
    {
        UIManagerP.instance._clickEvenetSea += SetTileData;
    }
    public void SetTileData(TileController data)
    {
        AreaName.text = data._tileName;
        GroundPrice.text = data._tileLandPrice.ToString();

        Visit1.text = data._tileLandToll.ToString();
        Visit2.text = data._tilePensionToll.ToString();
        Visit3.text = data._tileCondoToll.ToString();
        Visit4.text = data._tileHotelToll.ToString();

        //VisitNumber.text = data.< 방문횟수 >.ToString();
        //CurrentToll.text = data.< 통행료 >.ToString();
    }
}
