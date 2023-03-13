using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class UIManager : Singleton<UIManager>
    {

        class UIElement
        {
            public string Resources;
            public bool Cache;
            public GameObject Instance;
        }

        private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

        public UIManager()
        {
            //this.UIResources.Add(typeof(UITest), new UIElement() { Resources = "UI/UITest", Cache = true });
            this.UIResources.Add(typeof(UIBag), new UIElement() { Resources = "UI/UIBag", Cache = false});
            this.UIResources.Add(typeof(UIShop), new UIElement() { Resources = "UI/UIShop", Cache = false });
            this.UIResources.Add(typeof(UICharEquip), new UIElement() { Resources = "UI/UICharEquip", Cache = false });
            this.UIResources.Add(typeof(UIQuestSystem), new UIElement() { Resources = "UI/UIQuest", Cache = false });
            this.UIResources.Add(typeof(UIQuestDialog), new UIElement() { Resources = "UI/UIDialog", Cache = false });
        }

        ~UIManager()
        {

        }

        public T Show<T>()
        {
            // SoundManager.Instance.PlaySOund("ui_open");
            Type type = typeof(T);
            // UI类型检查
            if (this.UIResources.ContainsKey(type))
            {
                UIElement info = this.UIResources[type];
                // 如果manager中已经管理了一个游戏实体，激活它
                if (info.Instance != null)
                {
                    info.Instance.SetActive(true);
                }
                else
                {
                    //如果游戏实体还没有创建，从unity的资源路径中读取，创建它
                    UnityEngine.Object prefab = Resources.Load(info.Resources);
                    if(prefab == null)
                    {
                        return default(T);
                    }
                    info.Instance = (GameObject)GameObject.Instantiate(prefab);
                }
                return info.Instance.GetComponent<T>();
            }
            return default(T);
        }

        public void Close(Type type)
        {
            // SoundManager.Instance.PlaySound("ui_close");
            if (this.UIResources.ContainsKey(type))
            {
                UIElement element= this.UIResources[type];
                if (element.Cache)
                {
                    element.Instance.SetActive(false);
                }
                else
                {
                    GameObject.Destroy(element.Instance);
                    element.Instance = null;
                }
            }
        }
    }
}

