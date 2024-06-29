using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using UnityEngine.UI;

public class AddressableManager : MonoBehaviour
{
    private static AddressableManager s_instance;
    public static AddressableManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                return null;
            }
            return s_instance;
        }
    }

    [SerializeField]
    private AssetReferenceGameObject[] _prefab;
    [SerializeField]
    private AssetReferenceAtlasedSprite _spriteAtlas;
    [SerializeField]
    private AssetReferenceSprite[] _sprite;
    [SerializeField]
    private AssetReference _jsonItemData;

    private List<GameObject> _scrollViewPrefabs = new List<GameObject>();
    private SpriteAtlas _atlas;
    private List<Sprite> _sprites = new List<Sprite>();
    private TextAsset _itemData;

    private List<ItemData> _itemDataList = new List<ItemData>();

    public void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else { Destroy(this.gameObject); }
    }

    void Start()
    {
        InitAddressable().Forget();
    }


    private async UniTask InitAddressable()
    {
        var _init = Addressables.InitializeAsync();
        await _init;

        AddressableLoadState("itemData");
    }

    private void AddressableLoadState(string _label)
    {
        switch (_label)
        {
            case "itemData":
                LoadItemDataAsset().Forget();
                break;
            case "prefab":
                LoadPrefabAsset().Forget();
                break;
            case "sprite":
                LoadSpriteAsset().Forget();
                break;
            case "atlas":
                LoadAtlasAsset().Forget();
                break;
            case "finish":
                CreateUIObject();
                break;
        }
    }
    #region AddressableLoadState
    private async UniTask LoadItemDataAsset()
    {
        Debug.Log($"ItemData load start");
        var _itemDataLoad = Addressables.LoadAssetAsync<TextAsset>(_jsonItemData);
        await _itemDataLoad;
        _itemData = _itemDataLoad.Result;

        SetItemData();

        Debug.Log($"JsonData load end");
        AddressableLoadState("prefab");
    }

    private async UniTask LoadPrefabAsset()
    {
        Debug.Log($"Prefab load start");
        for (int i = 0; i < _prefab.Length; i++)
        {
            var _gameObjectLoad = Addressables.LoadAssetAsync<GameObject>(_prefab[i]);
            await _gameObjectLoad;
            _scrollViewPrefabs.Add(_gameObjectLoad.Result);
        }

        Debug.Log($"Prefab load end");
        AddressableLoadState("sprite");
    }

    private async UniTask LoadSpriteAsset()
    {
        Debug.Log($"Sprite load start");
        for (int i = 0; i < _sprite.Length; i++)
        {
            var _spriteLoad = Addressables.LoadAssetAsync<Sprite>(_sprite[i]);
            await _spriteLoad;
            Debug.Log($"Sprite[{i+1}] load completed");
            _sprites.Add(_spriteLoad.Result);
        }
        Debug.Log($"Sprite load end");
        AddressableLoadState("atlas");
    }

    private async UniTask LoadAtlasAsset()
    {
        Debug.Log($"Atlas load start");
        var _atlasLoad = Addressables.LoadAssetAsync<SpriteAtlas>(_spriteAtlas);
        await _atlasLoad;
        _atlas = _atlasLoad.Result;

        Debug.Log($"Atlas load end");
        AddressableLoadState("finish");
    }

    private void CreateUIObject()
    {
        Transform obj = GameObject.Find("UICanvas").transform;
        Instantiate(_scrollViewPrefabs[0], obj);
    }
    #endregion

    private void SetItemData()
    {
        ItemDatas itemDatas = JsonUtility.FromJson<ItemDatas>(_itemData.ToString());
        foreach (ItemData item in itemDatas.itemData)
        {
            _itemDataList.Add(item);
        }
    }

    public Sprite GetAtlasSprite(string _name)
    {
        if (_atlas.GetSprite($"{_name}") == null)
        {
            Debug.LogError($"AddressableManager.GetAtlas : ({_name}) not found.");
            return null;
        }
        return _atlas.GetSprite($"{_name}");
    }

    public GameObject GetScrollItemPrefab()
    {
        return _scrollViewPrefabs[1];
    }

    public List<ItemData> GetItemDataList()
    {
        return _itemDataList;
    }
}
