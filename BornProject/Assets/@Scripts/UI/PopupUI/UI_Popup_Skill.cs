using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup_Skill : UI_Popup {

    #region Enum

    enum Buttons {
        btnClose,
    }
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
        BindButton(typeof(Buttons));

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

        GetButton((int)Buttons.btnClose).onClick.AddListener(OnBtnClose);

        return true;
    }

    public void SetInfo() {
        SkillData range = Main.Skill.BaseRange;
        if (range != null) {
            UI_SkillSlot rangeSlot = RangeSlots[range];
            if (rangeSlot != null) rangeSlot.Activate();
        }
        SkillData melee = Main.Skill.BaseMelee;
        if (melee != null) {
            UI_SkillSlot meleeSlot = MeleeSlots[melee];
            if (meleeSlot != null) meleeSlot.Activate();
        }
    }

    public void SelectSkill(SkillData skill) {
        Initialize();

        MainSlots.gameObject.SetActive(true);
        MainSlots.SetInfoSkillTree(this, skill);
    }

    #endregion

    #region OnButtons

    private void OnBtnClose() {
        if (Main.Skill.BaseRange == null || Main.Skill.BaseMelee == null) {
            Main.UI.ShowToast("기본 스킬을 찍어주세요.");
            return;
        }
        Close();
    }

    #endregion

}