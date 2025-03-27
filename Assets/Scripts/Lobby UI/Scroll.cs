using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroll<T> : MonoBehaviour
{
    [Header("Componets")]
    [SerializeField] protected ScrollRect scrollRect;
    [SerializeField] protected RectTransform _contentRect;
    [SerializeField] protected RecyclableScrollSlot<T> _slotPrefab;

    [Space]
    [Header("Option")]
    [SerializeField] protected int _bufferCount = 5; // 추가적으로 미리 로드할 슬롯의 개수
    [SerializeField] protected float _spaing; // 각 아이템끼리의 간격

    [Space]
    [Header("VerticalScrollViewOption")]
    [SerializeField] protected int _itemsPerRow = 1;
    [SerializeField] protected float _topOffset;
    [SerializeField] protected float _bottonOffset;
    [SerializeField] protected float _horizontalOffset;

    protected LinkedList<RecyclableScrollSlot<T>> _slotList = new LinkedList<RecyclableScrollSlot<T>>(); // 슬롯 리스트
    protected List<T> _dataList = new List<T>(); // 데이터를 저장하는 리스트
    protected float _itemHeight;  // 슬롯의 높이
    protected float _itemWidth;  // 슬롯의 너비
    protected int _poolSize;
    protected int _tempfirstVisibleIndex;
    protected int _contentVisibleSlotCount;

    public virtual void Init(List<T> dataList)
    {
        _dataList = dataList;

        RectTransform scrollRectTransform = scrollRect.GetComponent<RectTransform>();

        _itemHeight = _slotPrefab.Heigh;
        _itemWidth = _slotPrefab.Width;

        int totalRows = Mathf.CeilToInt((float)_dataList.Count / _itemsPerRow);
        float contentHeight = _itemHeight * totalRows + ((totalRows -1 ) * _spaing) + _topOffset + _bottonOffset;

        _contentRect.anchorMax = new Vector2(1f, 1f);
        _contentRect.anchorMin = new Vector2(0f, 1f);


        _contentVisibleSlotCount = (int)(scrollRectTransform.rect.height / _itemHeight) * _itemsPerRow;
        _contentRect.sizeDelta = new Vector2(_contentRect.sizeDelta.x, contentHeight);


        _poolSize = _contentVisibleSlotCount + (_bufferCount *2 * _itemsPerRow);
        int index = -_bufferCount * _itemsPerRow;
        for(int i =0; i < _poolSize; i++)
        {
            RecyclableScrollSlot<T> item = Instantiate(_slotPrefab, _contentRect);
            _slotList.AddLast(item);
            item.Init();
            UpdataSlot(item, index++);
        }
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    public void UpdataData(List<T> dataList)
    {
        _dataList = dataList;

        int index = _tempfirstVisibleIndex -_bufferCount * _itemsPerRow;
        foreach(RecyclableScrollSlot<T> item in _slotList)
        {
            UpdataSlot(item, index);
            index++;
        }
    }

    protected void OnScroll(Vector2 scrollPosition)
    {
        float contentY = _contentRect.anchoredPosition.y;

        int firstVisibleRowIndex = Mathf.Max(0, Mathf.FloorToInt(contentY / (_itemHeight + _spaing)));
        int firstVisibleIndex = firstVisibleRowIndex * _itemsPerRow;

        if(_tempfirstVisibleIndex != firstVisibleIndex)
        {
            int diffindex = (_tempfirstVisibleIndex - firstVisibleIndex) / _itemsPerRow;

            if (diffindex < 0)
            {
                int lastVistbleIndex = _tempfirstVisibleIndex + _contentVisibleSlotCount;
                for( int i = 0, cnt = Mathf.Abs(diffindex) * _itemsPerRow; i < cnt; i++)
                {
                    RecyclableScrollSlot<T> item = _slotList.First.Value;
                    _slotList.RemoveFirst();
                    _slotList.AddLast(item);

                    int newIndex = lastVistbleIndex + (_bufferCount * _itemsPerRow) + i;
                    UpdataSlot(item, newIndex);
                }
            }

            else if (diffindex > 0 )
            {
                for (int i = 0, cnt = Mathf.Abs(diffindex) * _itemsPerRow; i < cnt; i++)
                {
                    RecyclableScrollSlot<T> item = _slotList.Last.Value;
                    _slotList.RemoveLast();
                    _slotList.AddFirst(item);

                    int newIndex = _tempfirstVisibleIndex - (_bufferCount * _itemsPerRow) - i;
                    UpdataSlot(item, newIndex);
                }
            }

            _tempfirstVisibleIndex = firstVisibleIndex;
        }
    }


    protected void UpdataSlot(RecyclableScrollSlot<T> item, int index)
    {
        int row = 0 <= index ? index / _itemsPerRow : (index -1) / _itemsPerRow;
        int column = Mathf.Abs(index) % _itemsPerRow;

        Vector2 pivot = item.RectTransform.pivot;
        float totalWidth = (_itemsPerRow * (_itemWidth + _spaing)) - _spaing;
        float contentWidth = _contentRect.rect.width;
        float offsetX = (contentWidth - totalWidth) / 2f;
        float adjustedY = -(row * (_itemHeight + _spaing)) - _itemHeight * (1 - pivot.y);
        float adjustedX = column * (_itemWidth + _spaing) + _itemWidth * pivot.x;
        adjustedX += offsetX + _horizontalOffset;
        adjustedY -= _topOffset;
        item.RectTransform.localPosition = new Vector3(adjustedX, adjustedY, 0);

        if(index < 0 || index >= _dataList.Count)
        {
            item.gameObject.SetActive(false);
            return;
        }
        else
        {
            item.UpdateSlot(_dataList[index]);
            item.gameObject.SetActive(true);
        }
    }
}
