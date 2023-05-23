using Entities;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour
{

    public Text avaverName;
    public Image avaterImage;

    public Creature character;

    void Start()
    {
        if (this.character != null)
        {
            if (character.Info.Type == SkillBridge.Message.CharacterType.Monster)
                this.avaterImage.gameObject.SetActive(false);
            else
                this.avaterImage.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        this.UpdateInfo();
    }

    void UpdateInfo()
    {
        if (this.character != null)
        {
            string name = this.character.Name + " Lv." + this.character.Info.Level;
            // UI无需更新时不重绘，提高性能
            if (name != this.avaverName.text)
            {
                this.avaverName.text = name;
            }
        }
    }
}
