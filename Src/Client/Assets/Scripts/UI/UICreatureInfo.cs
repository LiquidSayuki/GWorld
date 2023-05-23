﻿using Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreatureInfo : MonoBehaviour {
	public Text Name;
	public Slider HPBar;
	public Slider MPBar;
	public Text HPText;
	public Text MPText;

	void Start ()
	{

	}

	private Creature target;
	public Creature Target
	{
		get { return target; }
		set
		{
			this.target = value;
			this.UpdateUI();
		}
	}
	private void UpdateUI()
    {
		this.Name.text = string.Format("{0} Lv.{1}", target.Name, target.Info.Level);
		//TODO:更新头像

		this.HPBar.maxValue = target.Attributes.MaxHP;
		this.HPBar.value = target.Attributes.HP;
		this.HPText.text = string.Format("{0}/{1}", target.Attributes.HP, target.Attributes.MaxHP);

        this.MPBar.maxValue = target.Attributes.MaxMP;
        this.MPBar.value = target.Attributes.MP;
        this.MPText.text = string.Format("{0}/{1}", target.Attributes.MP, target.Attributes.MaxMP);
    }

    void Update () {
		this.UpdateUI();
	}
}