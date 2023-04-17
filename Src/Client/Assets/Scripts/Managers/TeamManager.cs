using Models;
using SkillBridge.Message;

namespace Assets.Scripts.Managers
{
    internal class TeamManager : Singleton<TeamManager>
    {
        public void Init()
        {

        }
        internal void UpdateTeamInfo(NTeamInfo team)
        {
            User.Instance.TeamInfo = team;
            ShowTeamUI(team != null);
        }

        private void ShowTeamUI(bool v)
        {
            if (UIMain.Instance != null)
            {
                UIMain.Instance.ShowTeamUI(v);
            }
        }
    }
}
