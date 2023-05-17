using Assets.Scripts.Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkillSlots : MonoBehaviour {

	public UISkillSlotItem[] slots;

	// Use this for initialization
	void Start () {
		RefreshUI();
	}

    private void RefreshUI()
    {
		var Skills = DataManager.Instance.Skills[(int)User.Instance.CurrentCharacterInfo.Class];
		int idx = 0;
		foreach(var kv in Skills)
		{
			slots[idx].SetSkill(kv.Value);
			idx++;
		}
    }
}
