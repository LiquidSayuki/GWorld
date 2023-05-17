using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetting : UIWindow {

	public void ExitToCharSelect()
	{
		SceneManager.Instance.LoadScene("CharSelect");
		SoundManager.Instance.Playmusic(SoundDefine.Music_Select);
		Services.UserService.Instance.SendGameLeave();
	}

	public void SystemConfig()
	{
		UIManager.Instance.Show<UISystemConfig>();
		this.Close();
	}

	public void ExitGame()
	{
		Services.UserService.Instance.SendGameLeave(true);
	}
}
