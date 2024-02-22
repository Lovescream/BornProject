using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager {

    public static readonly int RequiredClearCount = 3;

    public int ClearStageCount {
        get => _clearStageCount;
        set {
            _clearStageCount = value;
            if (_clearStageCount == RequiredClearCount) ClearQuest();
        }
    }

    private int _clearStageCount;

    public void ClearQuest() {
        
    }
}