using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx {

    public BaseScene Current { get; set; }

    public void LoadScene(string sceneName) {
        Main.Clear();
        SceneManager.LoadScene(sceneName);
        if (!Main.Audio.PlayBGM($"BGM_{sceneName}", "MainBGM"))
            Main.Audio.Stop(AudioType.BGM);
    }

}