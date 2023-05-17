using Common.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkillSlotItem : MonoBehaviour,IPointerClickHandler {

	public Image icon;
	public Image CDOverlay;
	public Text CDText;

	SkillDefine skill;

	private float overlaySpeed = 0;
	private float cdRemain = 0;

	void Update () 
	{
		if(CDOverlay.fillAmount > 0)
		{
			CDOverlay.fillAmount = this.cdRemain / this.skill.CD;
			this.CDText.text = ((int)Math.Ceiling(this.cdRemain)).ToString();
			this.cdRemain -= Time.deltaTime;
		}
		else
		{
			if (CDOverlay.enabled)
			{
				CDOverlay.enabled = false;
			}
			if (this.CDText.enabled)
			{
				this.CDText.enabled = false;
			}
		}
	}

    internal void SetSkill(SkillDefine value)
    {
		this.skill = value;
		if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.skill.Icon);
		this.SetCD(this.skill.CD);
    }


	public void SetCD(float cd)
	{
		if(!CDOverlay.enabled) CDOverlay.enabled = true;
		if(!this.CDText.enabled) this.CDText.enabled = true;

		this.CDText.text = ((int)Math.Floor(this.cdRemain)).ToString();
		CDOverlay.fillAmount = 1f;
		overlaySpeed = 1 / cd;
		cdRemain = cd;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.CDOverlay.fillAmount > 0)
        {
            MessageBox.Show("正在冷却");
        }
        else
        {
            MessageBox.Show("释放技能" + this.skill.Name);
            this.SetCD(this.skill.CD);
        }
    }
}
