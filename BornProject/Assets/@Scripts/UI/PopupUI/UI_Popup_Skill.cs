using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup_Skill : UI_Popup {

    #region Enum
    
    enum Objects {
        RangeSlots,
        MeleeSlots,
        MainSlots,
    }

    #endregion

    #region Properties

    public UI_SkillSlot RangeSlots { get; protected set; }
    public UI_SkillSlot MeleeSlots { get; protected set; }
    public UI_SkillSlot MainSlots { get; protected set; }

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        BindObject(typeof(Objects));

        GameObject rangeSlots = GetObject((int)Objects.RangeSlots);
        Destroy(rangeSlots.transform.Find("Sample").gameObject);
        RangeSlots = rangeSlots.GetComponent<UI_SkillSlot>();
        RangeSlots.SetInfo(this);

        GameObject meleeSlots = GetObject((int)Objects.MeleeSlots);
        Destroy(meleeSlots.transform.Find("Sample").gameObject);
        MeleeSlots = meleeSlots.GetComponent<UI_SkillSlot>();
        MeleeSlots.SetInfo(this);

        GameObject mainSlots = GetObject((int)Objects.MainSlots);
        mainSlots.SetActive(false);
        MainSlots = mainSlots.GetComponent<UI_SkillSlot>();

        return true;
    }

    public void SelectSkill(SkillData skill) {
        MainSlots.gameObject.SetActive(true);
        MainSlots.SetInfoSkillTree(this, skill);
    }

    #endregion

}