using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FDIM.Framework
{
    public class UIManager : SingletonMonoBase<UIManager>
    {
        private Transform _canvasRoot;
        private Transform rearmost, rear, middle, front, forefront;
        private readonly Dictionary<GameObject, GameObject> panelDictionary = new();

       
        /// <summary>
        /// 统一入口：确保 Canvas 与五个层都存在（若失效则重建）
        /// </summary>
        /// <returns></returns>

        private Transform EnsureCanvasRoot()
        {
            // 已有且未被销毁
            if (_canvasRoot) return _canvasRoot;

            var canvas = FindObjectOfType<Canvas>();
            GameObject go;

            if (canvas == null)
            {
                // 新建一个常驻 Canvas
                go = new GameObject("Canvas");
                go.layer = LayerMask.NameToLayer("UI");
                var c = go.AddComponent<Canvas>();
                c.renderMode = RenderMode.ScreenSpaceOverlay;
                c.sortingOrder = 30000;

                var scaler = go.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
                scaler.matchWidthOrHeight = Screen.width > Screen.height ? 1 : 0;

                go.AddComponent<GraphicRaycaster>();
                DontDestroyOnLoad(go);
            }
            else
            {
                go = canvas.gameObject;
                if (!go.GetComponent<GraphicRaycaster>()) go.AddComponent<GraphicRaycaster>();
                if (!go.GetComponent<CanvasScaler>())
                {
                    var scaler = go.AddComponent<CanvasScaler>();
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
                    scaler.matchWidthOrHeight = Screen.width > Screen.height ? 1 : 0;
                }
            }

            _canvasRoot = go.transform;

            // 确保五个层存在
            rearmost = EnsureChild(_canvasRoot, "Rearmost", ref rearmost);
            rear = EnsureChild(_canvasRoot, "Rear", ref rear);
            middle = EnsureChild(_canvasRoot, "Middle", ref middle);
            front = EnsureChild(_canvasRoot, "Front", ref front);
            forefront = EnsureChild(_canvasRoot, "Forefront", ref forefront);

            return _canvasRoot;
        }

        private static Transform EnsureChild(Transform root, string name, ref Transform cache)
        {
            if (cache) return cache;                  
            var t = root.Find(name);
            if (t == null)
            {
                var go = new GameObject(name);
                t = go.transform;
                t.SetParent(root, false);
            }
            cache = t;
            return cache;
        }

        private Transform GetLayer(E_PanelDisplayedLayer layer)
        {
            EnsureCanvasRoot(); // 先确保 Canvas/层都在
            return layer switch
            {
                E_PanelDisplayedLayer.Rearmost => rearmost,
                E_PanelDisplayedLayer.Rear => rear,
                E_PanelDisplayedLayer.Middle => middle,
                E_PanelDisplayedLayer.Front => front,
                E_PanelDisplayedLayer.Forefront => forefront,
                _ => middle
            };
        }

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;             

            EnsureCanvasRoot();
            CreateEventSystem();
        }

        private static void CreateEventSystem()
        {
            if (FindObjectOfType<EventSystem>()) return;
            var es = new GameObject("EventSystem");
            DontDestroyOnLoad(es);
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        // —— 业务 API —— 
        public void ShowPanel(GameObject panelPrefab, E_PanelDisplayedLayer layer = E_PanelDisplayedLayer.Middle)
        {
            if (!panelPrefab)
            {
                Debug.LogWarning("显示面板失败：panelPrefab 为 null");
                return;
            }

            PruneDeadPanels();
            if (panelDictionary.ContainsKey(panelPrefab)) return;

            var panel = Instantiate(panelPrefab);
            panel.name = panelPrefab.name;
            panelDictionary.Add(panelPrefab, panel);

            var parent = GetLayer(layer) ?? GetLayer(E_PanelDisplayedLayer.Middle);
            panel.transform.SetParent(parent, false);     // 只设置一次父节点，避免中途访问到无效 Transform
        }

        public void HidePanel(GameObject panelPrefab)
        {
            PruneDeadPanels();
            if (!panelPrefab) return;
            if (!panelDictionary.TryGetValue(panelPrefab, out var inst) || !inst) return;

            Destroy(inst);
            panelDictionary.Remove(panelPrefab);
        }

        public void HidePanel(string panelPrefabName)
        {
            PruneDeadPanels();
            GameObject key = null;
            foreach (var k in panelDictionary.Keys)
            {
                if (k && k.name == panelPrefabName) { key = k; break; }
            }
            if (key) HidePanel(key);
        }

        public bool IsPanelShowed(GameObject panelPrefab)
        {
            PruneDeadPanels();
            return panelPrefab && panelDictionary.ContainsKey(panelPrefab);
        }

        public bool IsPanelShowed(string panelPrefabName)
        {
            PruneDeadPanels();
            foreach (var k in panelDictionary.Keys)
                if (k && k.name == panelPrefabName) return true;
            return false;
        }

        private void PruneDeadPanels()
        {
            var dead = new List<GameObject>();
            foreach (var kv in panelDictionary)
                if (!kv.Value) dead.Add(kv.Key);
            foreach (var k in dead) panelDictionary.Remove(k);
        }

        /// <summary>
        ///  暴露只读层引用，如果你有外部脚本要读：
        /// </summary>
        public Transform Rearmost => EnsureCanvasRoot() ? rearmost : null;
        public Transform Rear => EnsureCanvasRoot() ? rear : null;
        public Transform Middle => EnsureCanvasRoot() ? middle : null;
        public Transform Front => EnsureCanvasRoot() ? front : null;
        public Transform Forefront => EnsureCanvasRoot() ? forefront : null;
    }
}
