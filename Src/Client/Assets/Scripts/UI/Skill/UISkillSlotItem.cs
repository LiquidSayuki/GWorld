using Assets.Scripts.Managers;
using Battle;
using Common.Battle;
using SkillBridge.Message;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkillSlotItem : MonoBehaviour,IPointerClickHandler {

	public Image icon;
	public Image CDOverlay;
	public Text CDText;
	Skill skill;

	private float overlaySpeed = 0;
	private float cdRemain = 0;

	void Start()
	{
		CDOverlay.enabled = false;
		CDText.enabled = false;
	}
	void Update () 
	{
		if(this.skill.CD > 0)
		{
			if(!CDOverlay.enabled) CDOverlay.enabled = true;
			if(!CDText.enabled) CDText.enabled = true;

			CDOverlay.fillAmount = this.skill.CD / this.skill.Define.CD;
			this.CDText.text = ((int)Math.Ceiling(this.skill.CD)).ToString();
		}
		else
		{
			if (CDOverlay.enabled) CDOverlay.enabled = false;
			if (this.CDText.enabled) this.CDText.enabled = false;
		}
	}

    internal void SetSkill(Skill value)
    {
		this.skill = value;
		if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.skill.Define.Icon);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
		SkillResult result= this.skill.CanCast(BattleManager.Instance.CurrentTarget);

		switch (result)
		{
			case SkillResult.Ok:
                MessageBox.Show("释放技能" + this.skill.Define.Name);
                BattleManager.Instance.CastSkill(this.skill);
                break;
			case SkillResult.InvalidTarget:
                MessageBox.Show("无效目标");
                break;
			case SkillResult.OutOfMp:
                MessageBox.Show("魔法不足");
                break;
			case SkillResult.Cooldown:
                MessageBox.Show("冷却中");
                break;
        }
    }
}
