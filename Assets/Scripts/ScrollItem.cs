using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class ScrollItem : MonoBehaviour
{
    [SerializeField]
    private Image _itemImage;
    [SerializeField]
    private Text _itemNameText;

    [SerializeField]
    private SpriteAtlas _itemAtlas;

    public void SetItem(int _itemId, string _itemName)  //여기에 파일 네임이랑 아이템 텍스트 정보 넘겨주기
    {
        _itemImage.sprite = AddressableManager.Instance.GetAtlasSprite($"{_itemId}");
        _itemNameText.text = _itemName;
    }
}
