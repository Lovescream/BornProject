using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Toast : UI_Base {

    #region Enums

    enum Texts {
        txtMessage,
    }

    #endregion

    #region MonoBehaviours

    protected override void OnEnable() {
        RectTransform rect = this.transform.GetChild(0).GetComponent<RectTransform>();
        rect.localScale = new Vector3(0.8f, 0.8f, 1.0f);
        rect.DOScale(1.0f, 0.1f).SetEase(Ease.InOutBack).SetUpdate(true);
        rect.anchoredPosition = new Vector2(0, -650f);
        rect.DOAnchorPosY(-450f, 0.1f).SetEase(Ease.InOutBack).SetUpdate(true);
        Initialize();
        StartCoroutine(CoClose());
    }

    #endregion

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        Main.UI.SetCanvas(this.gameObject, isToast: true);
        BindText(typeof(Texts));

        return true;
    }

    public void SetInfo(string message) {
        this.transform.localScale = Vector3.one;
        GetText((int)Texts.txtMessage).text = message;
    }

    private IEnumerator CoClose() {
        yield return new WaitForSeconds(1f);
        Main.UI.CloseToast(this);
    }
}