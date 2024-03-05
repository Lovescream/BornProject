using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScene : BaseScene {

    public UI_Scene_Victory UI => SceneUI as UI_Scene_Victory;

    protected override bool Initialize() {

        if (!base.Initialize()) return false;

        SceneUI = Main.UI.OpenSceneUI<UI_Scene_Victory>();

        return true;
    }
}