using Common.Battle;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UISkill : UIWindow {

	public Text Description;
	public GameObject ItemPrefab;
	public ListView ListMain;
	public UISkillItem SelectedItem;

	void Start () {
		RefreshUI();
		this.ListMain.onItemSelected += this.OnItemSelected;
	}

    public void OnItemSelected(ListView.ListViewItem item)
	{
		this.SelectedItem = item as UISkillItem;
        this.Description.text = SelectedItem.item.Define.Description;
	}

    private void RefreshUI()
    {
        ClearItems();
        InitItems();
    }

    private void ClearItems()
    {
        this.ListMain.RemoveAll();
    }

    private void InitItems()
    {
        var Skills = User.Instance.CurrentCharacter.SkillManager.Skills;
        foreach(var skill in Skills)
        {
            if(skill.Define.Type == SkillType.Skill)
            {
                GameObject go = Instantiate(ItemPrefab, this.ListMain.transform);
                UISkillItem ui = go.GetComponent<UISkillItem>();
                ui.SetItem(skill, this, false);
                this.ListMain.AddItem(ui);
            }
        }
    }
}
