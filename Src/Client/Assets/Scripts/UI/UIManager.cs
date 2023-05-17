using Assets.Scripts.UI;
using System;
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
            this.UIResources.Add(typeof(UIBag), new UIElement() { Resources = "UI/UIBag", Cache = false });
            this.UIResources.Add(typeof(UIShop), new UIElement() { Resources = "UI/UIShop", Cache = false });
            this.UIResources.Add(typeof(UICharEquip), new UIElement() { Resources = "UI/UICharEquip", Cache = false });

            this.UIResources.Add(typeof(UIQuestSystem), new UIElement() { Resources = "UI/UIQuest", Cache = false });
            this.UIResources.Add(typeof(UIQuestDialog), new UIElement() { Resources = "UI/UIDialog", Cache = false });

            this.UIResources.Add(typeof(UIFriend), new UIElement() { Resources = "UI/UIFriend", Cache = true });

            this.UIResources.Add(typeof(UIGuild), new UIElement() { Resources = "UI/Guild/UIGuild", Cache = true });
            this.UIResources.Add(typeof(UIGuildList), new UIElement() { Resources = "UI/Guild/UIGuildList", Cache = true });
            this.UIResources.Add(typeof(UIGuildPopNoGuild), new UIElement() { Resources = "UI/Guild/UIGuildPopNoGuild", Cache = false });
            this.UIResources.Add(typeof(UIGuildPopCreate), new UIElement() { Resources = "UI/Guild/UIGuildPopCreate", Cache = false });
            this.UIResources.Add(typeof(UIGuildApplyList), new UIElement() { Resources = "UI/Guild/UIGuildApplyList", Cache = false });

            this.UIResources.Add(typeof(UIPopCharMenu), new UIElement() { Resources = "UI/UIPopCharMenu", Cache = false });

            this.UIResources.Add(typeof(UISetting), new UIElement() { Resources = "UI/UISetting", Cache = false });
            this.UIResources.Add(typeof(UISystemConfig), new UIElement() { Resources = "UI/UISystemConfig", Cache = false });
        }

        ~UIManager()
        {

        }

        public T Show<T>()
        {
             SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Open);

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
                    if (prefab == null)
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
             SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Close);
            if (this.UIResources.ContainsKey(type))
            {
                UIElement element = this.UIResources[type];
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

