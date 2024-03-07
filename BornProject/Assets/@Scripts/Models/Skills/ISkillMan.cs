using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillMan {

    public SkillList SkillList { get; }
    public string SkillSetList { get; }
    public Status Status { get; }
    public SkillStatus DefaultStatus { get; }
    
}