using Assets.Scripts.Managers;
using Candlelight.UI;
using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : MonoBehaviour {

    public HyperText textArea;
    public TabView channelTab;
    public InputField inputField;
    public Text chatTarget;
    public Dropdown channelSelect;

    void Start ()
    {
        this.channelTab.OnTabSelect += OnDisplayChannelSelected;
        ChatManager.Instance.OnChat += RefreshUI;


    }
    private void OnDestroy()
    {
        ChatManager.Instance.OnChat -= RefreshUI;
    }
    void Update()
    {
        InputManager.Instance.IsInputMode = inputField.isFocused;
    }
    private void OnDisplayChannelSelected(int idx)
    {
        ChatManager.Instance.displayChannel = (ChatManager.LocalChannel)idx;
        RefreshUI();
    }

    private void RefreshUI()
    {
        //获取并展示当前所有聊天文本
        this.textArea.text = ChatManager.Instance.GetCurrentMessages();
        //更新当前发送信息频道的DropDown选中项
        this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        //私聊时展示私聊对象提示
        if(ChatManager.Instance.SendChannel == ChatChannel.Private)
        {
            this.chatTarget.transform.parent.gameObject.SetActive(true);
            if(ChatManager.Instance.PrivateID != 0)
            {
                this.chatTarget.text = ChatManager.Instance.PrivateName + ":";
            }
            else
            {
                this.chatTarget.text = "<无对象>";
            }
        }
        else//其他聊天隐藏私聊提示
        {
            this.chatTarget.transform.parent.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 点击聊天信息时
    /// </summary>
    /// <param name="text"></param>
    /// <param name="link"></param>
    public void OnClickChatLink(HyperText text, HyperText.LinkInfo link)
    {
        if (string.IsNullOrEmpty(link.Name)) return;
        // <a name="c:1001:Name" class="player">Name</a> HTML标签来区分道具，角色等
        // <a name="i:1001:Name" class="player">Name</a> 
        if (link.Name.StartsWith("c:"))
        {
            string[] strs = link.Name.Split(':');
            UIPopCharMenu menu = UIManager.Instance.Show<UIPopCharMenu>();
            menu.targetId = int.Parse(strs[1]);
            menu.targetName = strs[2];
        }
        /*        if (link.Name.StartsWith("i:"))
               {
                   string[] strs = link.Name.Split(':');
                   UIPopCharMenu menu = UIManager.Instance.Show<UIPopCharMenu>();
                   menu.targetId = int.Parse(strs[1]);
                   menu.targetName = strs[2];
               }*/
    }

    public void OnClickSend()
    {
        OnEndInput(this.inputField.text);
    }

    public void OnEndInput(string text)
    {
        if (!string.IsNullOrEmpty(text.Trim()))
        {
            this.SendChat(text);
        }
        this.inputField.text = "";
    }

    void SendChat(string content)
    {
        ChatManager.Instance.SendChat(content, ChatManager.Instance.PrivateID, ChatManager.Instance.PrivateName);
    }

    public void OnSendChannelChanged(int idx)
    {
        if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(idx + 1)) return;

        // 发送频道无综合频道，所以存在1的偏移量
        if(!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)(idx + 1)))
        {
            this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        }
        else
        {
            this.RefreshUI();
        }
    }
}
