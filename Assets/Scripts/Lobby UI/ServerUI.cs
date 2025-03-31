using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerUI : MonoBehaviour
{
    GameSound gameSound;
    public GameObject optionUi;

    private void Start()
    {
        gameSound = GameObject.Find("Click").GetComponent<GameSound>();
    }

    public void onClickServerBtnSoundPlay()
    {
        gameSound.ClickSoundPlay();
    }

    public void onSoundOptionUI()
    {
        optionUi.SetActive(true);
    }

    public void offSoundOptionUI()
    {
        optionUi.SetActive(false);
    }
}
