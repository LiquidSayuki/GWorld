using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.UI;

public class UIMain : MonoSingleton<UIMain>
{

    public Text avatarName;
    public Text avatarLevel;


    protected override void OnStart()
    {
        this.UpdateAvatar();
    }

    void UpdateAvatar()
    {
        this.avatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
        this.avatarLevel.text = User.Instance.CurrentCharacter.Level.ToString();
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
}
