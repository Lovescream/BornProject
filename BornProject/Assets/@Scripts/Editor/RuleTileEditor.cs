using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static Codice.Client.BaseCommands.Import.Commit;
using Object = UnityEngine.Object;

namespace ZerolizeDungeon {

#if UNITY_EDITOR
    [CustomEditor(typeof(Tile), true)]
    [CanEditMultipleObjects]
    public class RuleTileEditor : Editor {

        #region const

        public static readonly string IconBase64Path = $"{Application.dataPath}/Resources/RuleIcon.minecraftvirusohmygod";
        private const string s_MirrorX = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwQAADsEBuJFr7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAG1JREFUOE+lj9ENwCAIRB2IFdyRfRiuDSaXAF4MrR9P5eRhHGb2Gxp2oaEjIovTXSrAnPNx6hlgyCZ7o6omOdYOldGIZhAziEmOTSfigLV0RYAB9y9f/7kO8L3WUaQyhCgz0dmCL9CwCw172HgBeyG6oloC8fAAAAAASUVORK5CYII=";
        private const string s_MirrorY = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAG9JREFUOE+djckNACEMAykoLdAjHbPyw1IOJ0L7mAejjFlm9hspyd77Kk+kBAjPOXcakJIh6QaKyOE0EB5dSPJAiUmOiL8PMVGxugsP/0OOib8vsY8yYwy6gRyC8CB5QIWgCMKBLgRSkikEUr5h6wOPWfMoCYILdgAAAABJRU5ErkJggg==";
        private const string s_MirrorXY = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABl0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC4yMfEgaZUAAAHkSURBVDhPrVJLSwJRFJ4cdXwjPlrVJly1kB62cpEguElXKgYKIpaC+EIEEfGxLqI/UES1KaJlEdGmRY9ltCsIWrUJatGm0eZO3xkHIsJdH3zce+ec75z5zr3cf2MMmLdYLA/BYFA2mUyPOPvwnR+GR4PXaDQLLpfrKpVKSb1eT6bV6XTeocAS4sIw7S804BzEZ4IgsGq1ykhcr9dlj8czwPdbxJdBMyX/As/zLiz74Ar2J9lsVulcKpUYut5DnEbsHFwEx8AhtFqtGViD6BOc1ul0B5lMRhGXy2Wm1+ufkBOE/2fsL1FsQpXCiCAcQiAlk0kJRZjf7+9TRxI3Gg0WCoW+IpGISHHERBS5UKUch8n2K5WK3O125VqtpqydTkdZie12W261WjIVo73b7RZVKccZDIZ1q9XaT6fTLB6PD9BFKhQKjITFYpGFw+FBNBpVOgcCARH516pUGZYZXk5R4B3efLBxDM9f1CkWi/WR3ICtGVh6Rd4NPE+p0iEgmkSRLRoMEjYhHpA4kUiIOO8iZRU8AmnadK2/QOOfhnjPZrO95fN5Zdq5XE5yOBwvuKoNxGfBkQ8FzXkPprnj9Xrfm82mDI8fsLON3x5H/Od+RwHdLfDds9vtn0aj8QoF6QH9JzjuG3acpxmu1RgPAAAAAElFTkSuQmCC";
        private const string s_Rotated = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwQAADsEBuJFr7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAHdJREFUOE+djssNwCAMQxmIFdgx+2S4Vj4YxWlQgcOT8nuG5u5C732Sd3lfLlmPMR4QhXgrTQaimUlA3EtD+CJlBuQ7aUAUMjEAv9gWCQNEPhHJUkYfZ1kEpcxDzioRzGIlr0Qwi0r+Q5rTgM+AAVcygHgt7+HtBZs/2QVWP8ahAAAAAElFTkSuQmCC";
        private const string s_RotatedMirror = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAApklEQVQoFY2SMRaAIAxDwefknVg8uIt3ckVSKdYSnjBAi/lprcaUUphZ+3WGY3u1yJcJMBdNtqAyM3BAFRgohBNmUzDEzIDCVQgGK2rL1gAxhatY3vXh+U7hIs2uOqUZ7EGfN6O1RU/wEf5VX4zgAzpTSessIhL5VDrJkrepitJtFtRHvm0YtA6MMfRSUUGcbGC+A0AdOIJx7w1w1y1WWX/FYUV1uQFvVjvOTYh+rAAAAABJRU5ErkJggg==";
        private const string s_Fixed = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMjHxIGmVAAAA50lEQVQ4T51Ruw6CQBCkwBYKWkIgQAs9gfgCvgb4BML/qWBM9Bdo9QPIuVOQ3JIzosVkc7Mzty9NCPE3lORaKMm1YA/LsnTXdbdhGJ6iKHoVRTEi+r4/OI6zN01Tl/XM7HneLsuyW13XU9u2ous6gYh3kiR327YPsp6ZgyDom6aZYFqiqqqJ8mdZz8xoca64BHjkZT0zY0aVcQbysp6Z4zj+Vvkp65mZttxjOSozdkEzD7KemekcxzRNHxDOHSDiQ/DIy3pmpjtuSJBThStGKMtyRKSOLnSm3DCMz3f+FUpyLZTkOgjtDSWORSDbpbmNAAAAAElFTkSuQmCC";
        private const string s_InspectorPopupBackground = "iVBORw0KGgoAAAANSUhEUgAAAPoAAAAUCAYAAACkoiDPAAAAAXNSR0IArs4c6QAAAMNJREFUeJzt3LERAVEQgOF9RgMqoAGJQgRKUIMy1KAEgUIk1wAVKIGAMwLOBczN3X5feJts8s+bS7bEw/awvwYwKJvlqkRElIh75JP5tNuNgJ+7VOfYLFeliByG7VKdY9T1EsD/CR0SEDokIHRIQOiQwLjrBYB21rNF43x3On6cedGhJ5pCbppFCB165V3Q3yKPEDr0zmvYbSKP8I8OvdQ28JoXHRIQOiQgdEhA6JCA0CEBhydgwJ6HJ+oPTknB8NSnpG6z5i8nfo7P8QAAAABJRU5ErkJggg==";
        private const string s_InspectorPopupBackground_Focused = "iVBORw0KGgoAAAANSUhEUgAAAPoAAAAUCAYAAACkoiDPAAAAAXNSR0IArs4c6QAAAMJJREFUeJzt3KENwlAQgOF7BEPCEOhKNkHhGYFBGAGPYhMkmiFIkCCgBAGlAtK0932yZ878eam5Eg+7zfEawKAs11WJiCgR98jPi0u3GwE/N91PYrmuShE5DNt0P4lR10sA/yd0SEDokIDQIQGhQwLjrhcA2lnN5o3z7enwceZFh55oCrlpFiF06JV3QX+LPELo0DuvYbeJPMI/OvRS28BrXnRIQOiQgNAhAaFDAkKHBByegAF7Hp6oPzglBcNTn5K6AWgKL0hKD4OQAAAAAElFTkSuQmCC";
        private const string s_InspectorFieldBackground = "iVBORw0KGgoAAAANSUhEUgAAAPoAAAAUCAYAAACkoiDPAAAAAXNSR0IArs4c6QAAAJRJREFUeJzt3DERwlAQRdH9OEABCrCSIloiI1oosBIFURAJUEBQECYz/51TbrXNbV+rr/n5eBXQlWkYW1VVq/pEfr3fzv0IONy2rDUNY2sih75ty1qXs58A/k/oEEDoEEDoEEDoEEDoEEDoEEDoEEDoEEDoEEDoEEDoEEDoEEDoEEDoEMDwBHTsNzyxH0xJQX/2Kak3zs8cD+bHR18AAAAASUVORK5CYII=";
        private const string s_InspectorFieldBackground_Focused = "iVBORw0KGgoAAAANSUhEUgAAAPoAAAAUCAYAAACkoiDPAAAAAXNSR0IArs4c6QAAAJNJREFUeJzt3LENwlAQRMH7ZJZcibshch8uxH0QuRsqQSKEAOwKQJb+zoQXXfLSbfV1W++vAroyL1OrqmpVn8gf1+e5HwE/N25DzcvUmsihb+M21OXsJ4D/EzoEEDoEEDoEEDoEEDoEEDoEEDoEEDoEEDoEEDoEEDoEEDoEEDoEEDoEMDwBHTuGJ/aDKSnozz4l9QaC8xwwPJZJHgAAAABJRU5ErkJggg==";

        private static readonly string _undoName = L10n.Tr("Change RuleTile");

        private static readonly Color InspectorEdgeColor = (Color)new Color32(140, 178, 174, 255);
        private static readonly Color InspectorBoxColor = (Color)new Color32(231, 249, 231, 255);
        private static readonly Color InspectorInnerBoxColor = (Color)new Color32(182, 232, 214, 255);
        private static readonly Color InspectorTextColor = (Color)new Color32(132, 81, 57, 255);
        private static readonly Color InspectorHoverTextColor = (Color)new Color32(213, 191, 249, 255);
        private static readonly Color InspectorFocusedTextColor = (Color)new Color32(169, 142, 214, 255);

        #endregion

        #region Styles

        private static class Styles {
            public static readonly GUIContent defaultSprite = EditorGUIUtility.TrTextContent("Default Sprite", "새 Rule 작성 시 설정되는 기본 Sprite.");
            public static readonly GUIContent defaultGameObject = EditorGUIUtility.TrTextContent("Default GameObject", "새 Rule 작성 시 설정되는 기본 GameObject.");
            public static readonly GUIContent defaultCollider = EditorGUIUtility.TrTextContent("Default Collider"
                , "새 Rule 작성 시 설정되는 기본 Collider 타입.");

            public static readonly GUIContent emptyRuleTileInfo =
                EditorGUIUtility.TrTextContent(
                    "Sprite 또는 Sprite Texture 에셋을 드래그하여 Rule Tile을 생성.");

            public static readonly GUIContent extendNeighbor = EditorGUIUtility.TrTextContent("확장"
                , "활성화 시, 이웃 타일의 범위를 3x3 이상으로 늘릴 수 있음.");

            public static readonly GUIContent numberOfTilingRules = EditorGUIUtility.TrTextContent(
                "Number of Tiling Rules"
                , "규칙 수를 조정하려면 이 값을 변경.");

            public static readonly GUIContent tilingRules = EditorGUIUtility.TrTextContent("규칙 목록");
            public static readonly GUIContent tilingRulesGameObject = EditorGUIUtility.TrTextContent("GameObject"
                , "이 Rule의 Tile의 GameObject.");
            public static readonly GUIContent tilingRulesCollider = EditorGUIUtility.TrTextContent("Collider"
                , "이 Rule의 Tile의 Collider 타입.");
            public static readonly GUIContent tilingRulesOutput = EditorGUIUtility.TrTextContent("Output"
                , "이 Rule의 출력.");

            public static readonly GUIContent tilingRulesNoise = EditorGUIUtility.TrTextContent("Noise"
                , "Tile 배치 시 Perlin noise 계수.");
            public static readonly GUIContent tilingRulesShuffle = EditorGUIUtility.TrTextContent("Shuffle"
                , "Tile 배치 시 부여되는 무작위 변형.");
            public static readonly GUIContent tilingRulesRandomSize = EditorGUIUtility.TrTextContent("Size"
                , "무작위 추출할 Sprite 수.");

            public static readonly GUIContent tilingRulesMinSpeed = EditorGUIUtility.TrTextContent("Min Speed"
                , "애니메이션 재생 최소 속도.");
            public static readonly GUIContent tilingRulesMaxSpeed = EditorGUIUtility.TrTextContent("Max Speed"
                , "애니메이션 재생 최대 속도.");
            public static readonly GUIContent tilingRulesAnimationSize = EditorGUIUtility.TrTextContent("Size"
                , "애니메이션에 포함된 스프라이트 수.");

            public static readonly GUIStyle extendNeighborsStyle = new() {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                fontSize = 10,
                normal = new GUIStyleState() {
                    textColor = InspectorTextColor
                }
            };


            public static readonly GUIContent addNewRule = new("+", "새 규칙 추가");
            public static readonly GUIContent removeNewRule = new("-", "규칙 삭제");
            public static readonly GUIStyle HeaderStyle = new() {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = new() { textColor = InspectorTextColor }
            };
            public static readonly GUIStyle BoldStyle = new() {
                fontStyle = FontStyle.Bold,
                normal = new() { textColor = InspectorTextColor }
            };
            public static readonly GUIStyle LabelStyle = new() {
                normal = new() { textColor = InspectorTextColor }
            };
            public static readonly GUIStyle TitleStyle = new() {
                fontStyle = FontStyle.Bold,
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
                normal = new() { textColor = InspectorTextColor }
            };
        }

        #endregion

        #region Static

        public static List<List<Texture2D>> Icons {
            get {
                if (_icons == null) Initialize();
                return _icons;
            }
        }
        private static List<List<Texture2D>> _icons;

        private static bool _isInitialized;

        #endregion

        #region Properties

        public RuleTile Tile => target as RuleTile;

        public FieldInfo[] RuleTypes { get; protected set; }

        private bool DragAndDropActive => _dragAndDropSprites != null && _dragAndDropSprites.Count > 0;

        #endregion

        #region Fields

        // Constants.
        public const float DefaultElementHeight = 48f;  // Default height for a Rule Element.
        public const float PaddingBetweenRules = 24f;    // Padding between Rule Elements.
        public const float SingleLineHeight = 18f;      // Single line Height.
        public const float LabelWidth = 80f;            // Width for labels.

        private bool _extendNeighbour;    // Whether the RuleTile can extend its neighbours beyond directly adjacent ones.
        private PreviewRenderUtility _previewUtility;   // Priview Utility for rendering previews.
        private Grid _previewGrid;  // Grid for rendering previews.
        private SerializedProperty _tilingRules;
        private MethodInfo _clearCacheMethod;

        // Collections.
        private List<Tilemap> _previewTilemaps;
        private List<TilemapRenderer> _previewTilemapRenderers;
        private List<Sprite> _dragAndDropSprites;    // List of Sprites for Drag and Drop.
        private ReorderableList _reorderableList;   // Reorderablelist for Rules.

        #endregion

        #region Initialize
        private static void Initialize() {
            if (_isInitialized) return;
            _isInitialized = true;

            if (!File.Exists(IconBase64Path))
                Debug.LogError($"[RuleTileEditor] Initialize(): Failed to read RuleIcon Base64 data file.");
            string[] lines = File.ReadAllText(IconBase64Path).Split("\n");
            if (lines.Length <= 0)
                Debug.LogError($"[RuleTileEditor] Initialize(): Failed to read RuleIcon Base64 data file. Is the file empty?");
            _icons = new() { new(), };
            int ruleIndex = 0;
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                if (line.StartsWith("Rule"))
                    int.TryParse(line.Replace("Rule", "").Replace(".png", "").Split('_')[0], out ruleIndex);
                else {
                    if (_icons.Count <= ruleIndex) _icons.Add(new());
                    _icons[ruleIndex].Add(Base64ToTexture(line));
                }
            }

            _icons[0].Add(Base64ToTexture(s_Rotated));
            _icons[0].Add(Base64ToTexture(s_MirrorX));
            _icons[0].Add(Base64ToTexture(s_MirrorY));
            _icons[0].Add(Base64ToTexture(s_Fixed));
            _icons[0].Add(Base64ToTexture(s_MirrorXY));
            _icons[0].Add(Base64ToTexture(s_RotatedMirror));
        }

        #endregion

        #region Editor

        public virtual void OnEnable() {
            _reorderableList = new(Tile != null ? Tile.m_TilingRules : null,
                typeof(RuleTile.TilingRule),
                draggable: true,
                displayHeader: true,
                displayAddButton: true,
                displayRemoveButton: true) {
                drawHeaderCallback = OnDrawHeader,
                drawFooterCallback = OnDrawFooter,
                drawElementCallback = OnDrawElement,
                drawElementBackgroundCallback = OnDrawElementBackground,
                drawNoneElementCallback = OnDrawNoneElement,
                elementHeightCallback = GetElementHeight,
                onAddDropdownCallback = OnAddDropdownElement,
                onChangedCallback = ListUpdated,
                showDefaultBackground = false,
            };

            // Required to adjust element height changes.
            var rolType = GetType("UnityEditorInternal.ReorderableList");
            if (rolType != null) {
                // ClearCache was changed to InvalidateChche in newer versions of Unity.
                // To maintain backwards compatibility, we will attempt to retrieve each method in order.
                _clearCacheMethod = rolType.GetMethod("InvalidateCache", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_clearCacheMethod == null)
                    _clearCacheMethod = rolType.GetMethod("ClearCache", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            _tilingRules = serializedObject.FindProperty("m_TilingRules");

            RuleTypes = Tile.m_NeighborType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        }

        public virtual void OnDisable() {
            DestroyPreview();
        }

        public override void OnInspectorGUI() {
            this.serializedObject.Update();
            Undo.RecordObject(target, _undoName);

            EditorGUI.BeginChangeCheck();

            Rect background = EditorGUILayout.BeginVertical();
            background = new(0, 0, EditorGUIUtility.currentViewWidth, background.height);
            EditorGUI.DrawRect(background, InspectorBoxColor);

            StyleBackup();
            StyleApply();

            DrawDefaultProperties();
            EditorGUILayout.Space();
            DrawCustomFields(false);
            EditorGUILayout.Space();
            DrawTypeInfo();
            EditorGUILayout.Space();
            DrawRuleList();

            StyleRestore();

            if (EditorGUI.EndChangeCheck()) SaveTile();

            GUILayout.Space(DefaultElementHeight);

            EditorGUILayout.EndVertical();
        }

        #endregion

        #region DrawSupport

        private Color _labelNormalColor;
        private Color _labelHoverColor;
        private Color _labelFocusedColor;
        private FontStyle _labelFontStyle;

        private Texture2D _fieldNormalBackground;
        private Texture2D _fieldFocusedBackground;
        private Color _fieldNormalColor;
        private Color _fieldFocusedColor;
        private FontStyle _fieldFontStyle;

        private Texture2D _numberFieldNormalBackground;
        private Texture2D _numberFieldFocusedBackground;
        private Color _numberFieldNormalColor;
        private Color _numberFieldFocusedColor;

        private Texture2D _popupNormalBackground;
        private Texture2D _popupFocusedBackground;
        private Color _popupNormalColor;
        private Color _popupFocusedColor;
        private FontStyle _popupFontStyle;
        private static GUIStyle InnerBox {
            get {
                if (_innerBox == null) {
                    Texture2D texture = new(1, 1);
                    texture.SetPixel(0, 0, InspectorInnerBoxColor);
                    texture.Apply();
                    _innerBox = new();
                    _innerBox.normal.background = texture;
                }
                return _innerBox;
            }
        }
        private static GUIStyle _innerBox;
        private static Texture2D TextureFieldBackground => Base64ToTexture(s_InspectorFieldBackground);
        private static Texture2D TextureFieldBaclgroundFocused => Base64ToTexture(s_InspectorFieldBackground_Focused);
        private static Texture2D TexturePopupBackground => Base64ToTexture(s_InspectorPopupBackground);
        private static Texture2D TexturePopupBackgroundFocused => Base64ToTexture(s_InspectorPopupBackground_Focused);

        private bool _backup = false;
        private void StyleBackup() {
            if (_backup == true) Debug.LogError("??");

            _labelNormalColor = EditorStyles.label.normal.textColor;
            _labelHoverColor = EditorStyles.label.hover.textColor;
            _labelFocusedColor = EditorStyles.label.focused.textColor;
            _labelFontStyle = EditorStyles.label.fontStyle;

            _fieldNormalBackground = EditorStyles.objectField.normal.background;
            _fieldFocusedBackground = EditorStyles.objectField.focused.background;
            _fieldNormalColor = EditorStyles.objectField.normal.textColor;
            _fieldFocusedColor = EditorStyles.objectField.focused.textColor;
            _fieldFontStyle = EditorStyles.objectField.fontStyle;

            _numberFieldNormalBackground = EditorStyles.numberField.normal.background;
            _numberFieldFocusedBackground = EditorStyles.numberField.focused.background;
            _numberFieldNormalColor = EditorStyles.numberField.normal.textColor;
            _numberFieldFocusedColor = EditorStyles.numberField.focused.textColor;

            _popupNormalBackground = EditorStyles.popup.normal.background;
            _popupFocusedBackground = EditorStyles.popup.focused.background;
            _popupNormalColor = EditorStyles.popup.normal.textColor;
            _popupFocusedColor = EditorStyles.popup.focused.textColor;
            _popupFontStyle = EditorStyles.popup.fontStyle;

            _backup = true;
        }
        private void StyleApply() {
            EditorStyles.label.normal.textColor = InspectorTextColor;
            EditorStyles.label.hover.textColor = InspectorHoverTextColor;
            EditorStyles.label.focused.textColor = InspectorFocusedTextColor;
            EditorStyles.label.fontStyle = FontStyle.Bold;

            EditorStyles.objectField.normal.background = TexturePopupBackground;
            EditorStyles.objectField.focused.background = TexturePopupBackgroundFocused;
            EditorStyles.objectField.normal.textColor = InspectorTextColor;
            EditorStyles.objectField.focused.textColor = InspectorFocusedTextColor;
            EditorStyles.objectField.fontStyle = FontStyle.Bold;

            EditorStyles.numberField.normal.background = TextureFieldBackground;
            EditorStyles.numberField.focused.background = TextureFieldBaclgroundFocused;
            EditorStyles.numberField.normal.textColor = InspectorTextColor;
            EditorStyles.numberField.focused.textColor = InspectorFocusedTextColor;

            EditorStyles.popup.normal.background = TexturePopupBackground;
            EditorStyles.popup.focused.background = TexturePopupBackgroundFocused;
            EditorStyles.popup.normal.textColor = InspectorTextColor;
            EditorStyles.popup.focused.textColor = InspectorFocusedTextColor;
            EditorStyles.popup.fontStyle = FontStyle.Bold;
        }
        private void StyleRestore() {
            EditorStyles.label.normal.textColor = _labelNormalColor;
            EditorStyles.label.hover.textColor = _labelHoverColor;
            EditorStyles.label.focused.textColor = _labelFocusedColor;
            EditorStyles.label.fontStyle = _labelFontStyle;

            EditorStyles.objectField.normal.background = _fieldNormalBackground;
            EditorStyles.objectField.focused.background = _fieldFocusedBackground;
            EditorStyles.objectField.normal.textColor = _fieldNormalColor;
            EditorStyles.objectField.focused.textColor = _fieldFocusedColor;
            EditorStyles.objectField.fontStyle = _fieldFontStyle;

            EditorStyles.numberField.normal.background = _numberFieldNormalBackground;
            EditorStyles.numberField.focused.background = _numberFieldFocusedBackground;
            EditorStyles.numberField.normal.textColor = _numberFieldNormalColor;
            EditorStyles.numberField.focused.textColor = _numberFieldFocusedColor;

            EditorStyles.popup.normal.background = _popupNormalBackground;
            EditorStyles.popup.focused.background = _popupFocusedBackground;
            EditorStyles.popup.normal.textColor = _popupNormalColor;
            EditorStyles.popup.focused.textColor = _popupFocusedColor;
            EditorStyles.popup.fontStyle = _popupFontStyle;

            _backup = false;
        }

        #endregion

        #region ReorderableList Callbacks

        // Draws the header for the Rule list.
        private void OnDrawHeader(Rect rect) {
            // #1. Rect 계산.
            Rect headerRect = new(rect.x, rect.y + 1, rect.width, rect.height + 2);
            Rect toggleRect = new(rect.xMax - rect.height, rect.y, rect.height, rect.height);
            Vector2 extendSize = Styles.extendNeighborsStyle.CalcSize(Styles.extendNeighbor);
            float toggleWidth = toggleRect.width + extendSize.x + 5f;
            Rect toggleLabelRect = new(rect.x + rect.width - toggleWidth, rect.y, toggleWidth, rect.height);

            // #2. GUI 그리기.
            if (Event.current.type == EventType.Repaint)
                InnerBox.Draw(headerRect, isHover: false, isActive: false, on: false, hasKeyboardFocus: false);
            GUI.Label(rect, Styles.tilingRules, Styles.HeaderStyle);
            EditorGUI.BeginChangeCheck();
            _extendNeighbour = EditorGUI.Toggle(toggleRect, _extendNeighbour);
            EditorGUI.LabelField(toggleLabelRect, Styles.extendNeighbor, Styles.extendNeighborsStyle);
            if (EditorGUI.EndChangeCheck())
                if (_clearCacheMethod != null)
                    _clearCacheMethod.Invoke(_reorderableList, null);
        }

        protected virtual void OnDrawFooter(Rect rect) {
            // #1. Rect 계산.
            float padding = 4f;
            float buttonWidth = 25f;
            float buttonHeight = 16f;

            float footMaxX = rect.xMax - 10f;
            float footMinX = footMaxX - 2 * padding;
            if (_reorderableList.displayAdd) footMinX -= buttonWidth;
            if (_reorderableList.displayRemove) footMinX -= buttonWidth;
            rect = new(footMinX, rect.y, footMaxX - footMinX, rect.height);
            Rect addRect = new(footMinX + padding, rect.y, buttonWidth, buttonHeight);
            Rect removeRect = new(footMaxX - (buttonWidth + padding), rect.y, buttonWidth, buttonHeight);

            // #2. GUI 그리기.
            if (GUI.Button(addRect, Styles.addNewRule, EditorStyles.miniButtonLeft))
                OnAddDropdownElement(addRect, _reorderableList);

            bool canRemove = _reorderableList.index >= 0 && _reorderableList.index < _reorderableList.count;
            using (new EditorGUI.DisabledScope(!canRemove)) {
                if (GUI.Button(removeRect, Styles.removeNewRule, EditorStyles.miniButtonRight)) {
                    OnRemoveButton();
                }
            }
        }

        // Draws the Rule element for the Rule list.
        protected virtual void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            // #1. Rect 계산.
            Rect headerRect = new(rect.x - 6, rect.y + 1, rect.width + 12, rect.height + 2);
            RuleTile.TilingRule rule = Tile.m_TilingRules[index];
            BoundsInt bounds = GetRuleGUIBounds(rule.GetBounds(), rule);
            Vector2 matrixSize = GetMatrixSize(bounds);
            float yPos = rect.yMin + 2f;
            Rect spriteRect = new(rect.xMax - DefaultElementHeight - 5f, yPos, DefaultElementHeight, DefaultElementHeight);
            Rect matrixRect = new(rect.xMax - matrixSize.x - spriteRect.width - 10f, yPos, matrixSize.x, matrixSize.y);
            Rect inspectorRect = new(rect.xMin, yPos, rect.width - matrixSize.x - spriteRect.width - 20f, rect.height - PaddingBetweenRules);

            //if (Event.current.type == EventType.Repaint)
            //    InnerBox.Draw(rect, isHover: false, isActive: false, on: false, hasKeyboardFocus: false);

            RuleInspectorOnGUI(inspectorRect, rule);
            RuleMatrixOnGUI(Tile, matrixRect, bounds, rule);
            SpriteOnGUI(spriteRect, rule);
        }

        protected virtual void OnDrawElementBackground(Rect rect, int index, bool selected, bool focused) {
            //if (Event.current.type == EventType.Repaint) {
            //    ReorderableList.defaultBehaviours.elementBackground.Draw(rect, isHover: false, isActive: isActive, on: isActive, hasKeyboardFocus: isFocused);
            //}
            // ReorderableList.defaultBehaviours.elementBackground
            if (selected == false) return;
            rect.xMin += 3;
            rect.xMax -= 3;
            if (Event.current.type == EventType.Repaint) {
                InnerBox.Draw(rect, isHover: false, isActive: selected, on: selected, hasKeyboardFocus: focused);
            }


            //if (Event.current.type == EventType.Repaint)
            //    InnerBox.Draw(rect, isHover: false, isActive: isActive, on: isActive, hasKeyboardFocus: isFocused);
            //if (Event.current.type == EventType.Repaint)
            //    InnerBox.Draw(rect, isHover: false, isActive: false, on: false, hasKeyboardFocus: false);

        }

        protected virtual void OnDrawNoneElement(Rect rect) {
            EditorGUI.LabelField(rect, EditorGUIUtility.TrTextContent("List is Empty"));
        }

        private float GetElementHeight(int index) {
            RuleTile.TilingRule rule = Tile.m_TilingRules[index];
            return GetElementHeight(rule);
        }

        private void OnAddDropdownElement(Rect rect, ReorderableList list) {
            if (0 <= list.index && list.index < Tile.m_TilingRules.Count && list.IsSelected(list.index)) {
                GenericMenu menu = new();
                menu.AddItem(EditorGUIUtility.TrTextContent("Add"), false, OnAddElement, list);
                menu.AddItem(EditorGUIUtility.TrTextContent("Duplicate"), false, OnDupllicatedElement, list);
                menu.DropDown(rect);
            }
            else OnAddElement(list);
        }

        // Callback when the Rule list is updated.
        private void ListUpdated(ReorderableList list) {
            UpdateTilingRuleIDs();
        }

        #endregion

        #region Callbacks

        private void OnAddElement(object obj) {
            ReorderableList list = obj as ReorderableList;
            RuleTile.TilingRule rule = new() {
                m_Output = RuleTile.TilingRuleOutput.OutputSprite.Single,
                m_GameObject = Tile.m_DefaultGameObject,
                m_ColliderType = Tile.m_DefaultColliderType,
            };
            rule.m_Sprites[0] = Tile.m_DefaultSprite;

            int count = _tilingRules.arraySize;
            ResizeRuleTileList(count + 1);

            if (list.index == -1 || list.index >= list.count) Tile.m_TilingRules[count] = rule;
            else {
                Tile.m_TilingRules.Insert(list.index + 1, rule);
                Tile.m_TilingRules.RemoveAt(count + 1);
                if (list.IsSelected(list.index)) list.index += 1;
            }
            UpdateTilingRuleIDs();
        }

        private void OnDupllicatedElement(object obj) {
            ReorderableList list = obj as ReorderableList;
            if (list.index < 0 || list.index >= Tile.m_TilingRules.Count) return;

            RuleTile.TilingRule copyRule = Tile.m_TilingRules[list.index];
            RuleTile.TilingRule rule = copyRule.Clone();

            int count = _tilingRules.arraySize;
            ResizeRuleTileList(count + 1);

            Tile.m_TilingRules.Insert(list.index + 1, rule);
            Tile.m_TilingRules.RemoveAt(count + 1);
            if (list.IsSelected(list.index)) list.index += 1;
            UpdateTilingRuleIDs();
        }

        #endregion

        #region Draw

        #region DrawInspectorElement

        private void DrawDefaultProperties() {
            // #1. Rect 계산.
            float padding = 2;
            Rect rect = EditorGUILayout.BeginVertical();
            Rect contentRect = new(rect.x + padding, rect.y + padding, rect.width - 2 * padding, rect.height - 2 * padding);

            // #2. GUI 그리기.
            EditorGUILayout.Space(padding);
            EditorGUI.DrawRect(rect, InspectorEdgeColor);
            EditorGUI.DrawRect(contentRect, InspectorBoxColor);
            GUILayout.Space(padding + 8);
            GUILayout.Label("기본 정보", Styles.TitleStyle);
            Tile.m_DefaultSprite = EditorGUILayout.ObjectField(Styles.defaultSprite, Tile.m_DefaultSprite, typeof(Sprite), false) as Sprite;
            Tile.m_DefaultGameObject = EditorGUILayout.ObjectField(Styles.defaultGameObject, Tile.m_DefaultGameObject, typeof(GameObject), false) as GameObject;
            Tile.m_DefaultColliderType = (UnityEngine.Tilemaps.Tile.ColliderType)EditorGUILayout.EnumPopup(Styles.defaultCollider, Tile.m_DefaultColliderType);
            GUILayout.Space(padding + 8);
            EditorGUILayout.Space(padding);

            EditorGUILayout.EndVertical();
        }

        // Draw editor fields for custom properties for the RuleTile.
        private void DrawCustomFields(bool isOverrideInstance) {
            // #1. Rect 계산.
            float padding = 2;
            Rect rect = EditorGUILayout.BeginVertical();
            Rect contentRect = new(rect.x + padding, rect.y + padding, rect.width - 2 * padding, rect.height - 2 * padding);

            // #2. GUI 그리기.
            EditorGUILayout.Space(padding);
            EditorGUI.DrawRect(rect, InspectorEdgeColor);
            EditorGUI.DrawRect(contentRect, InspectorBoxColor);
            GUILayout.Space(padding + 8);
            GUILayout.Label("추가 정보", Styles.TitleStyle);

            FieldInfo[] customFields = Tile.GetCustomFields(isOverrideInstance);
            this.serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            foreach (FieldInfo field in customFields) {
                SerializedProperty property = this.serializedObject.FindProperty(field.Name);
                if (property != null) EditorGUILayout.PropertyField(property, true);
            }
            if (EditorGUI.EndChangeCheck()) {
                this.serializedObject.ApplyModifiedProperties();
                DestroyPreview();
                CreatePreview();
            }
            GUILayout.Space(padding + 8);

            EditorGUILayout.EndVertical();
        }

        private void DrawTypeInfo() {
            // #1. Rect 계산.
            int typeCount = RuleTypes.Count();
            float padding = 2f;
            float rectHeight = (SingleLineHeight + padding) * typeCount + padding * 3;
            Rect rect = EditorGUILayout.GetControlRect(false, rectHeight);
            rect.x -= 3;
            rect.width += 6;
            Rect contentRect = new(rect.x + padding, rect.y + padding, rect.width - 2 * padding, rect.height - 2 * padding);

            // #2. GUI 그리기.
            EditorGUI.DrawRect(rect, InspectorEdgeColor);
            EditorGUI.DrawRect(contentRect, InspectorBoxColor);
            Rect textRect = new(contentRect.x + padding, contentRect.y + padding, contentRect.width - 2 * padding, SingleLineHeight);
            for (int i = 0; i < typeCount; i++) {
                GUI.Label(textRect, EditorGUIUtility.TrTextContent(GetRuleTypeDescription(i + 1), Icons[i + 1][0]), Styles.BoldStyle);
                textRect.y += padding + SingleLineHeight;
            }
        }

        private void DrawRuleList() {
            // #1. Rect 계산.
            float padding = 2;
            Rect rect = EditorGUILayout.BeginVertical();
            Rect contentRect = new(rect.x + padding, rect.y + padding, rect.width - 2 * padding, rect.height - 2 * padding);

            // #2. GUI 그리기.
            EditorGUI.DrawRect(rect, InspectorEdgeColor);
            EditorGUI.DrawRect(contentRect, InspectorBoxColor);
            GUILayout.Space(padding + 8);
            GUILayout.Label("규칙", Styles.TitleStyle);

            EditorGUI.BeginChangeCheck();
            int count = EditorGUILayout.DelayedIntField(Styles.numberOfTilingRules, Tile.m_TilingRules?.Count ?? 0);
            if (count < 0) count = 0;
            if (EditorGUI.EndChangeCheck()) ResizeRuleTileList(count);

            if (count == 0) {
                Rect ruleRect = EditorGUILayout.GetControlRect(false, SingleLineHeight * 5);
                HandleDragAndDrop(ruleRect);
                EditorGUI.DrawRect(ruleRect, DragAndDropActive && ruleRect.Contains(Event.current.mousePosition) ? InspectorFocusedTextColor : InspectorEdgeColor);
                Rect innerRect = new(ruleRect.x + 1, ruleRect.y + 1, ruleRect.width - 2, ruleRect.height - 2);
                EditorGUI.DrawRect(innerRect, InspectorInnerBoxColor);
                DisplayClipboardText(Styles.emptyRuleTileInfo, ruleRect);
                GUILayout.Space(ruleRect.height);
                EditorGUILayout.Space();
            }

            _reorderableList?.DoLayoutList();
            GUILayout.Space(padding + 8);

            EditorGUILayout.EndVertical();
        }

        #endregion

        #region DrawListElement

        // Draws an Inspector for the Rule.
        private void RuleInspectorOnGUI(Rect rect, RuleTile.TilingRuleOutput tilingRule) {
            float labelX = rect.xMin;
            float y = rect.yMin;
            float contentX = labelX + LabelWidth;
            float contentWidth = rect.width - LabelWidth;
            DrawLabel(labelX, y, Styles.tilingRulesGameObject);
            tilingRule.m_GameObject = (GameObject)EditorGUI.ObjectField(new Rect(contentX, y, contentWidth, SingleLineHeight), "", tilingRule.m_GameObject, typeof(GameObject), false);
            y += SingleLineHeight;
            DrawLabel(labelX, y, Styles.tilingRulesCollider);
            tilingRule.m_ColliderType = (UnityEngine.Tilemaps.Tile.ColliderType)EditorGUI.EnumPopup(new Rect(contentX, y, contentWidth, SingleLineHeight), tilingRule.m_ColliderType);
            y += SingleLineHeight;
            DrawLabel(labelX, y, Styles.tilingRulesOutput);
            tilingRule.m_Output = (RuleTile.TilingRuleOutput.OutputSprite)EditorGUI.EnumPopup(new Rect(contentX, y, contentWidth, SingleLineHeight), tilingRule.m_Output);
            y += SingleLineHeight;

            if (tilingRule.m_Output == RuleTile.TilingRuleOutput.OutputSprite.Animation) {
                DrawLabel(labelX, y, Styles.tilingRulesMinSpeed);
                tilingRule.m_MinAnimationSpeed = EditorGUI.FloatField(new Rect(contentX, y, contentWidth, SingleLineHeight), tilingRule.m_MinAnimationSpeed);
                y += SingleLineHeight;
                DrawLabel(labelX, y, Styles.tilingRulesMaxSpeed);
                tilingRule.m_MaxAnimationSpeed = EditorGUI.FloatField(new Rect(contentX, y, contentWidth, SingleLineHeight), tilingRule.m_MaxAnimationSpeed);
                y += SingleLineHeight;
            }
            if (tilingRule.m_Output == RuleTile.TilingRuleOutput.OutputSprite.Random) {
                DrawLabel(labelX, y, Styles.tilingRulesNoise);
                tilingRule.m_PerlinScale = EditorGUI.Slider(new Rect(contentX, y, contentWidth, SingleLineHeight), tilingRule.m_PerlinScale, 0.001f, 0.999f);
                y += SingleLineHeight;

                DrawLabel(labelX, y, Styles.tilingRulesShuffle);
                tilingRule.m_RandomTransform = (RuleTile.TilingRuleOutput.Transform)EditorGUI.EnumPopup(new Rect(contentX, y, contentWidth, SingleLineHeight), tilingRule.m_RandomTransform);
                y += SingleLineHeight;
            }

            if (tilingRule.m_Output != RuleTile.TilingRuleOutput.OutputSprite.Single) {
                DrawLabel(labelX, y, tilingRule.m_Output == RuleTile.TilingRuleOutput.OutputSprite.Animation ? Styles.tilingRulesAnimationSize : Styles.tilingRulesRandomSize);
                EditorGUI.BeginChangeCheck();
                int newLength = EditorGUI.DelayedIntField(new Rect(contentX, y, contentWidth, SingleLineHeight), tilingRule.m_Sprites.Length);
                if (EditorGUI.EndChangeCheck())
                    Array.Resize(ref tilingRule.m_Sprites, Math.Max(newLength, 1));
                y += SingleLineHeight;

                for (int i = 0; i < tilingRule.m_Sprites.Length; i++) {
                    tilingRule.m_Sprites[i] = EditorGUI.ObjectField(new Rect(contentX, y, contentWidth, SingleLineHeight), tilingRule.m_Sprites[i], typeof(Sprite), false) as Sprite;
                    y += SingleLineHeight;
                }
            }
        }

        // Draws a Rule Matrix for the given Rule for a RuleTile.
        protected virtual void RuleMatrixOnGUI(RuleTile tile, Rect rect, BoundsInt bounds, RuleTile.TilingRule tilingRule) {
            Handles.color = InspectorTextColor;
            float unitWidth = rect.width / bounds.size.x;
            float unitHeight = rect.height / bounds.size.y;

            for (int y = 0; y <= bounds.size.y; y++) {
                float top = rect.yMin + y * unitHeight;
                Handles.DrawLine(new(rect.xMin, top), new(rect.xMax, top));
            }
            for (int x = 0; x <= bounds.size.x; x++) {
                float left = rect.xMin + x * unitWidth;
                Handles.DrawLine(new(left, rect.yMin), new(left, rect.yMax));
            }
            Handles.color = Color.white;

            Dictionary<Vector3Int, int> neighbours = tilingRule.GetNeighbors();

            for (int y = bounds.yMin; y < bounds.yMax; y++) {
                for (int x = bounds.xMin; x < bounds.xMax; x++) {
                    Vector3Int pos = new(x, y, 0);
                    Rect r = new(rect.xMin + (x - bounds.xMin) * unitWidth, rect.yMin + (-y + bounds.yMax - 1) * unitHeight, unitWidth - 1, unitHeight - 1);
                    RuleMatrixIconOnGUI(tilingRule, neighbours, pos, r);
                }
            }
        }

        #region DrawMatrixSupport

        // Draws a Rule Matrix Icon for the given matching Rule for a RuleTile with the given position.
        private void RuleMatrixIconOnGUI(RuleTile.TilingRule tilingRule, Dictionary<Vector3Int, int> neighbours, Vector3Int position, Rect rect) {
            using var check = new EditorGUI.ChangeCheckScope();
            if (position.x != 0 || position.y != 0) {
                if (neighbours.ContainsKey(position)) {
                    RuleOnGUI(rect, position, neighbours[position]);
                    RuleTooltipOnGUI(rect, neighbours[position]);
                }
                RuleNeighbourUpdate(rect, tilingRule, neighbours, position);
            }
            else {
                RuleTransformOnGUI(rect, tilingRule.m_RuleTransform);
                RuleTransformUpdate(rect, tilingRule);
            }
            if (check.changed) Tile.UpdateNeighborPositions();
        }

        // Draws a neighbour matching rule.
        protected virtual void RuleOnGUI(Rect rect, Vector3Int position, int neighbour) {
            switch (neighbour) {
                case ZerolizeDungeon.Tile.Neighbour.This:
                    GUI.DrawTexture(rect, Icons[1][GetArrowIndex(position)]); break;
                case ZerolizeDungeon.Tile.Neighbour.NotThis:
                    GUI.DrawTexture(rect, Icons[2][0]); break;
                case ZerolizeDungeon.Tile.Neighbour.Border:
                    GUI.DrawTexture(rect, Icons[3][GetArrowIndex(position)]); break;
                case ZerolizeDungeon.Tile.Neighbour.Wall:
                    GUI.DrawTexture(rect, Icons[4][GetArrowIndex(position)]); break;
                case ZerolizeDungeon.Tile.Neighbour.Edge:
                    GUI.DrawTexture(rect, Icons[5][GetArrowIndex(position)]); break;
                case ZerolizeDungeon.Tile.Neighbour.EmptyOrThis:
                    GUI.DrawTexture(rect, Icons[6][GetArrowIndex(position)]); break;
                case ZerolizeDungeon.Tile.Neighbour.EmptyOrBorder:
                    GUI.DrawTexture(rect, Icons[7][GetArrowIndex(position)]); break;
                case ZerolizeDungeon.Tile.Neighbour.WallOrEdge:
                    GUI.DrawTexture(rect, Icons[8][GetArrowIndex(position)]); break;
                case ZerolizeDungeon.Tile.Neighbour.NotBorderNorEdgeNorWall:
                    GUI.DrawTexture(rect, Icons[9][GetArrowIndex(position)]); break;
                default:
                    GUI.Label(rect, neighbour.ToString(), new() {
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 10
                    });
                    break;
            }
        }

        // Draws a tooltipo for the neighbour matching rule.
        private void RuleTooltipOnGUI(Rect rect, int neighbour) {
            var allConsts = Tile.m_NeighborType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            foreach (var c in allConsts) {
                if ((int)c.GetValue(null) == neighbour) {
                    GUI.Label(rect, new GUIContent("", c.Name));
                    break;
                }
            }
        }

        // Draws a transform matching rule.
        protected virtual void RuleTransformOnGUI(Rect rect, RuleTile.TilingRuleOutput.Transform ruleTransform) {
            switch (ruleTransform) {
                case RuleTile.TilingRuleOutput.Transform.Rotated:
                    GUI.DrawTexture(rect, Icons[0][0]);
                    break;
                case RuleTile.TilingRuleOutput.Transform.MirrorX:
                    GUI.DrawTexture(rect, Icons[0][1]);
                    break;
                case RuleTile.TilingRuleOutput.Transform.MirrorY:
                    GUI.DrawTexture(rect, Icons[0][2]);
                    break;
                case RuleTile.TilingRuleOutput.Transform.Fixed:
                    GUI.DrawTexture(rect, Icons[0][3]);
                    break;
                case RuleTile.TilingRuleOutput.Transform.MirrorXY:
                    GUI.DrawTexture(rect, Icons[0][4]);
                    break;
                case RuleTile.TilingRuleOutput.Transform.RotatedMirror:
                    GUI.DrawTexture(rect, Icons[0][5]);
                    break;
            }
            GUI.Label(rect, new GUIContent("", ruleTransform.ToString()));
        }

        #endregion

        // Draw a Sprite field for the Rule.
        protected virtual void SpriteOnGUI(Rect rect, RuleTile.TilingRuleOutput tilingRule) {
            tilingRule.m_Sprites[0] = EditorGUI.ObjectField(rect, tilingRule.m_Sprites[0], typeof(Sprite), false) as Sprite;
        }

        #endregion

        private void DisplayClipboardText(GUIContent clipboardText, Rect position) {
            Color old = GUI.color;
            GUI.color = InspectorTextColor;
            Vector2 infoSize = GUI.skin.label.CalcSize(clipboardText);
            Rect rect = new(position.center.x - infoSize.x * 0.5f,
                position.center.y - infoSize.y * 0.5f,
                infoSize.x,
                infoSize.y);
            GUI.Label(rect, clipboardText);
            GUI.color = old;
        }

        #region DrawBaseGUI

        private void DrawLabel(float x, float y, GUIContent content) {
            GUI.Label(new Rect(x, y, LabelWidth, SingleLineHeight), content, Styles.LabelStyle);
        }

        #endregion

        #endregion

        #region Calculate

        // Get the GUI bounds for a Rule.
        protected virtual BoundsInt GetRuleGUIBounds(BoundsInt bounds, RuleTile.TilingRule rule) {
            if (_extendNeighbour) {
                bounds.xMin--;
                bounds.yMin--;
                bounds.xMax++;
                bounds.yMax++;
            }
            bounds.xMin = Mathf.Min(bounds.xMin, -1);
            bounds.yMin = Mathf.Min(bounds.yMin, -1);
            bounds.xMax = Mathf.Max(bounds.xMax, 2);
            bounds.yMax = Mathf.Max(bounds.yMax, 2);
            return bounds;
        }

        // Gets the GUI matrix size for a Rule of a RuleTile.
        protected virtual Vector2 GetMatrixSize(BoundsInt bounds) {
            return new(bounds.size.x * SingleLineHeight, bounds.size.y * SingleLineHeight);
        }

        // Gets the GUI element height for a TilingRule.
        private float GetElementHeight(RuleTile.TilingRule rule) {
            BoundsInt bounds = GetRuleGUIBounds(rule.GetBounds(), rule);

            float inspectorHeight = GetElementHeight(rule as RuleTile.TilingRuleOutput);
            float matrixHeight = GetMatrixSize(bounds).y + 10f;

            return Mathf.Max(inspectorHeight, matrixHeight);
        }

        // Gets the GUI element height for a TilingRuleOutput.
        private float GetElementHeight(RuleTile.TilingRuleOutput rule) {
            float inspectorHeight = DefaultElementHeight + PaddingBetweenRules;
            switch (rule.m_Output) {
                case RuleTile.TilingRuleOutput.OutputSprite.Random:
                case RuleTile.TilingRuleOutput.OutputSprite.Animation:
                    inspectorHeight = DefaultElementHeight + SingleLineHeight * (rule.m_Sprites.Length + 3) + PaddingBetweenRules;
                    break;
            }
            return inspectorHeight;
        }

        private void UpdateTilingRuleIDs() {
            HashSet<int> existingIDSet = new();
            HashSet<int> usedIDSet = new();
            foreach (RuleTile.TilingRule rule in Tile.m_TilingRules)
                existingIDSet.Add(rule.m_Id);
            foreach (RuleTile.TilingRule rule in Tile.m_TilingRules) {
                if (usedIDSet.Contains(rule.m_Id)) {
                    while (existingIDSet.Contains(rule.m_Id)) rule.m_Id++;
                    existingIDSet.Add(rule.m_Id);
                }
                usedIDSet.Add(rule.m_Id);
            }
        }

        private string GetRuleTypeDescription(int index) {
            foreach (FieldInfo ruleType in RuleTypes) {
                if ((int)ruleType.GetValue(null) == index) {
                    return $"{index}. {ruleType.Name}: {ZerolizeDungeon.Tile.RuleDescription[index]}";
                }
            }
            return "";
        }

        private void ResizeRuleTileList(int count) {
            if (_tilingRules.arraySize == count) return;

            bool isEmpty = _tilingRules.arraySize == 0;
            _tilingRules.arraySize = count;
            serializedObject.ApplyModifiedProperties();
            if (isEmpty) {
                for (int i = 0; i < count; ++i) Tile.m_TilingRules[i] = new RuleTile.TilingRule();
            }
            UpdateTilingRuleIDs();
        }

        #endregion

        // Saves any changes to the RuleTile.
        private void SaveTile() {
            this.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            SceneView.RepaintAll();

            UpdateAffectedOverrideTiles(Tile);
        }

        // Updates all RuleOverrideTiles which override the given RuleTile.
        public static void UpdateAffectedOverrideTiles(RuleTile target) {
            List<RuleOverrideTile> overrideTiles = FindAffectedOverrideTiles(target);
            if (overrideTiles != null) {
                foreach (RuleOverrideTile overrideTile in overrideTiles) {
                    Undo.RegisterCompleteObjectUndo(overrideTile, _undoName);
                    Undo.RecordObject(overrideTile.m_InstanceTile, _undoName);
                    overrideTile.Override();
                    UpdateAffectedOverrideTiles(overrideTile.m_InstanceTile);
                    EditorUtility.SetDirty(overrideTile);
                }
            }
        }

        // Gets a RuleOverrideTIles which override the given RuleTile.
        public static List<RuleOverrideTile> FindAffectedOverrideTiles(RuleTile target) {
            List<RuleOverrideTile> overrideTiles = new();
            string[] overrideTileGuids = AssetDatabase.FindAssets("t:" + typeof(RuleOverrideTile).Name);
            foreach (string overrideTileGuid in overrideTileGuids) {
                string overrideTilePath = AssetDatabase.GUIDToAssetPath(overrideTileGuid);
                RuleOverrideTile overrideTile = AssetDatabase.LoadAssetAtPath<RuleOverrideTile>(overrideTilePath);
                if (overrideTile.m_Tile == target) {
                    overrideTiles.Add(overrideTile);
                }
            }
            return overrideTiles;
        }

        #region Callbacks

        private void OnRemoveButton() {
            int[] array = _reorderableList.index > 0 ? _reorderableList.selectedIndices.Reverse().ToArray() : (new int[1] { _reorderableList.index });

            int _num = -1;
            int[] array2 = array;
            foreach (int _num2 in array2) {
                if (_num2 >= _reorderableList.count) continue;
                if (_reorderableList.serializedProperty != null) {
                    _reorderableList.serializedProperty.DeleteArrayElementAtIndex(_num2);
                    if (_num2 < _reorderableList.count - 1) {
                        SerializedProperty serializedProperty = _reorderableList.serializedProperty.GetArrayElementAtIndex(_num2);
                        for (int j = _num2 + 1; j < _reorderableList.count; j++) {
                            SerializedProperty arrayElementAtIndex = _reorderableList.serializedProperty.GetArrayElementAtIndex(j);
                            serializedProperty.isExpanded = arrayElementAtIndex.isExpanded;
                            serializedProperty = arrayElementAtIndex;
                        }
                    }
                }
                else {
                    _reorderableList.list.RemoveAt(_reorderableList.index);
                }
                _num = _num2;
            }
            _reorderableList.index = Mathf.Clamp(_num - 1, 0, _reorderableList.count - 1);
            Undo.SetCurrentGroupName("Remove Element From Array");

            _reorderableList.onChangedCallback?.Invoke(_reorderableList);
            GUI.changed = true;
        }

        #endregion

        #region Handles update

        // Handles a neighbour matching Rule update from user mouse input.
        private void RuleNeighbourUpdate(Rect rect, RuleTile.TilingRule tilingRule, Dictionary<Vector3Int, int> neighbours, Vector3Int position) {
            if (Event.current.type == EventType.MouseDown && ContainsMousePosition(rect)) {
                FieldInfo[] allConsts = Tile.m_NeighborType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                List<int> neighbourConsts = allConsts.Select(c => (int)c.GetValue(null)).ToList();
                neighbourConsts.Sort();

                if (neighbours.ContainsKey(position)) {
                    int oldIndex = neighbourConsts.IndexOf(neighbours[position]);
                    int newIndex = oldIndex + GetMouseChange();
                    if (newIndex >= 0 && newIndex < neighbourConsts.Count) {
                        newIndex = (int)Mathf.Repeat(newIndex, neighbourConsts.Count);
                        neighbours[position] = neighbourConsts[newIndex];
                    }
                    else neighbours.Remove(position);
                }
                else neighbours.Add(position, neighbourConsts[GetMouseChange() == 1 ? 0 : (neighbourConsts.Count - 1)]);
                tilingRule.ApplyNeighbors(neighbours);

                GUI.changed = true;
                Event.current.Use();
            }
        }
        private void RuleTransformUpdate(Rect rect, RuleTile.TilingRule tilingRule) {
            if (Event.current.type == EventType.MouseDown && ContainsMousePosition(rect)) {
                tilingRule.m_RuleTransform = (RuleTile.TilingRuleOutput.Transform)(int)Mathf.Repeat((int)tilingRule.m_RuleTransform + GetMouseChange(), Enum.GetValues(typeof(RuleTile.TilingRule.Transform)).Length);
                GUI.changed = true;
                Event.current.Use();
            }
        }

        #endregion

        #region MouseInput

        // Determines the current mouse position is within the given Rect.
        protected virtual bool ContainsMousePosition(Rect rect) => rect.Contains(Event.current.mousePosition);

        // Gets the offset change for a mouse click input.
        public static int GetMouseChange() => Event.current.button == 1 ? -1 : 1;

        // Gets the index for a Rule with the RuleTile to display an arrow.
        protected virtual int GetArrowIndex(Vector3Int position) {
            if (Mathf.Abs(position.x) == Mathf.Abs(position.y)) {
                if (position.x < 0 && position.y > 0) return 0;
                else if (position.x > 0 && position.y > 0) return 2;
                else if (position.x < 0 && position.y < 0) return 5;
                else if (position.x > 0 && position.y < 0) return 7;
            }
            else if (Mathf.Abs(position.x) > Mathf.Abs(position.y)) {
                if (position.x > 0) return 4;
                else return 3;
            }
            else {
                if (position.y > 0) return 1;
                else return 6;
            }
            return -1;
        }

        private void HandleDragAndDrop(Rect guiRect) {
            if (DragAndDrop.objectReferences.Length==0 || !guiRect.Contains(Event.current.mousePosition)) return;

            switch (Event.current.type) {
                case EventType.DragUpdated: {
                        _dragAndDropSprites = GetValidSingleSprites(DragAndDrop.objectReferences);
                        if (DragAndDropActive) {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            Event.current.Use();
                            GUI.changed = true;
                        }
                    }
                    break;
                case EventType.DragPerform: {
                        if (!DragAndDropActive)
                            return;

                        Undo.RegisterCompleteObjectUndo(Tile, "Drag and Drop to Rule Tile");
                        ResizeRuleTileList(_dragAndDropSprites.Count);
                        for (int i = 0; i < _dragAndDropSprites.Count; ++i) {
                            Tile.m_TilingRules[i].m_Sprites[0] = _dragAndDropSprites[i];
                        }
                        DragAndDropClear();
                        GUI.changed = true;
                        EditorUtility.SetDirty(Tile);
                        GUIUtility.ExitGUI();
                    }
                    break;
                case EventType.Repaint:
                    // Handled in Render()
                    break;
            }

            if (Event.current.type == EventType.DragExited ||
                Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape) {
                DragAndDropClear();
            }
        }
        private void DragAndDropClear() {
            _dragAndDropSprites = null;
            DragAndDrop.visualMode = DragAndDropVisualMode.None;
            Event.current.Use();
        }

        #endregion

        #region Preview

        // Weather the RuleTile has a preview GUI.
        public override bool HasPreviewGUI() => true;

        // Draws the preview GUI for the RuleTile.
        public override void OnPreviewGUI(Rect r, GUIStyle background) {
            if (_previewUtility == null) CreatePreview();

            if (Event.current.type != EventType.Repaint) return;

            _previewUtility.BeginPreview(r, background);
            _previewUtility.camera.orthographicSize = 2;
            if (r.height > r.width)
                _previewUtility.camera.orthographicSize *= r.height / r.width;
            _previewUtility.camera.Render();
            _previewUtility.EndAndDrawPreview(r);
        }

        // Create a Preview for the RuleTile.
        protected virtual void CreatePreview() {
            _previewUtility = new PreviewRenderUtility(true);
            _previewUtility.camera.orthographic = true;
            _previewUtility.camera.orthographicSize = 2;
            _previewUtility.camera.transform.position = new Vector3(0, 0, -10);

            var previewInstance = new GameObject();
            _previewGrid = previewInstance.AddComponent<Grid>();
            _previewUtility.AddSingleGO(previewInstance);

            _previewTilemaps = new List<Tilemap>();
            _previewTilemapRenderers = new List<TilemapRenderer>();

            for (int i = 0; i < 4; i++) {
                var previewTilemapGo = new GameObject();
                _previewTilemaps.Add(previewTilemapGo.AddComponent<Tilemap>());
                _previewTilemapRenderers.Add(previewTilemapGo.AddComponent<TilemapRenderer>());

                previewTilemapGo.transform.SetParent(previewInstance.transform, false);
            }

            for (int x = -2; x <= 0; x++)
                for (int y = -1; y <= 1; y++)
                    _previewTilemaps[0].SetTile(new Vector3Int(x, y, 0), Tile);

            for (int y = -1; y <= 1; y++)
                _previewTilemaps[1].SetTile(new Vector3Int(1, y, 0), Tile);

            for (int x = -2; x <= 0; x++)
                _previewTilemaps[2].SetTile(new Vector3Int(x, -2, 0), Tile);

            _previewTilemaps[3].SetTile(new Vector3Int(1, -2, 0), Tile);
        }

        // Handles cleanup for the Preview GUI.
        protected virtual void DestroyPreview() {
            if (_previewUtility != null) {
                _previewUtility.Cleanup();
                _previewUtility = null;
                _previewGrid = null;
                _previewTilemaps = null;
                _previewTilemapRenderers = null;
            }
        }

        // Renders a static preview Texture2D for a RuleTile asset.
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
            if (Tile.m_DefaultSprite != null) {
                Type type = GetType("UnityEditor.SpriteUtility");
                if (type != null) {
                    MethodInfo method = type.GetMethod("RenderStaticPreview", new[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) });
                    if (method != null) {
                        object ret = method.Invoke("RenderStaticPreview", new object[] { Tile.m_DefaultSprite, Color.white, width, height });
                        if (ret is Texture2D texture2D) return texture2D;
                    }
                }
            }
            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }
        #endregion

        #region Utilities

        private static Type GetType(string typeName) {
            Type type = Type.GetType(typeName);
            if (type != null) return type;

            var currentAssembly = Assembly.GetExecutingAssembly();
            var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
            foreach (var assemblyName in referencedAssemblies) {
                var assembly = Assembly.Load(assemblyName);
                if (assembly != null) {
                    type = assembly.GetType(typeName);
                    if (type != null) return type;
                }
            }
            return null;
        }
        private static Texture2D Base64ToTexture(string base64) {
            Texture2D texture = new(1, 1);
            texture.hideFlags = HideFlags.HideAndDontSave; ;
            texture.LoadImage(Convert.FromBase64String(base64));
            return texture;
        }
        private static List<Sprite> GetSpritesFromTexture(Texture2D texture) {
            string path = AssetDatabase.GetAssetPath(texture);
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            List<Sprite> sprites = new();

            foreach (Object asset in assets) if (asset is Sprite sprite) sprites.Add(sprite);

            return sprites;
        }
        private static List<Sprite> GetValidSingleSprites(Object[] objects) {
            List<Sprite> result = new();
            foreach (Object obj in objects) {
                if (obj is Sprite sprite) result.Add(sprite);
                else if (obj is Texture2D texture2D) {
                    List<Sprite> sprites = GetSpritesFromTexture(texture2D);
                    if (sprites.Count > 0) result.AddRange(sprites);
                }
            }
            return result;
        }

        #endregion

        //// Wrapper for serializing a list of Rules.
        //[Serializable]
        //class RuleTileRuleWrapper {
        //    [SerializeField]
        //    public List<RuleTile.TilingRule> rules = new();
        //}

        //// Copies all Rules from a RuleTile to the clipboard.
        //[MenuItem("CONTEXT/Tile/RuleTile Copy All Rules")]
        //public static void CopyAllRules(MenuCommand item) {
        //    RuleTile tile = item.context as RuleTile;
        //    if (tile == null) return;

        //    RuleTileRuleWrapper rulesWrapper = new() {
        //        rules = tile.m_TilingRules,
        //    };
        //    string rulesJson = EditorJsonUtility.ToJson(rulesWrapper);
        //    EditorGUIUtility.systemCopyBuffer = rulesJson;
        //}

        //// Pastes all Rules from the clipboard to a RuleTile.
        //[MenuItem("CONTEXT/Tile/RuleTile Paste Rules")]
        //public static void PasteRules(MenuCommand item) {
        //    RuleTile tile = item.context as RuleTile;
        //    if (tile == null) return;

        //    try {
        //        RuleTileRuleWrapper rulesWrapper = new();
        //        EditorJsonUtility.FromJsonOverwrite(EditorGUIUtility.systemCopyBuffer, rulesWrapper);
        //        tile.m_TilingRules.AddRange(rulesWrapper.rules);
        //    }
        //    catch (Exception) {
        //        Debug.LogError("Unable to paste rules from system copy buffer");
        //    }
        //}

    }
#endif
}