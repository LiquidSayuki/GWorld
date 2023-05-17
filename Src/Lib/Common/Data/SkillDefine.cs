using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;

namespace Common.Data
{
    public enum SkillType
    {
        Normal = 0,
        Skill = 1,
    }
    public enum TargetType
    {
        None = 0,
        Target = 1,
        Position = 2,
    }
    public class SkillDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public SkillType Type { get; set; }
        public TargetType CastTarget { get; set; }
        public int UnlockLevel { get; set; }
        public float CasrRange { get; set; }
        public float CastTime { get; set; }
        public float CD { get; set; }
        public float MPCost { get; set; }
        public bool Bullet { get; set; }
        public float BulletSpeed { get; set; }
        public string BulletResource { get; set; }
        public float AOERange { get; set; }
        public float Duration { get; set; }
        public float Interval { get; set; }
        public List<int> Buff { get; set; }
        public float AD { get; set; }
        public float AP { get; set; }
        public float ADFactor { get; set; }
        public float APFactor { get; set; }
    }
}
