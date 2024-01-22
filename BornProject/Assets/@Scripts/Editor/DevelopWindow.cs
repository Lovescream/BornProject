using DungeonGenerate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevelopWindow : EditorWindow {

#if UNITY_EDITOR

    #region Properties

    public int Indent { get; private set; } = 0;
    public bool IsInitialized { get; private set; }
    public bool IsExpandedRoomList { get; private set; }

    public string PlayerKey { get; private set; } = "Player";
    public Vector2 PlayerSpawnPosition { get; private set; } = Vector2.zero;
    public int PlayerSpawnRoomIndex { get; private set; }

    public string EnemyKey { get; private set; } = "Enemy1";
    public int EnemySpawnRoomIndex { get; private set; }

    #endregion

    #region Layouts

    private GUILayoutOption[] _buttonLayout = new[] {
            GUILayout.Width(100),
            GUILayout.Height(EditorGUIUtility.singleLineHeight),
    };
    private GUILayoutOption[] _longButtonLayout = new[] {
        GUILayout.Width(200),
        GUILayout.Height(EditorGUIUtility.singleLineHeight),
    };
    private GUILayoutOption[] _labelLayout = new[] {
        GUILayout.Width(200),
        GUILayout.Height(EditorGUIUtility.singleLineHeight),
    };
    private GUILayoutOption[] _spaceLayout = new[] {
        GUILayout.Width(10),
        GUILayout.Height(EditorGUIUtility.singleLineHeight),
    };

    #endregion

    [MenuItem("Tools/Open Develop Window")]
    public static void ShowWindow() {
        DevelopWindow window = GetWindow<DevelopWindow>();
        window.titleContent = new("Develop Tools");
        window.position = new Rect(1000, 200, 500, 800);
        window.maxSize = new Vector2(500, 800);
        window.Show();
    }
    private void OnGUI() {
        GUIStyle titleLabelStyle = new(EditorStyles.boldLabel) {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16,
        };

        GUILayout.Label("데이터 경로", titleLabelStyle);
        ShowDataPathInfo();

        if (SceneManager.GetActiveScene().name != "TestScene") return;

        GUILayout.Space(20);
        GUILayout.Label("게임 초기화", titleLabelStyle);
        ShowGameInitButtons();

        GUILayout.Space(20);
        GUILayout.Label("게임 초기화 상태", titleLabelStyle);
        ShowGameInitState();

        if (!IsInitialized) return;

        GUILayout.Space(20);
        GUILayout.Label("던전 생성 정보", titleLabelStyle);
        ShowDungeonGenerateInfo();

        if (Main.Dungeon.Current != null) {
            GUILayout.Space(20);
            GUILayout.Label("현재 던전 정보", titleLabelStyle);
            ShowDungeonState();
        }

        if (Application.isPlaying == false) return;

        GUILayout.Space(20);
        GUILayout.Label("플레이어 정보", titleLabelStyle);
        ShowPlayerInfo();

        GUILayout.Space(20);
        GUILayout.Label("적 생성", titleLabelStyle);
        ShowEnemyGenerateInfo();
    }

    #region ShowArea

    private void ShowDataPathInfo() {
        DataTransformer.csvDataPath = EditorGUILayout.TextField("CSV Path", DataTransformer.csvDataPath);
        DataTransformer.jsonDataPath = EditorGUILayout.TextField("JSON Path", DataTransformer.jsonDataPath);
        DefaultButton("Parse All Data", DataTransformer.ParseExcel);
    }

    private void ShowGameInitButtons() {
        GUILayout.BeginHorizontal();

        DeactivatableButton("도메인 리로드", EditorUtility.RequestScriptReload, !EditorApplication.isCompiling);
        GUILayout.FlexibleSpace();
        DeactivatableButton("매니저 초기화", () => {
            Main.Instance.ManualInitialize();
            Main.Resource.Initialize();
            Main.Data.Initialize();
        }, Main.Instance == null || Main.Resource == null || !Main.Resource.IsInitialized);
        GUILayout.FlexibleSpace();
        DeactivatableButton("씬 리로드", () => {
            Scene scene = SceneManager.GetActiveScene();
            if (scene == null) return;
            EditorSceneManager.LoadSceneInPlayMode(scene.path, new LoadSceneParameters { });
        }, EditorApplication.isPlaying);

        GUILayout.EndHorizontal();
    }

    private void ShowGameInitState() {
        bool instantiatedMain = GameObject.Find("@Main");
        bool isInitialized = instantiatedMain && Main.Resource != null;
        bool isInitResource = isInitialized && Main.Resource.IsInitialized;
        bool isInitData = isInitialized && Main.Data.IsInitialized;

        if (isInitResource && isInitData) {
            LabelToggle("게임 초기 설정 완료", true);
            IsInitialized = true;
            return;
        }
        else IsInitialized = false;

        LabelToggle("Main 생성 여부", instantiatedMain);
        Indent++;

        if (instantiatedMain) {
            LabelToggle("Instance 생성 여부", Main.IsInitialized);
            LabelToggle("초기화 여부", isInitialized);
            Indent++;

            if (isInitialized) {
                LabelToggle("Resource 초기화 여부", isInitResource);
                Indent++;

                if (!isInitResource) Button("Resource 초기화", Main.Resource.Initialize);
                Indent--;

                LabelToggle("Data 초기화 여부", isInitData);
                Indent++;

                if (!isInitData) Button("Data 초기화", Main.Data.Initialize);
                Indent--;

            }
            else Button("Main 초기화", Main.Instance.ManualInitialize);
            Indent--;
        }
        Indent--;

        Indent = 0;
    }

    private void ShowDungeonGenerateInfo() {
        Main.Dungeon.DungeonWidth = EditorGUILayout.IntField("던전 최대 수평 길이", Main.Dungeon.DungeonWidth);
        Main.Dungeon.DungeonHeight = EditorGUILayout.IntField("던전 최대 수직 길이", Main.Dungeon.DungeonHeight);
        Main.Dungeon.Count = EditorGUILayout.IntField("던전 방 생성 개수", Main.Dungeon.Count);

        if (Main.Dungeon.Current == null) {
            if (Main.Dungeon.DungeonRoot.childCount > 0) {
                LongButton("던전 초기화", Main.Dungeon.DungeonRoot.gameObject.DestroyChilds);
            }
            else {
                LongButton("던전 생성", Main.Dungeon.Generate);
            }
        }
        else {
            LongButton("던전 삭제", Main.Dungeon.Destroy);
        }

    }

    private void ShowDungeonState() {
        Dungeon dungeon = Main.Dungeon.Current;
        if (dungeon == null) return;

        IsExpandedRoomList = EditorGUILayout.Foldout(IsExpandedRoomList, "방 목록", true);
        if (IsExpandedRoomList) {
            List<Room> rooms = dungeon.Rooms;
            foreach (Room room in rooms) {
                RoomInfo(room);
            }
        }
    }

    private void ShowPlayerInfo() {
        if (Main.Object.Player == null) {
            PlayerKey = EditorGUILayout.TextField("PlayerKey", PlayerKey);
            //PlayerSpawnPosition = EditorGUILayout.Vector2Field("SpawnPosition", PlayerSpawnPosition);
            if (Main.Dungeon.Current != null) {
                PlayerSpawnRoomIndex = EditorGUILayout.Popup("SpawnRoom", PlayerSpawnRoomIndex, Main.Dungeon.Current.Rooms.Select(x => x.ToString()).ToArray());

                LongButton("플레이어 생성", () => { Main.Object.SpawnPlayer(PlayerKey, Main.Dungeon.Current.Rooms[PlayerSpawnRoomIndex].CenterPosition); });
            }
        }
        else {

        }
    }

    private void ShowEnemyGenerateInfo() {
        EnemyKey = EditorGUILayout.TextField("EnemyKey", EnemyKey);
        if (Main.Dungeon.Current != null) {
            EnemySpawnRoomIndex = EditorGUILayout.Popup("SpawnRoom", EnemySpawnRoomIndex, Main.Dungeon.Current.Rooms.Select(x => x.ToString()).ToArray());
            LongButton("적 생성", () => { Main.Object.SpawnEnemy(EnemyKey, Main.Dungeon.Current.Rooms[EnemySpawnRoomIndex].GetRandomPosition()); });
        }
    }

    #endregion

    #region GUIElement

    private void Space() => GUILayout.Label("", _spaceLayout);
    private void Label(string label, bool isIndent = true) {
        GUILayout.BeginHorizontal();
        if (isIndent) {
            for (int i = 0; i < Indent; i++) Space();
        }
        GUILayout.Label(label, _labelLayout);
        GUILayout.EndHorizontal();
    }
    private void Toggle(bool value, string label = "", bool isIndent = false) {
        GUILayout.BeginHorizontal();
        if (isIndent) {
            for (int i = 0; i < Indent; i++) Space();
        }
        GUILayout.Toggle(value, label);
        GUILayout.EndHorizontal();
    }
    private void LabelToggle(string label, bool value, bool isIndent = true) {
        GUILayout.BeginHorizontal();
        Label(label, isIndent: isIndent);
        GUILayout.FlexibleSpace();
        Toggle(value, isIndent: isIndent);
        GUILayout.EndHorizontal();
    }
    private void Button(string label, Action action, bool isIndent = true) {
        GUILayout.BeginHorizontal();
        if (isIndent) {
            for (int i = 0; i < Indent; i++) Space();
        }
        if (GUILayout.Button(label, _buttonLayout)) action?.Invoke();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
    private void LongButton(string label, Action action) {
        if (GUILayout.Button(label, _longButtonLayout)) action?.Invoke();
    }
    private void DefaultButton(string label, Action action) {
        if (GUILayout.Button(label)) action?.Invoke();
    }
    private void DeactivatableButton(string label, Action action, bool isActivated) {
        EditorGUI.BeginDisabledGroup(!isActivated);
        if (GUILayout.Button(label, _buttonLayout)) action?.Invoke();
        EditorGUI.EndDisabledGroup();
    }

    #endregion

    #region RoomInfo

    private void RoomInfo(Room room) {
        EditorGUI.BeginDisabledGroup(true);
        GUILayout.BeginHorizontal();
        EditorGUILayout.ObjectField($"Room[{room.X}, {room.Y}] ({room.Type})", room.Object, typeof(GameObject), true);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();
    }

    #endregion


#endif

}