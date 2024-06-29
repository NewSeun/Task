using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecycleScrollView : MonoBehaviour
{
    public Vector2 itemCellSize;
    public List<ItemData> itemDataList = new List<ItemData>();    //나중에 제이슨 데이터로 불러오기 //

    [Header("스크롤 아이템 가로 개수(최소 1개)")]
    public int scrollColumeCount = 1;
    private int _curScrollColumeCount;

    [Header("데이터 개수(최소 1개, 최대 50개)")]
    public int itemDataCount = 50;
    private int _curItemDataCount;

    private ScrollItem listItemPrefab;
    private ScrollRect _scroll;
    private List<ScrollItem> _itemList = new List<ScrollItem>();
    private RectTransform _scrollRect;
    private float _offset;


    private void Awake()
    {
        _curScrollColumeCount = scrollColumeCount;
        _scroll = GetComponent<ScrollRect>();
        _scrollRect = _scroll.GetComponent<RectTransform>();
    }

    private void Start()
    {
        listItemPrefab = AddressableManager.Instance.GetScrollItemPrefab().GetComponent<ScrollItem>();
        ResetScroll();
    }

    private void ResetScroll()
    {
        if (_scroll.content.childCount > 0)
        {
            for (int i = _scroll.content.childCount - 1; i >= 0; i--)
            {
                Destroy(_scroll.content.GetChild(i).gameObject);
            }
        }
        _itemList.Clear();
        itemDataList = AddressableManager.Instance.GetItemDataList();
        CreateItem();
        SetContentHeight();
    }

    private void CreateItem()
    {
        int _columeCount = 0;
        int _itemObjectCount = (int)(_scrollRect.rect.height / itemCellSize.y) + (int)Mathf.Pow(_curScrollColumeCount, 2) + _curScrollColumeCount;

        float _itemPosY = 0;
        int _rowCnt = 0;
        for (int i = 0; i < _itemObjectCount; i++)
        {
            ScrollItem _item = Instantiate<ScrollItem>(listItemPrefab, _scroll.content);
            _itemList.Add(_item);

            if (_columeCount == _curScrollColumeCount)
            {
                _columeCount = 0;
                _rowCnt++;

                _itemPosY -= itemCellSize.y;
            }
            _item.transform.localPosition = new Vector3(_columeCount * itemCellSize.x, _itemPosY);
            _columeCount++;

            SetData(_item, i);
        }
        _offset = _itemPosY * -1;
    }

    private void SetContentHeight()
    {
        float _contentSizeY = 0;

        _contentSizeY = _curItemDataCount % _curScrollColumeCount == 0 ? _curItemDataCount / _curScrollColumeCount :
            _curItemDataCount / _curScrollColumeCount + 1;

        _scroll.content.sizeDelta = new Vector2(0, _contentSizeY * itemCellSize.y);
    }

    private bool RelocationItem(ScrollItem _item, float _contentY, float _scrollHeight)
    {
        if (_item.transform.localPosition.y + _contentY > itemCellSize.y/* * 2f*/)
        {
            _item.transform.localPosition -= new Vector3(0, _offset);
            return true;
        }
        else if (_item.transform.localPosition.y + _contentY < -_scrollHeight /*- itemCellSize.y*/)
        {
            _item.transform.localPosition += new Vector3(0, _offset);
            return true;
        }
        return false;
    }

    private void SetData(ScrollItem _item, int _index)
    {
        if (_index < 0 || _index >= _curItemDataCount)
        {
            _item.gameObject.SetActive(false);
            return;
        }
        _item.SetItem(itemDataList[_index].id, itemDataList[_index].name);
        _item.gameObject.SetActive(true);
    }

    private void Update()
    {
        CheckScrollColumeCount();
        CheckDataCount();

        CheckScrollItemPosY();
    }

    private void CheckScrollColumeCount()
    {

        if (scrollColumeCount != _curScrollColumeCount)
        {
            scrollColumeCount = scrollColumeCount < 1 ? 1 : scrollColumeCount;

            _curScrollColumeCount = scrollColumeCount;
            ResetScroll();
        }
    }

    private void CheckDataCount()
    {
        if (itemDataCount != _curItemDataCount)
        {
            if (itemDataCount < 1)
            {
                itemDataCount = 1;
            }
            else if (itemDataCount > 50)
            {
                itemDataCount = 50;
            }
            _curItemDataCount = itemDataCount;
            ResetScroll();
        }
    }

    private void CheckScrollItemPosY()
    {
        float _scrollHeight = _scrollRect.rect.height;
        float _contentY = _scroll.content.anchoredPosition.y;

        foreach (ScrollItem _item in _itemList)
        {
            if (RelocationItem(_item, _contentY, _scrollHeight))
            {
                int _index = 0;
                if (-_item.transform.localPosition.y < 0)
                {
                    SetData(_item, -1);
                    return;
                }
                if ((int)(-_item.transform.localPosition.y / itemCellSize.y) > 0)
                {
                    _index += (int)(_item.transform.localPosition.x / itemCellSize.x) + (int)(-_item.transform.localPosition.y / itemCellSize.y) * _curScrollColumeCount;
                }
                else
                {
                    _index = (int)(_item.transform.localPosition.x / itemCellSize.x);
                }
                SetData(_item, _index);
            }
        }
    }
}
