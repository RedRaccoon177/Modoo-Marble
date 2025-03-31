using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    public GameObject optionUi;

    public void onSoundOptionUI()
    {
        optionUi.SetActive(true);
    }

    public void offSoundOptionUI()
    {
        optionUi.SetActive(false);
    }
}
