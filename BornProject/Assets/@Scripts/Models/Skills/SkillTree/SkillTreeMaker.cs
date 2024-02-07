using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class SkillTreeMaker : MonoBehaviour
{
    #region Properties

    public int SkillLineNumber {  get; private set; }
    public int SkillCountInLine { get; private set; }


    #endregion

    #region Method

    private void DrawSkillTreeSlot()
    {
        // 조건에 해당되는 SkillSlot을 그린다.
        // (class) HeadSkillSlot, SoloSkillSlot, SubSoloSkillSlot, SubMultiSkillSlot.
    }

    private void DrawSkillTreeLine()
    {
        // 하위 스킬칸들이 2개 이상, 같은 행에 존재할 경우 Line을 갯수에 맞게 그려준다.
    }


    #endregion
}
