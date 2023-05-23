using Battle;
using Common.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillItem : ListView.ListViewItem
{
    public Text SkillName;
    public Text SkillLevel;
    public Image SkillIcon;

    public Image Background;
    public Sprite NormalBg;
    public Sprite SelectedBg;

    public Skill item;
    public override void onSelected(bool selected)
    {
        this.Background.overrideSprite = selected ? SelectedBg : NormalBg;
    }

    internal void SetItem(Skill value, UISkill uISkill, bool v)
    {
        this.item = value;

        if(this.SkillName != null) this.SkillName.text = this.item.Define.Name;
        if(this.SkillLevel != null) this.SkillLevel.text = this.item.Info.Level.ToString();
        if (this.SkillIcon != null) this.SkillIcon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
    }
}
