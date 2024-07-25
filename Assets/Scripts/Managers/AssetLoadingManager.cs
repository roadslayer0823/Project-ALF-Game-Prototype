using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AssetLoadingManager : MonoBehaviour
{
    public static IList<T> LoadAsset<T>(string filePath)
    {
        List<string> filePathlist = new(){ filePath };
        return LoadAssets<T>(filePathlist);
    }

    public static IList<T> LoadAssets<T>(List<string> filePathList)
    {
        IList<T> assetList = null;
        Addressables.LoadAssetsAsync<T>(filePathList, (loadedAsset) =>
        {
            Debug.Log("Loading asset: " + loadedAsset);
        }, Addressables.MergeMode.Union,false).Completed +=
        (handle) =>
        {
            if (handle.Result != null)
            {
                assetList = handle.Result;
            }
            else
            {
                Debug.Log("The list is empty");
            }
            Addressables.Release(handle);
        };
        return assetList;
    }
}
