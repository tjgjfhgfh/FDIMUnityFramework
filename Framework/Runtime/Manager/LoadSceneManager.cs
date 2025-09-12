using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace FDIM.Framework
{
    /// <summary>
    /// 切换场景的管理器。用于加载场景和切换场景。
    /// </summary>
    public class LoadSceneManager : SingletonPatternBase<LoadSceneManager>
    {
        /// <summary>
        /// <para>同步加载指定索引的场景，加载成功后会把该场景设置为活动场景。</para>
        /// <para>场景要预先放到Build Settings窗口中，场景的索引也可以在Build Settings窗口中查看。</para>
        /// </summary>
        /// <param name="sceneBuildIndex">场景的索引</param>
        public void LoadScene(int sceneBuildIndex)
        {
            SceneManager.LoadScene(sceneBuildIndex);
        }
    
        /// <summary>
        /// <para>同步加载指定名字的场景，加载成功后会把该场景设置为活动场景。</para>
        /// <para>场景要预先放到Build Settings窗口中。</para>
        /// <para>如果使用AssetBundle加载，则只要成功加载了该场景，即使它没有放到Build Settings窗口中，也可以使用这个方法来加载这个场景，加载成功后会把该场景设置为活动场景。</para>
        /// </summary>
        /// <param name="sceneName">场景的名字</param>
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    
        /// <summary>
        /// <para>同步加载指定索引的场景，加载成功后会把该场景设置为活动场景。</para>
        /// <para>场景要预先放到Build Settings窗口中，场景的索引也可以在Build Settings窗口中查看。</para>
        /// <para>第二个参数如果使用LoadSceneMode.Single，则加载完毕后，会自动切换到该场景。原来场景会被卸载。默认就是使用这个。</para>
        /// <para>第二个参数如果使用LoadSceneMode.Additive，则加载完毕后，该场景会叠加到原来的场景中。原来的场景不会被卸载，且活动场景依然是原来的场景。</para>
        /// </summary>
        /// <param name="sceneBuildIndex">场景的索引</param>
        /// <param name="mode">加载场景的模式。</param>
        public void LoadScene(int sceneBuildIndex, LoadSceneMode mode)
        {
            SceneManager.LoadScene(sceneBuildIndex, mode);
        }
    
        /// <summary>
        /// <para>同步加载指定名字的场景，加载成功后会把该场景设置为活动场景。</para>
        /// <para>场景要预先放到Build Settings窗口中。</para>
        /// <para>如果使用AssetBundle加载，则只要成功加载了该场景，即使它没有放到Build Settings窗口中，也可以使用这个方法来加载这个场景，加载成功后会把该场景设置为活动场景。</para>
        /// <para>第二个参数如果使用LoadSceneMode.Single，则加载完毕后，会自动切换到该场景。原来场景会被卸载。默认就是使用这个。</para>
        /// <para>第二个参数如果使用LoadSceneMode.Additive，则加载完毕后，该场景会叠加到原来的场景中。原来的场景不会被卸载，且活动场景依然是原来的场景。</para>
        /// </summary>
        /// <param name="sceneName">场景的名字</param>
        /// <param name="mode">加载场景的模式。</param>
        public void LoadScene(string sceneName, LoadSceneMode mode)
        {
            SceneManager.LoadScene(sceneName, mode);
        }
    
        /// <summary>
        /// <para>同步加载指定索引的场景，加载成功后会把该场景设置为活动场景。</para>
        /// <para>场景要预先放到Build Settings窗口中，场景的索引也可以在Build Settings窗口中查看。</para>
        /// </summary>
        /// <param name="sceneBuildIndex">场景的索引</param>
        public void LoadScene(int sceneBuildIndex, LoadSceneParameters parameters)
        {
            SceneManager.LoadScene(sceneBuildIndex, parameters);
        }
    
        /// <summary>
        /// <para>同步加载指定名字的场景，加载成功后会把该场景设置为活动场景。</para>
        /// <para>场景要预先放到Build Settings窗口中。</para>
        /// <para>如果使用AssetBundle加载，则只要成功加载了该场景，即使它没有放到Build Settings窗口中，也可以使用这个方法来加载这个场景，加载成功后会把该场景设置为活动场景。</para>
        /// </summary>
        /// <param name="sceneName">场景的名字</param>
        public void LoadScene(string sceneName, LoadSceneParameters parameters)
        {
            SceneManager.LoadScene(sceneName, parameters);
        }
    
        /// <summary>
        /// <para>同步加载当前的活动场景，加载完毕后会重新切换到当前活动场景。</para>
        /// <para>即使当前活动场景没有加入到Build Settings窗口，本方法依然有效。</para>
        /// <para>如果当前只有一个场景，则本方法实际上就是重新载入当前场景。</para>
        /// </summary>
        public void LoadActiveScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    
        /// <summary>
        /// <para>同步加载Build Settings窗口中当前活动场景的下一个场景，加载完毕后会切换到该场景。</para>
        /// <para>加载场景成功返回true，加载场景失败返回false</para>
        /// </summary>
        /// <param name="isCyclical">是否循环加载场景。当要加载的场景的索引超过了Build Settings窗口中的最大索引，如果这个参数为true，则会加载索引为0的场景，然后返回true，如果这个参数为false，则视为加载失败，返回false，且不会加载该场景。</param>
        public void LoadNextScene(bool isCyclical = false)
        {
            //下一个场景的索引。
            int buildIndex = SceneManager.GetActiveScene().buildIndex + 1;
    
            //当要加载的场景的索引超过了Build Settings窗口中的最大索引，则根据参数来判断是否循环加载场景。
            if (buildIndex > SceneManager.sceneCountInBuildSettings - 1)
            {
                if (isCyclical)
                {
                    buildIndex = 0;
                    //同步加载该场景，加载完毕后会切换到该场景。
                    SceneManager.LoadScene(buildIndex);
                }
                else
                {
                    Debug.LogWarning($"加载场景失败！要加载的场景的索引是{buildIndex},超过了当前Build Settings窗口中的最大索引。");
                }
            }
        }
    
        /// <summary>
        /// <para>同步加载Build Settings窗口中当前活动场景的上一个场景，加载完毕后会切换到该场景。</para>
        /// <para>加载场景成功返回true，加载场景失败返回false</para>
        /// </summary>
        /// <param name="isCyclical">是否循环加载场景。当要加载的场景的索引超过了Build Settings窗口中的最大索引，如果这个参数为true，则会加载索引为0的场景，然后返回true，如果这个参数为false，则视为加载失败，返回false，且不会加载该场景。</param>
        public void LoadPreviousScene(bool isCyclical = false)
        {
            //上一个场景的索引。
            int buildIndex = SceneManager.GetActiveScene().buildIndex - 1;
    
            //当要加载的场景的索引为负数，则根据参数来判断是否循环加载场景。
            if (buildIndex < 0)
            {
                if (isCyclical)
                {
                    buildIndex = SceneManager.sceneCountInBuildSettings - 1;
                    //同步加载该场景，加载完毕后会切换到该场景。
                    SceneManager.LoadScene(buildIndex);
                }
                else
                {
                    Debug.LogWarning($"加载场景失败！要加载的场景的索引是{buildIndex},没有索引为负数的场景。");
                }
            }
        }
    
        /// <summary>
        /// <para>异步加载场景。默认情况下，成功加载场景后，会切换到该场景。</para>
        /// <para>可以控制外部进度条的显示。</para>
        /// </summary>
        /// <param name="sceneBuildIndex">要加载的场景在Build Settings窗口中的索引。</param>
        /// <param name="loading">加载中的回调。只要在加载中，就会不断地执行这个回调。一般用于进度条的显示。</param>
        /// <param name="completed">加载完毕后的回调。</param>
        /// <param name="setActiveAfterCompleted">加载场景完毕后，是否切换到该场景。</param>
        /// <param name="mode">加载场景的模式。默认是LoadSceneMode.Single，表示会卸载原来的场景，再切换到新场景。LoadSceneMode.Additive表示会将新场景叠加在原来的场景中。</param>
        public void LoadSceneAsync(int sceneBuildIndex, UnityAction<float> loading = null,
            UnityAction<AsyncOperation> completed = null, bool setActiveAfterCompleted = true,
            LoadSceneMode mode = LoadSceneMode.Single)
        {
            //如果要加载的场景的索引不合法，则返回。
            if (!IsSceneBuildIndexValid(sceneBuildIndex))
                return;
    
            //开启协程进行异步加载。
            MonoManager.Instance.StartCoroutine(LoadSceneAsyncCoroutine(sceneBuildIndex, loading, completed,
                setActiveAfterCompleted, mode));
        }
    
        IEnumerator LoadSceneAsyncCoroutine(int sceneBuildIndex, UnityAction<float> loading = null,
            UnityAction<AsyncOperation> completed = null, bool setActiveAfterCompleted = true,
            LoadSceneMode mode = LoadSceneMode.Single)
        {
            //异步操作对象，记录了异步操作的数据。
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
    
            //设置为false，则即使场景加载完毕，也不会切换过去。当场景加载完毕，再次把这个值复制为true，才会切换过去。
            asyncOperation.allowSceneActivation = false;
    
            //加载场景的过程中，提供给外部执行的回调。一般用于进度条的显示。
            while (asyncOperation.progress < 0.9f)
            {
                loading?.Invoke(asyncOperation.progress);
                yield return null;
            }
    
            //当asyncOperation.allowSceneActivation为false，则asyncOperation.progress最多只能到达0.9，我们人为让它凑成整数1，方便外部进度条的显示。
            loading?.Invoke(1f);
    
            //加载场景完毕之后，如果把这个变量设置为true，则会切换到该场景。如果为false，则不会切换到该场景。
            asyncOperation.allowSceneActivation = setActiveAfterCompleted;
            //确保场景激活
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
    
            //加载场景完毕之后执行的回调。
            completed?.Invoke(asyncOperation);
        }
    
        /// <summary>
        /// <para>异步加载场景。默认情况下，成功加载场景后，会切换到该场景。</para>
        /// <para>可以控制外部进度条的显示。</para>
        /// </summary>
        /// <param name="sceneBuildIndex">要加载的场景在Build Settings窗口中的索引。</param>
        /// <param name="loading">加载中的回调。只要在加载中，就会不断地执行这个回调。一般用于进度条的显示。</param>
        /// <param name="completed">加载完毕后的回调。</param>
        /// <param name="setActiveAfterCompleted">加载场景完毕后，是否切换到该场景。</param>
        public void LoadSceneAsync(int sceneBuildIndex, LoadSceneParameters parameters, UnityAction<float> loading = null,
            UnityAction<AsyncOperation> completed = null, bool setActiveAfterCompleted = true)
        {
            //如果要加载的场景的索引不合法，则返回。
            if (!IsSceneBuildIndexValid(sceneBuildIndex))
                return;
    
            //开启协程进行异步加载。
            MonoManager.Instance.StartCoroutine(LoadSceneAsyncCoroutine(sceneBuildIndex, parameters, loading, completed,
                setActiveAfterCompleted));
        }
    
        IEnumerator LoadSceneAsyncCoroutine(int sceneBuildIndex, LoadSceneParameters parameters,
            UnityAction<float> loading = null, UnityAction<AsyncOperation> completed = null,
            bool setActiveAfterCompleted = true)
        {
            //异步操作对象，记录了异步操作的数据。
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, parameters);
    
            //设置为false，则即使场景加载完毕，也不会切换过去。当场景加载完毕，再次把这个值复制为true，才会切换过去。
            asyncOperation.allowSceneActivation = false;
    
            //加载场景的过程中，提供给外部执行的回调。一般用于进度条的显示。
            while (asyncOperation.progress < 0.9f)
            {
                loading?.Invoke(asyncOperation.progress);
                yield return null;
            }
    
            //当asyncOperation.allowSceneActivation为false，则asyncOperation.progress最多只能到达0.9，我们人为让它凑成整数1，方便外部进度条的显示。
            loading?.Invoke(1f);
    
            //加载场景完毕之后，如果把这个变量设置为true，则会切换到该场景。如果为false，则不会切换到该场景。
            asyncOperation.allowSceneActivation = setActiveAfterCompleted;
            //确保场景激活
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
    
            //加载场景完毕之后执行的回调。
            completed?.Invoke(asyncOperation);
        }
    
        /// <summary>
        /// <para>异步加载场景。默认情况下，成功加载场景后，会切换到该场景。</para>
        /// <para>可以控制外部进度条的显示。</para>
        /// </summary>
        /// <param name="sceneName">要加载的场景名</param>
        /// <param name="loading">加载中的回调。只要在加载中，就会不断地执行这个回调。一般用于进度条的显示。</param>
        /// <param name="completed">加载完毕后的回调。</param>
        /// <param name="setActiveAfterCompleted">加载场景完毕后，是否切换到该场景。</param>
        /// <param name="mode">加载场景的模式。默认是LoadSceneMode.Single，表示会卸载原来的场景，再切换到新场景。LoadSceneMode.Additive表示会将新场景叠加在原来的场景中。</param>
        public void LoadSceneAsync(string sceneName, UnityAction<float> loading = null,
            UnityAction<AsyncOperation> completed = null, bool setActiveAfterCompleted = true,
            LoadSceneMode mode = LoadSceneMode.Single)
        {
            //开启协程进行异步加载。
            MonoManager.Instance.StartCoroutine(LoadSceneAsyncCoroutine(sceneName, loading, completed,
                setActiveAfterCompleted, mode));
        }
    
        IEnumerator LoadSceneAsyncCoroutine(string sceneName, UnityAction<float> loading = null,
            UnityAction<AsyncOperation> completed = null, bool setActiveAfterCompleted = true,
            LoadSceneMode mode = LoadSceneMode.Single)
        {
            //异步操作对象，记录了异步操作的数据。
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
    
            //设置为false，则即使场景加载完毕，也不会切换过去。当场景加载完毕，再次把这个值复制为true，才会切换过去。
            asyncOperation.allowSceneActivation = false;
    
            //加载场景的过程中，提供给外部执行的回调。一般用于进度条的显示。
            while (asyncOperation.progress < 0.9f)
            {
                loading?.Invoke(asyncOperation.progress);
                yield return null;
            }
    
            //当asyncOperation.allowSceneActivation为false，则asyncOperation.progress最多只能到达0.9，我们人为让它凑成整数1，方便外部进度条的显示。
            loading?.Invoke(1f);
    
            //加载场景完毕之后，如果把这个变量设置为true，则会切换到该场景。如果为false，则不会切换到该场景。
            asyncOperation.allowSceneActivation = setActiveAfterCompleted;
    
            //确保场景激活
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
    
            //加载场景完毕之后执行的回调。
            completed?.Invoke(asyncOperation);
        }
    
        /// <summary>
        /// <para>异步加载场景。默认情况下，成功加载场景后，会切换到该场景。</para>
        /// <para>可以控制外部进度条的显示。</para>
        /// </summary>
        /// <param name="sceneName">要加载的场景名</param>
        /// <param name="loading">加载中的回调。只要在加载中，就会不断地执行这个回调。一般用于进度条的显示。</param>
        /// <param name="completed">加载完毕后的回调。</param>
        /// <param name="setActiveAfterCompleted">加载场景完毕后，是否切换到该场景。</param>
        /// <param name="mode">加载场景的模式。默认是LoadSceneMode.Single，表示会卸载原来的场景，再切换到新场景。LoadSceneMode.Additive表示会将新场景叠加在原来的场景中。</param>
        public void LoadSceneAsync(string sceneName, LoadSceneParameters parameters, UnityAction<float> loading = null,
            UnityAction<AsyncOperation> completed = null, bool setActiveAfterCompleted = true)
        {
            //开启协程进行异步加载。
            MonoManager.Instance.StartCoroutine(LoadSceneAsyncCoroutine(sceneName, parameters, loading, completed,
                setActiveAfterCompleted));
        }
    
        IEnumerator LoadSceneAsyncCoroutine(string sceneName, LoadSceneParameters parameters,
            UnityAction<float> loading = null, UnityAction<AsyncOperation> completed = null,
            bool setActiveAfterCompleted = true)
        {
            //异步操作对象，记录了异步操作的数据。
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, parameters);
    
            //设置为false，则即使场景加载完毕，也不会切换过去。当场景加载完毕，再次把这个值复制为true，才会切换过去。
            asyncOperation.allowSceneActivation = false;
    
            //加载场景的过程中，提供给外部执行的回调。一般用于进度条的显示。
            while (asyncOperation.progress < 0.9f)
            {
                loading?.Invoke(asyncOperation.progress);
                yield return null;
            }
    
            //当asyncOperation.allowSceneActivation为false，则asyncOperation.progress最多只能到达0.9，我们人为让它凑成整数1，方便外部进度条的显示。
            loading?.Invoke(1f);
    
            //加载场景完毕之后，如果把这个变量设置为true，则会切换到该场景。如果为false，则不会切换到该场景。
            asyncOperation.allowSceneActivation = setActiveAfterCompleted;
    
            //确保场景激活
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
    
            //加载场景完毕之后执行的回调。
            completed?.Invoke(asyncOperation);
        }
    
        /// <summary>
        /// <para>异步加载当前活动场景。</para>
        /// <para>即使当前场景没有加入到Build Settings窗口，本方法依然有效。</para>
        /// <para>如果当前只有一个场景，则本方法实际上就是重新载入当前场景。</para>
        /// </summary>
        /// <param name="loading">加载中的回调。只要在加载中，就会不断地执行这个回调。一般用于进度条的显示。</param>
        /// <param name="completed">加载完毕后的回调。</param>
        /// <param name="setActiveAfterCompleted">加载场景完毕后，是否切换到该场景。</param>
        /// <param name="mode">加载场景的模式。默认是LoadSceneMode.Single，表示会卸载原来的场景，再切换到新场景。LoadSceneMode.Additive表示会将新场景叠加在原来的场景中。</param>
        public void LoadActiveSceneAsync(UnityAction<float> loading = null, UnityAction<AsyncOperation> completed = null,
            bool setActiveAfterCompleted = true, LoadSceneMode mode = LoadSceneMode.Single)
        {
            LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, loading, completed, setActiveAfterCompleted, mode);
        }
    
        /// <summary>
        /// 异步加载Build Settings窗口中当前活动场景的下一个场景。
        /// </summary>
        /// <param name="isCyclical">是否循环加载场景。当要加载的场景的索引超过了Build Settings窗口中的最大索引，如果这个参数为true，则会加载索引为0的场景，然后返回true，如果这个参数为false，则视为加载失败，返回false，且不会加载该场景。</param>
        /// <param name="loading">加载中的回调。只要在加载中，就会不断地执行这个回调。一般用于进度条的显示。</param>
        /// <param name="completed">加载完毕后的回调。</param>
        /// <param name="setActiveAfterCompleted">加载场景完毕后，是否切换到该场景。</param>
        /// <param name="mode">加载场景的模式。默认是LoadSceneMode.Single，表示会卸载原来的场景，再切换到新场景。LoadSceneMode.Additive表示会将新场景叠加在原来的场景中。</param>
        public void LoadNextSceneAsync(bool isCyclical = false, UnityAction<float> loading = null,
            UnityAction<AsyncOperation> completed = null, bool setActiveAfterCompleted = true,
            LoadSceneMode mode = LoadSceneMode.Single)
        {
            //下一个场景的索引。
            int buildIndex = SceneManager.GetActiveScene().buildIndex + 1;
    
            //当要加载的场景的索引超过了Build Settings窗口中的最大索引，则根据参数来判断是否循环加载场景。
            if (buildIndex > SceneManager.sceneCountInBuildSettings - 1)
            {
                if (isCyclical)
                {
                    buildIndex = 0;
                    //异步加载该场景。
                    LoadSceneAsync(buildIndex, loading, completed, setActiveAfterCompleted, mode);
                }
                else
                {
                    Debug.LogWarning($"加载场景失败！要加载的场景的索引是{buildIndex}，超过了当前Build Settings窗口中的最大索引。");
                }
            }
        }
    
        /// <summary>
        /// 异步加载Build Settings窗口中当前活动场景的上一个场景。
        /// </summary>
        /// <param name="isCyclical">是否循环加载场景。当要加载的场景的索引超过了Build Settings窗口中的最大索引，如果这个参数为true，则会加载索引为0的场景，然后返回true，如果这个参数为false，则视为加载失败，返回false，且不会加载该场景。</param>
        /// <param name="loading">加载中的回调。只要在加载中，就会不断地执行这个回调。一般用于进度条的显示。</param>
        /// <param name="completed">加载完毕后的回调。</param>
        /// <param name="setActiveAfterCompleted">加载场景完毕后，是否切换到该场景。</param>
        /// <param name="mode">加载场景的模式。默认是LoadSceneMode.Single，表示会卸载原来的场景，再切换到新场景。LoadSceneMode.Additive表示会将新场景叠加在原来的场景中。</param>
        public void LoadPreviousSceneAsync(bool isCyclical = false, UnityAction<float> loading = null,
            UnityAction<AsyncOperation> completed = null, bool setActiveAfterCompleted = true,
            LoadSceneMode mode = LoadSceneMode.Single)
        {
            //上一个场景的索引。
            int buildIndex = SceneManager.GetActiveScene().buildIndex - 1;
    
            //当要加载的场景的索引为负数，则根据参数来判断是否循环加载场景。
            if (buildIndex < 0)
            {
                if (isCyclical)
                {
                    buildIndex = SceneManager.sceneCountInBuildSettings - 1;
                    //异步加载该场景。
                    LoadSceneAsync(buildIndex, loading, completed, setActiveAfterCompleted, mode);
                }
                else
                {
                    Debug.LogWarning($"加载场景失败！要加载的场景的索引是{buildIndex}，没有索引为负数的场景。");
                }
            }
        }
    
        /// <summary>
        /// <para>销毁指定的场景和这个场景中的所有游戏对象。</para>
        /// <para>本方法只对加载时用了LoadSceneMode.Additive来加载的场景有效。如果当前游戏中只有一个场景，则本方法无效，且会在控制台报黄色的警告。</para>
        /// <para>本方法不会释放内存中的场景资源，如果要释放该场景资源，应在调用这个方法后，再调用Resources.UnloadUnusedAssets方法。</para>
        /// </summary>
        /// <param name="sceneBuildIndex">要销毁的场景在Build Settings窗口中的索引。</param>
        /// <param name="callback">销毁完毕后执行的回调。</param>
        /// <param name="options">销毁场景的选项。是一个UnloadSceneOptions型枚举。</param>
        public void DestroySceneAsync(int sceneBuildIndex, UnityAction callback = null,
            UnloadSceneOptions options = UnloadSceneOptions.None)
        {
            MonoManager.Instance.StartCoroutine(DestroySceneCoroutine(sceneBuildIndex, callback, options));
        }
    
        IEnumerator DestroySceneCoroutine(int sceneBuildIndex, UnityAction callback = null,
            UnloadSceneOptions options = UnloadSceneOptions.None)
        {
            //异步操作对象，记录了异步操作的数据。
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneBuildIndex, options);
    
            if (asyncOperation == null)
            {
                Debug.LogWarning("要销毁的场景不合法，销毁无效！");
                yield break;
            }
    
            //如果没有卸载完成，则停在这里面等待。
            while (asyncOperation.progress < 0.9f)
                yield return null;
    
            //卸载场景完毕之后执行的回调。
            callback?.Invoke();
        }
    
    
        /// <summary>
        /// <para>销毁指定的场景和这个场景中的所有游戏对象。</para>
        /// <para>本方法只对加载时用了LoadSceneMode.Additive来加载的场景有效。如果当前游戏中只有一个场景，则本方法无效，且会在控制台报黄色的警告。</para>
        /// <para>本方法不会释放内存中的场景资源，如果要释放该场景资源，应在调用这个方法后，再调用Resources.UnloadUnusedAssets方法。</para>
        /// </summary>
        /// <param name="sceneName">要销毁的场景名或场景路径。</param>
        /// <param name="callback">销毁完毕后执行的回调。</param>
        /// <param name="options">销毁场景的选项。是一个UnloadSceneOptions型枚举。</param>
        public void DestroySceneAsync(string sceneName, UnityAction callback = null,
            UnloadSceneOptions options = UnloadSceneOptions.None)
        {
            MonoManager.Instance.StartCoroutine(DestroySceneCoroutine(sceneName, callback, options));
        }
    
        IEnumerator DestroySceneCoroutine(string sceneName, UnityAction callback = null,
            UnloadSceneOptions options = UnloadSceneOptions.None)
        {
            //异步操作对象，记录了异步操作的数据。
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName, options);
    
            if (asyncOperation == null)
            {
                Debug.LogWarning("要销毁的场景不合法，销毁无效！");
                yield break;
            }
    
            //如果没有卸载完成，则停在这里面等待。
            while (asyncOperation.progress < 0.9f)
                yield return null;
    
            //卸载场景完毕之后执行的回调。
            callback?.Invoke();
        }
    
        /// <summary>
        /// <para>销毁指定的场景和这个场景中的所有游戏对象。</para>
        /// <para>本方法只对加载时用了LoadSceneMode.Additive来加载的场景有效。如果当前游戏中只有一个场景，则本方法无效，且会在控制台报黄色的警告。</para>
        /// <para>本方法不会释放内存中的场景资源，如果要释放该场景资源，应在调用这个方法后，再调用Resources.UnloadUnusedAssets方法。</para>
        /// </summary>
        /// <param name="scene">要销毁的场景对象。</param>
        /// <param name="callback">销毁完毕后执行的回调。</param>
        /// <param name="options">销毁场景的选项。是一个UnloadSceneOptions型枚举。</param>
        public void DestroySceneAsync(Scene scene, UnityAction callback = null,
            UnloadSceneOptions options = UnloadSceneOptions.None)
        {
            MonoManager.Instance.StartCoroutine(DestroySceneCoroutine(scene, callback, options));
        }
    
        IEnumerator DestroySceneCoroutine(Scene scene, UnityAction callback = null,
            UnloadSceneOptions options = UnloadSceneOptions.None)
        {
            //异步操作对象，记录了异步操作的数据。
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(scene, options);
    
            if (asyncOperation == null)
            {
                Debug.LogWarning("要销毁的场景不合法，销毁无效！");
                yield break;
            }
    
            //如果没有卸载完成，则停在这里面等待。
            while (asyncOperation.progress < 0.9f)
                yield return null;
    
            //卸载场景完毕之后执行的回调。
            callback?.Invoke();
        }
    
        /// <summary>
        /// 判断指定的场景索引在Build Settings窗口中是否合法。
        /// </summary>
        /// <param name="buildIndex">指定的场景索引</param>
        bool IsSceneBuildIndexValid(int buildIndex)
        {
            if (buildIndex < 0)
            {
                Debug.LogWarning($"要加载的场景的索引不合法！该索引是{buildIndex}，不能为负数。");
                return false;
            }
    
            if (buildIndex > SceneManager.sceneCountInBuildSettings - 1)
            {
                Debug.LogWarning(
                    $"要加载的场景的索引不合法！该索引越界，越界的索引是{buildIndex},但是场景最大的索引是{SceneManager.sceneCountInBuildSettings - 1}。");
                return false;
            }
    
            return true;
        }
    
        /// <summary>
        /// 在控制台打印当前已加载的所有场景的信息。
        /// </summary>
        public void PrintLoadedScenesInfo()
        {
            Debug.Log($"当前已加载的场景有{SceneManager.sceneCount}个。");
    
            for (int i = 0; i < SceneManager.sceneCount; i++)
                Debug.Log($"索引为{i}的场景的名字是：{SceneManager.GetSceneAt(i).name}");
        }
    
        /// <summary>
        /// <para>返回当前活动场景的所有根游戏对象。隐藏的根游戏对象也会包含在其中。</para>
        /// <para>DontDestroyOnLoad的游戏对象不包含在其中。</para>
        /// </summary>
        public GameObject[] GetActiveSceneRootGameObjects()
        {
            return SceneManager.GetActiveScene().GetRootGameObjects();
        }
    
        /// <summary>
        /// <para>返回当前所有加载的场景的根游戏对象。隐藏的根游戏对象也会包含在其中。</para>
        /// <para>DontDestroyOnLoad的游戏对象不包含在其中。</para>
        /// </summary>
        public GameObject[] GetLoadedScenesRootGameObjects()
        {
            List<GameObject> list = new List<GameObject>();
    
            //把当前所有加载的场景的根游戏对象都逐一存储到列表中。
            for (int i = 0; i < SceneManager.sceneCount; i++)
                list.AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects());
    
            return list.ToArray();
        }
    }
}
