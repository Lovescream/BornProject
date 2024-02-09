using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    #region Properties
    public int SkillPointCount { get; private set; }
    public int SkillPointAccumulatedCount { get; private set; }
    public int LastLearnSkill {  get; private set; }
    public int MouseOverDelayTime { get; private set; }

    // 스킬 목록.
    // 스킬 설명 목록.

    #endregion

    #region Method

    private void SkillActivate() // Bool 값으로 해야하는가.
    {
        // 스킬을 배울때 활성화 표시해주기.
        // 해당 스킬 플레이어에게 적용하기.
    }
    
    private void PopupOpenSkillExplanation() // Bool 값으로 해야하는가.
    {
        // 마우스오버하고 int MouseOverDelayTime 시간이 지나면 설명Open.
        // 마우스가 해당 스킬칸을 벗어나면 설명Close.
    }

    #endregion
}
