using Services;
using UnityEngine.UI;

internal class UIGuildPopCreate:UIWindow
{
    public InputField inputName;
    public InputField inputNotice;

    void Start()
    {
        GuildService.Instance.OnGuildCreateResult = OnGuildCreated;
    }

    void OnDestroy()
    {
        GuildService.Instance.OnGuildCreateResult = null;
    }

    public override void OnYesClick()
    {
        if (string.IsNullOrEmpty(inputName.text))
        {
            MessageBox.Show("请输入公会名称","错误", MessageBoxType.Error);
            return;
        }
        if(inputName.text.Length < 4 || inputName.text.Length > 10)
        {
            MessageBox.Show("公会名称应为4-10个字符", "错误", MessageBoxType.Error);
            return;
        }

        if (string.IsNullOrEmpty(inputNotice.text))
        {
            MessageBox.Show("请输入公会宣言", "错误", MessageBoxType.Error);
            return;
        }
        if (inputNotice.text.Length < 3 || inputNotice.text.Length > 50)
        {
            MessageBox.Show("工会宣言需要在3-50个字符之间", "错误", MessageBoxType.Error);
            return;
        }

        GuildService.Instance.SendGuildCreate(inputName.text, inputNotice.text);
    }

    void OnGuildCreated(bool result)
    {
        if (result)
        {
            this.Close(WindowResult.Yes);
        }
    }
}

