using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameLoadingScript : MonoBehaviour
{
    public GameObject LodingPanel;

    private void Start()
    {
        StartCoroutine(IngameLoading());
    }

    IEnumerator IngameLoading()
    {
        LodingPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        LodingPanel.gameObject.SetActive(false);
    }
}
