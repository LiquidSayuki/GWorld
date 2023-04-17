using Assets.Scripts.Managers;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPopCharMenu : UIWindow, IDeselectHandler {
    public int targetId;
    public string targetName;
	
	public void OnDeselect(BaseEventData eventData)
    {
        // 点击事件
        var ed = eventData as PointerEventData;
        // 悬浮在这个界面之上
        if (ed.hovered.Contains(this.gameObject)) return;
        // 点击了非此界面的内容，关闭
        this.Close(WindowResult.None);
    }

    public void OnEnable()
    {
        //使用selectable和取消选中回调来实现自动关闭窗口的行为
        this.GetComponent<Selectable>().Select();
        this.Root.transform.position = Input.mousePosition + new Vector3(-10,-10,0);
    }

    public void OnChat()
    {
        ChatManager.Instance.StartPrivateChat(targetId, targetName);
        this.Close(WindowResult.No);
    }

    public void OnAddFriend()
    {
        FriendService.Instance.SendFriendAddRequest(targetId, targetName);
        this.Close(WindowResult.No);
    }

    public void OnInviteTeam()
    {
        TeamService.Instance.SendTeamInviteRequest(targetId, targetName);
        this.Close(WindowResult.No);
    }
}
