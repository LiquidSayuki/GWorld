using Assets.Scripts.Managers;
using Common.Data;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkill : UIWindow {
	public Text Description;
	public GameObject ItemPrefab;
	public ListView ListMain;
	public UISkillItem SelectedItem;

	void Start () {
		RefreshUI();
		this.ListMain.onItemSelected += this.OnItemSelected;
	}

    public void OnItemSelected(ListView.ListViewItem item)
	{
		this.SelectedItem = item as UISkillItem;
	}

    private void RefreshUI()
    {
        ClearItems();
        InitItems();
    }

    private void ClearItems()
    {
        this.ListMain.RemoveAll();
    }

    private void InitItems()
    {
        var Skills = DataManager.Instance.Skills[(int)User.Instance.CurrentCharacterInfo.Class];
        foreach(var kv in Skills)
        {
            if(kv.Value.Type == SkillType.Skill)
            {
                GameObject go = Instantiate(ItemPrefab, this.ListMain.transform);
                UISkillItem ui = go.GetComponent<UISkillItem>();
                ui.SetItem(kv.Value, this, false);
                this.ListMain.AddItem(ui);
            }
        }
    }
}
