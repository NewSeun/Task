using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public class SpriteAtlasSystem : MonoBehaviour
{
    private void OnEnable()
    {
        SpriteAtlasManager.atlasRequested += AtlasRequested;
    }

    private void OnDisable()
    {
        SpriteAtlasManager.atlasRequested -= AtlasRequested;
    }

    private async void AtlasRequested(string tag, System.Action<SpriteAtlas> atlasAction)
    {
        var handle = Addressables.LoadAssetAsync<SpriteAtlas>($"Assets/AddressableData/Atlases/{tag}.spriteatlas");
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            atlasAction.Invoke(handle.Result);
        }
    }
}
