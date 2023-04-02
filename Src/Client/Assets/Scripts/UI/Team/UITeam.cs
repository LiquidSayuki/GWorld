using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UITeam : UIWindow
{
    public Text teamTitle;
    public ListView list;
    public UITeamItem[] members;
    void Start()
    {
        foreach (var i in members)
        {
            this.list.AddItem(i);
        }
        if (User.Instance.TeamInfo == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
    }

    void OnEnable()
    {
        UpdateTeamUI();
    }


    internal void ShowTeam(bool show)
    {
        this.gameObject.SetActive(show);
        if (show)
        {
            UpdateTeamUI();
        }
    }

    private void UpdateTeamUI()
    {
        if(User.Instance.TeamInfo == null)
        {
            return;
        }
        NCharacterInfo teamLeader = User.Instance.TeamInfo.Members.FirstOrDefault((i) => i.Id == User.Instance.TeamInfo.Leader);

        this.teamTitle.text = string.Format("{0}的队伍 ({1}/5)", teamLeader.Name,User.Instance.TeamInfo.Members.Count);

        for(int i = 0; i < 5; i++)
        {
            if(i < User.Instance.TeamInfo.Members.Count)
            {
                this.members[i].SetMemberInfo(i, User.Instance.TeamInfo.Members[i], User.Instance.TeamInfo.Members[i].Id == User.Instance.TeamInfo.Leader);
                this.members[i].gameObject.SetActive(true);
            }
            else
            {
                this.members[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnClickLeave()
    {
        MessageBox.Show("确定要离开队伍吗？", "退出队伍", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            TeamService.Instance.SendTeamLeaveRequest(User.Instance.TeamInfo.Id);
        };
    }
}
