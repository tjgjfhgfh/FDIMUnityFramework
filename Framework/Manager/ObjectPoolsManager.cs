using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 对象池管理器。可以通过这个管理器从对象池生成游戏对象，也可以回收游戏对象进对象池。
/// </summary>
public class ObjectPoolsManager : SingletonPatternBase<ObjectPoolsManager>
{
    //所有对象池的父物体
    GameObject poolsParent;

    //所有对象池的共同父物体在Hierarchy窗口的名字
    readonly string poolsParentName = "ObjectPools";

    //当前所有对象池的列表
    public List<ObjectPool> objectPoolsList = new List<ObjectPool>();

    //键：由对象池生成的并且正在使用的游戏对象
    //值：这个游戏对象所属的对象池
    public Dictionary<GameObject, ObjectPool> objectsDictionary = new Dictionary<GameObject, ObjectPool>();

    /// <summary>
    /// <para>从对象池获取一个对象，并返回这个对象。</para>
    /// <para>如果对象池中有，则从对象池中取出来用。</para>
    /// <para>如果对象池中没有，则实例化该对象。</para>
    /// </summary>
    /// <param name="prefab">要生成的游戏对象的预制体</param>
    /// <param name="position">生成游戏对象的位置。如果没有设置父物体，则是世界坐标。如果设置了父物体，则是局部坐标。</param>
    /// <param name="rotation">游戏对象的旋转情况</param>
    /// <param name="parent">游戏对象的父物体</param>
    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        //如果要生成的物体的预制体为null，则不进行生成，而是直接返回null
        if (prefab == null) return null;

        //如果场景中没有对象池的父物体，则生成一个空物体，作为所有对象池的父物体。
        CreatePoolsParentIfNull();

        //先通过预制体来查找它所属的对象池，如果找到了，则返回该对象池。
        //如果找不到，则创建一个该预制体所对应的对象池，并返回。
        ObjectPool objectPool = FindPoolByPrefabOrCreatePool(prefab);

        //如果对象池中有，则从对象池中取出来用。
        //如果对象池中没有，则实例化该对象。
        //最后返回这个游戏对象。
        GameObject go = objectPool.Spawn(position, rotation, parent);

        //把生成的游戏对象与它所属的对象池记录到字典
        objectsDictionary.Add(go, objectPool);

        //返回游戏对象
        return go;
    }

    /// <summary>
    /// <para>隐藏指定的游戏对象，并把它回收进象池中。</para>
    /// <para>如果该游戏对象为null，则什么也不做。</para>
    /// <para>先从对象池生成的正在使用的游戏对象的字典中找指定的游戏对象，如果找到了，则把它隐藏并放入对应的对象池中。</para>
    /// <para>如果找不到，则遍历每一个对象池，再遍历其中的每一个游戏对象。如果找到了，则把它隐藏并放入对应的对象池中。</para>
    /// <para>如果都找不到，则什么也不做。</para>
    /// </summary>
    /// <param name="go">要放入对象池的游戏对象</param>
    /// <param name="delayTime">延迟多少秒执行。如果传入0或负数，则表示不延迟，而是立即执行。</param>
    public void Despawn(GameObject go, float delayTime = 0)
    {
        //如果该游戏为null，则什么也不做。
        if (go == null) return;

        //开启协程，延迟执行回收到对象池的逻辑。
        MonoManager.Instance.StartCoroutine(DespawnCoroutine(go, delayTime));
    }

    IEnumerator DespawnCoroutine(GameObject go, float delayTime = 0)
    {
        //等待指定秒数
        if (delayTime > 0)
            yield return new WaitForSeconds(delayTime);

        //先从象池生成的正在使用的游戏对象的字典中找指定的游戏对象，如果找到了，则把它放入对应的对象池中。
        if (objectsDictionary.TryGetValue(go, out ObjectPool pool))
        {
            //在从象池生成的正在使用的游戏对象的字典中移除这个游戏对象的键值对
            objectsDictionary.Remove(go);

            //把这个游戏对象放入找到的对象池
            pool.Despawn(go);
        }
        //如果找不到，则遍历每一个对象池，再遍历其中的每一个游戏对象。如果找到了，则把它放入对应的对象池中。
        else
        {
            //获取这个游戏对象所属的对象池
            pool = FindPoolByUsedGameObject(go);

            //如果找到了，则把这个游戏对象放入这个对象池中。
            if (pool != null)
                pool.Despawn(go);
        }
    }

    /// <summary>
    /// 把所有通过对象池生成的对象全部隐藏，并回收进对象池中。
    /// </summary>
    public void DespawnAll()
    {
        //遍历每一个对象池，把这些对象池中所有正在使用的游戏对象全部隐藏并放入对象池中。
        for (int i = 0; i < objectPoolsList.Count; i++)
        {
            objectPoolsList[i].DespawnAll();
        }

        //此时所有用对象池生成的游戏对象都隐藏并放入了对象池中，因此清空由对象池生成的并且正在使用的游戏对象的字典。
        objectsDictionary.Clear();
    }

    /// <summary>
    /// <para>在对象池中预加载指定数量的游戏对象。</para>
    /// <para>如果预制体为null，则什么也不会做。</para>
    /// <para>如果预加载0或负数个游戏对象，则什么也不会做。</para>
    /// </summary>
    /// <param name="amount">要预加载的数量</param>
    /// <param name="prefab">游戏对象的预制体</param>
    public void Preload(GameObject prefab, int amount = 1)
    {
        //如果预制体为null，则什么也不会做。
        if (prefab == null) return;

        //如果预加载0或负数个游戏对象，则什么也不会做。
        if (amount <= 0) return;

        //先通过预制体来查找它所属的对象池，如果找到了，则返回该对象池。
        //如果找不到，则创建一个该预制体所对应的对象池，并返回。
        ObjectPool pool = FindPoolByPrefabOrCreatePool(prefab);

        //预加载指定数量的游戏对象
        pool.Preload(amount);
    }

    /// <summary>
    /// <para>返回指定的预制体所属的对象池的容量。</para>
    /// <para>如果该对象池不存在，则会创建它，然后返回-1。</para>
    /// </summary>
    /// <param name="prefab">预制体</param>
    public int GetCapacity(GameObject prefab)
    {
        ObjectPool pool = FindPoolByPrefabOrCreatePool(prefab);

        return pool.capacity;
    }

    /// <summary>
    /// <para>设置指定的预制体所属的对象池的容量。</para>
    /// <para>如果该对象池不存在，则会创建它，然后再设置它的容量。</para>
    /// </summary>
    /// <param name="prefab">预制体</param>
    /// <param name="capacity">要设置的容量。如果设置为负数，则表示这个对象池可以容纳无数个游戏对象。</param>
    public void SetCapacity(GameObject prefab, int capacity = -1)
    {
        ObjectPool pool = FindPoolByPrefabOrCreatePool(prefab);

        pool.capacity = capacity;
    }

    /// <summary>
    ///如果场景中没有对象池的父物体，则生成一个空物体，作为所有对象池的父物体。
    /// </summary>
    void CreatePoolsParentIfNull()
    {
        if (poolsParent == null)
        {
            //清空列表和字典，避免上一个场景的数据的影响。
            objectPoolsList.Clear();
            objectsDictionary.Clear();

            //生成一个空物体
            poolsParent = new GameObject(poolsParentName);
        }
    }

    /// <summary>
    /// <para>查找并返回指定的预制体所属的对象池</para>
    /// <para>如果找不到，则返回null</para>
    /// </summary>
    /// <param name="prefab">预制体</param>
    ObjectPool FindPoolByPrefab(GameObject prefab)
    {
        if (prefab == null) return null;

        //遍历每一个对象池，如果这个对象池管理的预制体就是所要找的预制体，则返回这个对象池。
        for (int i = 0; i < objectPoolsList.Count; i++)
        {
            if (objectPoolsList[i].prefab == prefab)
                return objectPoolsList[i];
        }

        // 如果找不到，则返回null
        return null;
    }

    /// <summary>
    /// <para>在所有由对象池生成的且正在使用的游戏对象中查找并返回指定的游戏对象所属的对象池。</para>
    /// <para>如果找不到，则返回null</para>
    /// </summary>
    /// <param name="go">要查找的游戏对象</param>
    ObjectPool FindPoolByUsedGameObject(GameObject go)
    {
        if (go == null) return null;

        //先遍历每一个对象池
        for (int i = 0; i < objectPoolsList.Count; i++)
        {
            ObjectPool pool = objectPoolsList[i];

            //再遍历每一个对象池正在使用的所有游戏对象
            //如果该游戏对象就是所要找的游戏对象，则返回这个对象池。
            for (int j = 0; j < pool.usedGameObjectsList.Count; j++)
            {
                if (pool.usedGameObjectsList[j] == go)
                    return pool;
            }
        }

        //如果找不到，则返回null
        return null;
    }

    /// <summary>
    /// <para>先通过预制体来查找它所属的对象池，如果找到了，则返回该对象池。</para>
    /// <para>如果找不到，则创建一个该预制体所对应的对象池，并返回。</para>
    /// </summary>
    /// <param name="prefab">预制体</param>
    ObjectPool FindPoolByPrefabOrCreatePool(GameObject prefab)
    {
        //如果场景中没有对象池的父物体，则生成一个空物体，作为所有对象池的父物体。
        CreatePoolsParentIfNull();

        //查找并返回该预制体所属的对象池
        ObjectPool objectPool = FindPoolByPrefab(prefab);

        //如果该对象池不存在，则创建一个。
        if (objectPool == null)
        {
            //创建一个对象池
            objectPool = new GameObject($"ObjectPool（{prefab.name}）").AddComponent<ObjectPool>();

            //设置这个对象池所管理的预制体，就是这个游戏物体的预制体。
            objectPool.prefab = prefab;

            //把生成的对象池放到父物体中，方便管理。
            objectPool.transform.SetParent(poolsParent.transform);

            //记录这个对象池
            objectPoolsList.Add(objectPool);
        }

        return objectPool;
    }

    /// <summary>
    /// 对象池
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        //这个对象池存储的游戏对象的预制体
        public GameObject prefab;

        //对象池最多能容纳多少个游戏对象。负数表示可以容纳无数个。
        public int capacity = -1;

        //从这个对象池中取出并正在使用的游戏对象
        public List<GameObject> usedGameObjectsList = new List<GameObject>();

        //存在这个对象池中并没有被使用的游戏对象
        public List<GameObject> unusedGameObjectsList = new List<GameObject>();

        //这个对象池中正在使用和没有被使用的游戏对象的总数
        public int TotalGameObjectsCount
        {
            get => usedGameObjectsList.Count + unusedGameObjectsList.Count;
        }

        /// <summary>
        /// <para>从对象池获取一个对象，并返回这个对象。</para>
        /// <para>如果对象池中有，则从对象池中取出来用。</para>
        /// <para>如果对象池中没有，则实例化该对象。</para>
        /// </summary>
        /// <param name="position">游戏对象的旋转情况</param>
        /// <param name="rotation">游戏对象的父物体</param>
        /// <param name="parent">生成的游戏对象的父物体</param>
        public GameObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            //要实例化的游戏对象
            GameObject go;

            //如果对象池中有，则从对象池中取出来用。
            if (unusedGameObjectsList.Count > 0)
            {
                go = unusedGameObjectsList[0];

                unusedGameObjectsList.RemoveAt(0);

                usedGameObjectsList.Add(go);

                go.transform.localPosition = position;

                go.transform.localRotation = rotation;

                go.transform.SetParent(parent, false);

                go.SetActive(true);
            }
            //如果对象池中没有，则实例化该对象。
            else
            {
                go = Instantiate(prefab, position, rotation, parent);

                usedGameObjectsList.Add(go);
            }

            //如果该游戏对象身上继承Monobehavior的脚本中写了名叫OnSpawn的方法，则会执行它们。
            go.SendMessage("OnSpawn", SendMessageOptions.DontRequireReceiver);

            return go;
        }

        /// <summary>
        /// <para>隐藏指定的游戏对象，并把它回收进对象池中。</para>
        /// <para>如果该游戏对象为null，或者该对象不是由对象池生成的，或者该对象已经隐藏放入对象池中了，则什么也不做。</para>
        /// </summary>
        /// <param name="go">要放入对象池的游戏对象</param>
        public void Despawn(GameObject go)
        {
            if (go == null) return;

            //遍历这个对象池中所有正在使用的游戏对象
            for (int i = 0; i < usedGameObjectsList.Count; i++)
            {
                //如果找到了指定的游戏对象，则把它放入对象池中。然后返回。
                if (usedGameObjectsList[i] == go)
                {
                    //如果这个对象池的容量不为负数，且容纳的游戏对象已经满了，则把0号的游戏对象删掉，确保之后新的游戏对象能放入到池子中。
                    if (capacity >= 0 && unusedGameObjectsList.Count >= capacity)
                    {
                        if (unusedGameObjectsList.Count > 0)
                        {
                            Destroy(unusedGameObjectsList[0]);
                            unusedGameObjectsList.RemoveAt(0);
                        }
                    }

                    //把游戏对象放入到对象池中
                    unusedGameObjectsList.Add(go);
                    usedGameObjectsList.RemoveAt(i);

                    //如果该游戏对象身上继承Monobehavior的脚本中写了名叫OnDespawn的方法，则会执行它们。
                    go.SendMessage("OnDespawn", SendMessageOptions.DontRequireReceiver);

                    go.SetActive(false);

                    go.transform.SetParent(transform, false);

                    return;
                }
            }
        }

        /// <summary>
        /// 把通过这个对象池生成的所有游戏对象全部隐藏并放入对象池中
        /// </summary>
        public void DespawnAll()
        {
            //有几个游戏对象，就遍历几次。
            //之所以要在这里刻意声明一个变量来记录次数，是因为在循环的时候，列表元素的个数会发生变量。
            int count = usedGameObjectsList.Count;

            //遍历这个对象池正在使用的游戏对象的列表，把这些游戏对象全部隐藏并放入对象池中。
            for (int i = 1; i <= count; i++)
            {
                //每次遍历会把第一个元素放入对象池，之后会把它从字典中移除，于是后面的元素会往前移动一位，即原来的第二个元素会变成第一个元素。
                //不断放入对象池，就能把所有游戏对象都放入其中。
                Despawn(usedGameObjectsList[0]);
            }

            //清空列表
            usedGameObjectsList.Clear();
        }

        /// <summary>
        /// <para>在这个对象池中预加载指定数量的游戏对象。</para>
        /// <para>如果预加载0或负数个游戏对象，则什么也不会做。</para>
        /// </summary>
        /// <param name="amount">要预加载的数量</param>
        public void Preload(int amount = 1)
        {
            if (prefab == null) return;

            if (amount <= 0) return;

            for (int i = 1; i <= amount; i++)
            {
                //实例化游戏对象
                GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity);

                //隐藏游戏对象
                go.SetActive(false);

                //把游戏对象放入对象池物体之下，作为它的子物体。
                go.transform.SetParent(transform, false);

                //记录进对象池没有使用的游戏对象的列表中
                unusedGameObjectsList.Add(go);

                //游戏对象在Hierarchy面板的名字就是该预制体的名字
                go.name = prefab.name;
            }
        }
    }
}