using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
