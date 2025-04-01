using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiceButton : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{
    public Image _gageBackGroud;
    public TurnBasedManager turnBasedManager;
    public Image _gage;
    public Button _diceBtn;
    Coroutine cor_;
    bool _isClicking;
    float _test;
    bool tt;


    private void Start()
    {
        _isClicking = false;
        _diceBtn = GetComponent<Button>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _isClicking = true;
        _gage.fillAmount = 0;
        if (cor_ == null)
        {
            cor_ = StartCoroutine(DiceGageCor());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isClicking = false;
        if (cor_ !=null)
        {
            StopCoroutine(cor_);
            cor_ = null;
            turnBasedManager.Dice();
        }
    }

    // 증가 , 1일 될때까지
    // 감수 ,0이 될때까지

    // 증가중인지, 감소중인지
    // 증가값, 감소 값
    // 클릭 중인지
    // 현재 

    IEnumerator DiceGageCor()
    {
        bool _isIncrease = true;
        float _currentFill = 0;
        float delta = Time.deltaTime;
        while (_isClicking == true)
        {
            if (_isIncrease == true)
            {
                if (_currentFill >= 1)
                {
                    _isIncrease = false;
                }
                _currentFill += delta;
            }
            else
            {
                if (_currentFill <= 0)
                {
                    _isIncrease = true;
                }
                _currentFill -= delta;
            }
            _gage.fillAmount = _currentFill;
            yield return null;
        }
    }

}
