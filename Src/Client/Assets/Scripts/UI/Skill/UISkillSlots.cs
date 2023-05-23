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
		var Skills = User.Instance.CurrentCharacter.SkillManager.Skills;
		int idx = 0;
		foreach(var skill in Skills)
		{
			slots[idx].SetSkill(skill);
			idx++;
		}
    }
}
