using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendItem : ListView.ListViewItem {

	public Text NickName;
	public Text Class;
	public Text Level;
	public Text Status;
	public Image Background;
	public Sprite NormalBg;
	public Sprite SelectedBg;

    public override void onSelected(bool selected)
    {
        this.Background.overrideSprite = selected? SelectedBg : NormalBg ;
    }

	public NFriendInfo Info;
    // Use this for initialization
    void Start () {
		
	}
	bool isEquiped = false;

	public void SetFriendInfo(NFriendInfo info)
	{
		this.Info= info;
		if (this.NickName != null) this.NickName.text = this.Info.friendInfo.Name;
		if (this.Class != null) this.Class.text = this.Info.friendInfo.Class.ToString();
		if(this.Level != null) this.Level.text = this.Info.friendInfo.Level.ToString();
		if (this.Status != null) this.Status.text = this.Info.Status == 1 ? "在线" : "离线";
	}
}
