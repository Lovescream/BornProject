using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeTemp : MonoBehaviour
{
    public static SkillTreeTemp skillTree;
    private void Awake() => skillTree = this;

    public int[] SkillLevels;
    public int[] SkillCaps;
    public string[] SkillNames;
    public string[] SkillDescriptions;

    public List<SkillTemp> SkillList;
    public GameObject SkillHolder;

    public List<GameObject> ConnectorList;
    public GameObject ConnectorHolder;


    public int SkillPoint;

    private void Start()
    {
        SkillPoint = 3;

        SkillLevels = new int[8];
        SkillCaps = new[] { 1, 1, 1, 1, 1, 1, 1, 1 };

        SkillNames = new[] { "1", "2", "3", "4", "5", "6", "7" };

        foreach (var skill in SkillHolder.GetComponentsInChildren<SkillTemp>()) SkillList.Add(skill);
        foreach (var connector in ConnectorHolder.GetComponentsInChildren<RectTransform>()) ConnectorList.Add(connector.gameObject);

        for (var i = 0; i < SkillList.Count; i++) SkillList[i].id = i;

        SkillList[0].ConnectedSkills = new[] { 1, 2, 3 };
        SkillList[1].ConnectedSkills = new[] { 4 };
        SkillList[2].ConnectedSkills = new[] { 5 };
        SkillList[3].ConnectedSkills = new[] { 6 };

        UpdateAllSkillUI();
    }

    public void UpdateAllSkillUI()
    {
        foreach (var skill in SkillList) skill.UpdateUI();
    }
}
