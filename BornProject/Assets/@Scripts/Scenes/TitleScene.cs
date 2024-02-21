using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene {
    public UI_Scene_Title UI => SceneUI as UI_Scene_Title;

    protected override bool Initialize()
    {
        if (!base.Initialize()) return false;

        SceneUI = Main.UI.OpenSceneUI<UI_Scene_Title>();
       

        return true;
    }
}
