using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源加载管理器。基于Resources类的加载。
/// 这个脚本依赖Mono管理器。
/// 要加载的资源必须放到项目中名叫Resources的文件夹中。项目中可以有多个名叫Resources的文件夹，但如此一来，必须避免资源的路径相同。
/// </summary>
public class ResourcesManager : SingletonPatternBase<ResourcesManager>
{
    /// <summary>
    /// <para>同步加载Resources文件夹中的资源。</para>
    /// <para>如果有多个相同路径的资源，则只会返回找到的第一个资源。</para>
    /// </summary>
    /// <param name="path">要加载的资源的路径。例如"Prefabs/Cube"表示Resources文件夹中的Prefabs文件夹中的名叫Cube的资源。</param>
    public Object Load(string path)
    {
        return Resources.Load(path);
    }

    /// <summary>
    /// <para>同步加载Resources文件夹中指定类型的资源。</para>
    /// <para>如果有多个相同类型，且相同路径的资源，则只会返回找到的第一个资源。</para>
    /// </summary>
    /// <param name="path">要加载的资源的路径。例如"Prefabs/Cube"表示Resources文件夹中的Prefabs文件夹中的名叫Cube的资源。</param>
    /// <param name="systemTypeInstance">要加载的资源的类型的Type对象。例如typeof(GameObject)表示要加载的资源的类型是GameObject型。</param>
    /// <returns></returns>
    public Object Load(string path, System.Type systemTypeInstance)
    {
        return Resources.Load(path, systemTypeInstance);
    }

    /// <summary>
    /// <para>同步加载Resources文件夹中指定类型的资源。</para>
    /// <para>如果有多个相同类型，且相同路径的资源，则只会返回找到的第一个资源。</para>
    /// </summary>
    /// <typeparam name="T">要加载的资源的类型</typeparam>
    /// <param name="path">要加载的资源的路径。例如"Prefabs/Cube"表示Resources文件夹中的Prefabs文件夹中的名叫Cube的资源。</param>
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    /// <summary>
    /// <para>同步加载Resources文件夹中指定路径的文件夹中的所有资源，包括其中子孙文件夹中的所有资源，然后返回到一个Object[]型数组。</para>
    /// <para>如果该路径是一个文件，则只会加载该文件，并返回到一个Object[]型数组。</para>
    /// <para>如果没有加载到任何资源，则返回一个没有任何元素的Object[]型数组。</para>
    /// </summary>
    /// <param name="path">要加载的文件夹或文件的路径。例如"Prefabs"表示Resources文件夹中的Prefabs文件夹。例如"Prefabs/Cube"表示Resources文件夹中的Prefabs文件夹中的名叫Cube的资源。</param>
    public Object[] LoadAll(string path)
    {
        return Resources.LoadAll(path);
    }

    /// <summary>
    /// <para>同步加载Resources文件夹中指定路径的文件夹中指定类型的所有资源，包括其中子孙文件夹中的该类型的所有资源，然后返回到一个Object[]型数组。</para>
    /// <para>如果该路径是一个该指定类型的文件，则只会加载该文件，并返回到一个Object[]型数组。</para>
    /// <para>如果没有加载到任何资源，则返回一个没有任何元素的Object[]型数组。</para>
    /// </summary>
    /// <param name="path">要加载的文件夹或文件的路径。例如"Prefabs"表示Resources文件夹中的Prefabs文件夹。例如"Prefabs/Cube"表示Resources文件夹中的Prefabs文件夹中的名叫Cube的资源。</param>
    /// <param name="systemTypeInstance">要加载的资源的类型的Type对象。例如typeof(GameObject)表示要加载的资源的类型是GameObject型。</param>
    public Object[] LoadAll(string path, System.Type systemTypeInstance)
    {
        return Resources.LoadAll(path, systemTypeInstance);
    }

    /// <summary>
    /// <para>同步加载Resources文件夹中指定路径的文件夹中指定类型的所有资源，包括其中子孙文件夹中的该类型的所有资源，然后返回到一个Object[]型数组。</para>
    /// <para>如果该路径是一个该指定类型的文件，则只会加载该文件，并返回到一个Object[]型数组。</para>
    /// <para>如果没有加载到任何资源，则返回一个没有任何元素的Object[]型数组。</para>
    /// </summary>
    /// <typeparam name="T">要加载的资源的类型</typeparam>
    /// <param name="path">要加载的文件夹或文件的路径。例如"Prefabs"表示Resources文件夹中的Prefabs文件夹。例如"Prefabs/Cube"表示Resources文件夹中的Prefabs文件夹中的名叫Cube的资源。</param>
    public T[] LoadAll<T>(string path) where T : Object
    {
        return Resources.LoadAll<T>(path);
    }

    /// <summary>
    /// <para>异步加载Resources文件夹中的资源。</para>
    /// <para>如果有多个相同路径的资源，则只会加载找到的第一个资源。</para>
    /// </summary>
    /// <param name="path">要加载的资源的路径。例如"Prefabs/Cube"表示Resources文件夹中的Prefabs文件夹中的名叫Cube的资源。</param>
    /// <param name="callback">资源加载完毕后要执行的逻辑。参数表示加载的资源。</param>
    public void LoadAsync(string path, UnityAction<Object> callback = null)
    {
        MonoManager.Instance.StartCoroutine(LoadAsyncCoroutine(path, callback));
    }

    IEnumerator LoadAsyncCoroutine(string path, UnityAction<Object> callback = null)
    {
        ResourceRequest resourceRequest = Resources.LoadAsync(path);
        yield return resourceRequest;
        callback?.Invoke(resourceRequest.asset);
    }

    /// <summary>
    /// <para>异步加载Resources文件夹中指定类型的资源。</para>
    /// <para>如果有多个相同类型，且相同路径的资源，则只会加载找到的第一个资源。</para>
    /// </summary>
    /// <param name="path"></param>
    /// <param name="type">要加载的资源的类型的Type对象。例如typeof(GameObject)表示要加载的资源的类型是GameObject型。</param>
    /// <param name="callback">资源加载完毕后要执行的逻辑。参数表示加载的资源。</param>
    public void LoadAsync(string path, System.Type type, UnityAction<Object> callback = null)
    {
        MonoManager.Instance.StartCoroutine(LoadAsyncCoroutine(path, type, callback));
    }

    IEnumerator LoadAsyncCoroutine(string path, System.Type type, UnityAction<Object> callback = null)
    {
        ResourceRequest resourceRequest = Resources.LoadAsync(path, type);
        yield return resourceRequest;
        callback?.Invoke(resourceRequest.asset);
    }

    /// <summary>
    /// <para>异步加载Resources文件夹中指定类型的资源。</para>
    /// <para>如果有多个相同类型，且相同路径的资源，则只会加载找到的第一个资源。</para>
    /// </summary>
    /// <typeparam name="T">加载的资源的类型</typeparam>
    /// <param name="path">要加载的资源的路径。例如"Prefabs/Cube"表示Resources文件夹中的Prefabs文件夹中的名叫Cube的资源。</param>
    /// <param name="callback">资源加载完毕后要执行的逻辑</param>
    public void LoadAsync<T>(string path, UnityAction<T> callback = null) where T : Object
    {
        MonoManager.Instance.StartCoroutine(LoadAsyncCoroutine(path, callback));
    }

    IEnumerator LoadAsyncCoroutine<T>(string path, UnityAction<T> callback = null) where T : Object
    {
        ResourceRequest resourceRequest = Resources.LoadAsync<T>(path);
        yield return resourceRequest;
        callback?.Invoke(resourceRequest.asset as T);
    }

    /// <summary>
    /// <para>异步卸载所有用Resources方式加载到内存中且当前没有被任何地方使用的资源。</para>
    /// <para>例如要卸载某一个用Resources方式加载的预制体，则必须确保场景中所有这个预制体创建的物体都被销毁了，且这个预制体资源没有赋值给任何脚本中的任何变量。</para>
    /// <para>如果有，可以把该变量也赋值为null，这样本方法才能成功释放它。</para>
    /// </summary>
    /// <param name="callback">资源卸载完毕后执行的逻辑</param>
    public void UnloadUnusedAssets(UnityAction callback = null)
    {
        MonoManager.Instance.StartCoroutine(UnLoadUnusedAssetsCoroutine(callback));
    }

    IEnumerator UnLoadUnusedAssetsCoroutine(UnityAction callback = null)
    {
        //异步操作对象，记录了异步操作的数据。
        AsyncOperation asyncOperation = Resources.UnloadUnusedAssets();

        //等待资源卸载
        while (asyncOperation.progress < 1)
            yield return null;

        //资源卸载完毕后执行的逻辑
        callback?.Invoke();
    }

    /// <summary>
    /// <para>同步卸载指定的资源。</para>
    /// <para>只能卸载非GameObject类型和Component类型，例如Mesh、Texture、Material、Shader。如果卸载了不让卸载的资源，则会报错。</para>
    /// <para>如果随后加载的任何场景或资源引用了该资源，将导致从磁盘中加载该资源的新实例。此新实例将与先前卸载的对象相互独立。</para>
    /// </summary>
    /// <param name="assetToUnload">要卸载的资源</param>
    public void UnloadAsset(Object assetToUnload)
    {
        Resources.UnloadAsset(assetToUnload);
    }

    /// <summary>
    /// <para>同步把Resources文件夹中指定路径的文件夹及其所有子孙文件夹中所有指定类型的资源添加到一个新建的字典中，并返回该字典。</para>
    /// <para>应保证Prefabs文件夹中以及它的子孙文件夹中没有重名的资源，如果有重名的，则只会添加找到的第一个资源进字典，其它重名的资源不会进到字典中。</para>
    /// </summary>
    /// <typeparam name="T">要加载的资源类型</typeparam>
    /// <param name="path">资源的路径。例如"Folder/Res"表示Resources文件夹中的Folder文件夹中的Res文件夹</param>
    public Dictionary<string, T> LoadAllIntoDictionary<T>(string path) where T : Object
    {
        Dictionary<string, T> dic = new Dictionary<string, T>();
        T[] temp = Resources.LoadAll<T>(path);
        for (int i = 0; i < temp.Length; i++)
        {
            if (!dic.ContainsKey(temp[i].name)) //字典不存在该键，才添加进去。这样可以防止字典的键名重复而报错。
            {
                dic.Add(temp[i].name, temp[i]);
            }
            else //如果字典已经存在该键，则跳过这个资源，并输出警告，不将它加入到字典中
            {
                Debug.LogWarning(string.Format("Resources/{0}的子孙文件夹的资源{1}与已经添加到字典中的资源重名，因此无法将它添加到字典中，请确保加载的资源的名字是唯一的。",
                    path, temp[i].name));
            }
        }

        return dic;
    }

    /// <summary>
    /// <para>同步把Resources文件夹中指定路径及其所有子孙文件夹中所有指定类型的资源添加到指定的字典中</para>
    /// </summary>
    /// <typeparam name="T">要加载的资源类型</typeparam>
    /// <param name="path">资源的路径。例如"Folder/Res"表示Resources文件夹中的Folder文件夹中的Res文件夹</param>
    /// <param name="dictionary">指定的字典</param>
    public void LoadAllIntoDictionary<T>(string path, Dictionary<string, T> dictionary) where T : Object
    {
        T[] temp = Resources.LoadAll<T>(path);
        for (int i = 0; i < temp.Length; i++)
        {
            if (!dictionary.ContainsKey(temp[i].name)) //字典不存在该键，才添加进去。这样可以防止字典的键名重复而报错。
            {
                dictionary.Add(temp[i].name, temp[i]);
            }
            else //如果字典已经存在该键，则跳过这个资源，并输出警告，不将它加入到字典中
            {
                Debug.LogWarning(string.Format(
                    "Resources/{0}的子孙文件夹的资源{1}与已经添加到字典中的资源重名，因此无法将它添加到字典中，请确保加载的资源的名字是唯一的，并且传入参数的字典中不包含该名字的资源。", path,
                    temp[i].name));
            }
        }
    }
}