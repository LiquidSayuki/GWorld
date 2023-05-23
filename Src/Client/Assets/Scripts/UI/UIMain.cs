using Assets.Scripts.Managers;
using Assets.Scripts.UI;
using Entities;
using Managers;
using Models;
using System;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>
{

    public Text avatarName;
    public Text avatarLevel;

    public UITeam TeamWindow;
    public UICreatureInfo targetUI;

    protected override void OnStart()
    {
        this.UpdateAvatar();
        this.targetUI.gameObject.SetActive(false);
        BattleManager.Instance.OnTargetChanged += OnTargetChanged;
    }

    void UpdateAvatar()
    {
        this.avatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacterInfo.Name, User.Instance.CurrentCharacterInfo.Id);
        this.avatarLevel.text = User.Instance.CurrentCharacterInfo.Level.ToString();
    }
    private void OnTargetChanged(Creature creature)
    {
        if(targetUI != null)
        {
            if(!targetUI.isActiveAndEnabled) targetUI.gameObject.SetActive(true);
            targetUI.Target = creature;
        }
        else
        {
            targetUI.gameObject.SetActive(false);
        }
    }

    public void OnClickCharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        Services.UserService.Instance.SendGameLeave();
    }

    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();
    }

    public void OnClickEquipment()
    {
        UIManager.Instance.Show<UICharEquip>();
    }

    public void OnClickQuest()
    {
        UIManager.Instance.Show<UIQuestSystem>();
    }

    public void OnClickFriend()
    {
        UIManager.Instance.Show<UIFriend>();
    }

    public void ShowTeamUI(bool show)
    {
        TeamWindow.ShowTeam(show);
    }

    public void OnCilickGuild()
    {
        GuildManager.Instance.ShowGuild();
    }

    public void OnClickRide()
    {

    }
    public void OnClickSetting()
    {
        UIManager.Instance.Show<UISetting>();
    }

    public void OnClickSkill()
    {
        UIManager.Instance.Show<UISkill>();
    }
}
