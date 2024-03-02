using ZerolizeDungeon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager {

    public DungeonGenerator Generator { get; private set; } = new(20, 20, 30, 30, 40);
    public UI_Map MapUI { get; private set; }

    public Dungeon Current { get; private set; }

    public Transform DungeonRoot {
        get {
            if (_dungeonRoot == null) {
                GameObject obj = GameObject.Find("Dungeon");
                if (obj == null) obj = new("Dungeon");
                _dungeonRoot = obj.transform;
            }
            return _dungeonRoot;
        }
    }

    private Transform _dungeonRoot;

    public void Generate() {
        if (Current != null) Destroy();
        Generator.Clear();
        if (Generator.Generate())
            Current = new Dungeon(Generator.Result, DungeonRoot);
    }

    public void ToggleMap() {
        if (MapUI == null) {
            MapUI = Main.Resource.Instantiate("UI_Map").GetComponent<UI_Map>();
            MapUI.SetInfo();
            return;
        }
        MapUI.gameObject.SetActive(!MapUI.gameObject.activeSelf);
    }

    public void Destroy() {
        for (int i = DungeonRoot.childCount; i > 0; i--) {
            Main.Resource.Destroy(DungeonRoot.transform.GetChild(i).gameObject);
        }
        Current = null;
    }

    public Room GetRoom(Vector2 position) {
        return Current.Rooms.Where(room => room.IsInRoom(position)).FirstOrDefault();
    }

    public void CheckClear() {
        if (GameObject.FindObjectOfType<GameScene>().IsPlaying == false) return;
        if (Current.Rooms.Where(x => x.ExistEnemy).Count() > 0) return;
        if (GameObject.FindObjectOfType<EnemySpawner>() != null) return;
        Main.Quest.ClearStageCount++;
        Main.UI.OpenPopupUI<UI_Popup_Clear>();
    }

    public void NextStage() {
        Main.UI.Clear();
        SceneManager.LoadScene("GameScene");
    }
}