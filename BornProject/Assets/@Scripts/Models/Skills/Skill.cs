using System.Collections.Generic;

public class Skill {

    #region Properties

    public SkillData Data { get; protected set; }
    public Skill Parent { get; set; }                   // 이 스킬의 상위 스킬.
    public SkillSet SkillSet { get; set; }              // 이 스킬이 속한 스킬트리.

    public string BaseKey { get; protected set; }       // Ex) Laser_Normal_RangeUp 에서 "Laser"
    public string AdvancedKey { get; protected set; }   // Ex) Laser_Normal_RangeUp 에서 "RangeUp"
    public SkillType Type => Data.Type;
    public SkillLevel Level => Data.Level;

    #endregion

    #region Constructor

    public Skill(SkillData data) {
        this.Data = data;

        string[] s = Data.Key.Split('_');
        BaseKey = s[0];
        AdvancedKey = s[2];
    }

    #endregion

    public List<Skill> GetSubs() => SkillSet.GetSubs(this);

}