using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HpBar : MonoBehaviour {

    private readonly float defaultWidth = 100f;
    private readonly float defaultHeight = 20f;
    private readonly float showTime = 2.5f;
    private readonly float animationTime = 0.25f;


    public Entity Owner { get; protected set; }
    public float Timer {
        get => _timer;
        set {
            if (value <= 0) {
                if (_timer <= 0) return;
                _timer = 0;
                HideHpBar();
            }
            else {
                _timer = value;
            }
        }
    }

    private float _timer;
    private bool _isInitialized;

    private Canvas _canvas;
    private Slider _slider;
    private Image _background;
    private Image _fill;
    private RectTransform _rect;

    void OnEnable() {
        Initialize();
    }

    void Update() {
        Timer -= Time.deltaTime;
    }

    private bool Initialize() {
        if (_isInitialized) return false;
        _isInitialized = true;

        _canvas = this.GetComponent<Canvas>();
        _slider = this.GetComponentInChildren<Slider>();
        _background = _slider.transform.Find("Background").GetComponent<Image>();
        _fill = _slider.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
        _rect = _slider.GetComponent<RectTransform>();

        return true;
    }

    public void SetInfo(Entity entity, float hp, float from = -1) {
        DOTween.KillAll(this);
        this.Owner = entity;
        Timer = showTime;
        this.transform.SetParent(entity.transform, false);

        _canvas.sortingOrder = entity.GetComponent<SpriteRenderer>().sortingOrder;
        float ratio = entity.SpriteWidthRatio;
        float sizeX = defaultWidth * ratio;
        float sizeY = defaultHeight * (1 + 0.1f * (ratio < 0 ? (-1 / ratio) : ratio));
        _rect.sizeDelta = new(sizeX, sizeY);

        ShowHpBar();
        SetCreatureHpBar(from, hp);
    }

    private void ShowHpBar() {
        _background.color = Color.white;
        _fill.color = Color.white;
    }

    private void SetCreatureHpBar(float prevHp, float currHp) {
        DOTween.KillAll(this);
        Creature creature = Owner as Creature;
        if (prevHp < 0) {
            _slider.value = Mathf.Clamp01(currHp / creature.HpMax);
            return;
        }
        float from = Mathf.Clamp01(prevHp / creature.HpMax);
        float to = Mathf.Clamp01(currHp / creature.HpMax);
        DOTween.To(()=>from, x => _slider.value = x, to, animationTime);
    }

    private void HideHpBar() {
        DOTween.KillAll(this);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_background.DOFade(0, animationTime))
            .Join(_fill.DOFade(0, animationTime))
            .OnComplete(() => Main.Resource.Destroy(this.gameObject));
    }

}