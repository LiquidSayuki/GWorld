using JetBrains.Annotations;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITeamItem : ListView.ListViewItem {

	public Text nickName;
	public Text level;
	public Image classIcon;
	public Image leaderIcon;
	public Image background;
	public int index;
	public NCharacterInfo Info;

    public override void onSelected(bool selected)
    {
        this.background.enabled = selected? true: false;
    }

    // Use this for initialization
    void Start () {
		this.background.enabled=false;
	}

	public void SetMemberInfo(int index, NCharacterInfo info, bool isLeader)
	{
		this.index = index;
		this.Info = info;
		if(this.nickName != null) this.nickName.text = this.Info.Name;
		if (this.level != null) this.level.text = this.Info.Level.ToString();
		if(this.classIcon != null) this.classIcon.overrideSprite = SpriteManager.Instance.ClassIcons[(int) this.Info.Class -1];
		if(this.leaderIcon != null) this.leaderIcon.gameObject.SetActive(isLeader);
	}
}
