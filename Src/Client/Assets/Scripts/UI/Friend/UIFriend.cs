using Assets.Scripts.Managers;
using Models;
using Services;
using UnityEngine;

public class UIFriend : UIWindow
{

    public GameObject itemPrefab;
    public ListView ListMain;
    public Transform itemRoot;
    public UIFriendItem selectedItem;

    // Use this for initialization
    void Start()
    {
        FriendService.Instance.OnFriendUpdate = RefreshUI;
        this.ListMain.onItemSelected += this.OnFriendSelected;
        RefreshUI();
    }

    private void OnFriendSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIFriendItem;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickFriendAdd()
    {
        // UIInputBox 的 event OnSubmit 的形态 SubmitHandler(string inputText, out string tips);
        InputBox.Show("输入你要添加的好友的名称或ID", "添加好友").OnSubmit += OnFriendAddSubmit;
    }

    /// <summary>
    /// 当玩家在添加好友的输入框中输入内容后点击发送时，处理对应逻辑的函数
    /// </summary>
    /// <param name="input">玩家的输入</param>
    /// <param name="tips">事件观察者向事件发布者（输入框）反向传输的内容</param>
    /// <returns></returns>
    private bool OnFriendAddSubmit(string input, out string tips)
    {
        tips = "";
        int friendId = 0;
        string friendName = "";
        if (!int.TryParse(input, out friendId))
            friendName = input;
        if (friendId == User.Instance.CurrentCharacter.Id || friendName == User.Instance.CurrentCharacter.Name)
        {
            // 反向向输入框发送本地的校验信息
            tips = "不能添加自己为好友";
            return false;
        }

        FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;
    }

    public void OnClickFriendChat()
    {
        MessageBox.Show("暂未开放");
    }

    public void OnClickFriendRemove()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择你要删除的好友");
            return;
        }
        MessageBox.Show(string.Format("确定要删除好友[{0}]吗？", selectedItem.Info.friendInfo.Name), "删除好友", MessageBoxType.Confirm, "删除", "取消").OnYes = () =>
        {
            FriendService.Instance.SendFriendRemoveRequest(this.selectedItem.Info.Id, this.selectedItem.Info.friendInfo.Id);
        };
    }

    public void OnClickFriendTeamInvite()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("您还没有选择要邀请的好友");
            return;
        }
        if (selectedItem.Info.Status == 0)
        {
            MessageBox.Show("您的好友未在线");
            return;
        }
        MessageBox.Show(string.Format("确定要邀请[{0}]加入你的队伍吗?", selectedItem.Info.friendInfo.Name), "邀请好友组队", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            TeamService.Instance.SendTeamInviteRequest(this.selectedItem.Info.friendInfo.Id, this.selectedItem.Info.friendInfo.Name);
        };

    }


    private void RefreshUI()
    {
        ClearFriendList();
        InitFriendItems();
    }

    private void InitFriendItems()
    {
        foreach (var item in FriendManager.Instance.allFriends)
        {
            GameObject go = Instantiate(itemPrefab, this.ListMain.transform);
            UIFriendItem ui = go.GetComponent<UIFriendItem>();
            ui.SetFriendInfo(item);
            this.ListMain.AddItem(ui);
        }
    }

    private void ClearFriendList()
    {
        this.ListMain.RemoveAll();
    }
}
