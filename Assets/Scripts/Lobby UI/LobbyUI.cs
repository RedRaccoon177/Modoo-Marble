using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class LobbyUI : MonoBehaviour
{
    GameSound gameSound;
    public GameObject optionUi;

    private void Start()
    {
        gameSound = GameObject.Find("Click").GetComponent<GameSound>();
    }

    public void onClickBtnSoundPlay()
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
