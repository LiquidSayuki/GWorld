using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Common.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestSystem :UIWindow {
    public Text title;

    public GameObject questItemPrefab;

    public TabView tabs;
    //public ListView listMain;
    //public ListView listBranch;

    public ListView QuestList;

    public ListView listOngoing;
    public ListView listAvaliable;

    public UIQuestInfo questInfo;

    private bool showAvaliableList = false;

    void Start()
    {
        /* this.listOngoing.onItemSelected += this.OnQuestSelected;
        this.listAvaliable.onItemSelected += this.OnQuestSelected;*/
        this.QuestList.onItemSelected += this.OnQuestSelected;
        this.tabs.OnTabSelect += OnSelectTab;
        RefreshUI();
    }

    public void OnQuestSelected(ListView.ListViewItem item)
    {
        UIQuestItem questItem = item as UIQuestItem;
        this.questInfo.SetQuestInfo(questItem.quest);
    }

    private void OnSelectTab(int idx)
    {
        showAvaliableList = idx == 1;
        RefreshUI();
    }

    private void InitAllQuestItems()
    {
        foreach (var kv in QuestManager.Instance.allQuests)
        {
            if (showAvaliableList)
            {
                if (kv.Value.Info != null)
                {
                    continue;
                }
                GameObject go = Instantiate(questItemPrefab, listAvaliable.transform);
                UIQuestItem ui = go.GetComponent<UIQuestItem>();
                ui.SetQuestInfo(kv.Value);

                this.QuestList.AddItem(ui);

/*                if (kv.Value.Define.Type == QuestType.Main)
                    this.listMain.AddItem(ui as ListView.ListViewItem);
                else
                    this.listBranch.AddItem(ui as ListView.ListViewItem);*/

            }
            else
            {
                if (kv.Value.Info == null)
                    continue;
                GameObject go = Instantiate(questItemPrefab, listOngoing.transform);
                UIQuestItem ui = go.GetComponent<UIQuestItem>();
                ui.SetQuestInfo(kv.Value);

                this.QuestList.AddItem(ui);

/*                if (kv.Value.Define.Type == QuestType.Main)
                    this.listMain.AddItem(ui as ListView.ListViewItem);
                else
                    this.listBranch.AddItem(ui as ListView.ListViewItem);*/

            }
        }
    }

    #region Refresh
    private void ClearAllQuestList()
    {
        this.QuestList.RemoveAll();
        this.listOngoing.RemoveAll();
        this.listAvaliable.RemoveAll();
    }
    void RefreshUI()
    {
        ClearAllQuestList();
        InitAllQuestItems();
    }
    #endregion
}
