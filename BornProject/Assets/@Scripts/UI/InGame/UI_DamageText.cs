using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour {

    private TextMeshPro txtDamage;

    public void SetInfo(Vector2 position, float damage = 0) {
        this.txtDamage = this.GetComponent<TextMeshPro>();
        this.transform.position = position;
        txtDamage.text = $"{Mathf.RoundToInt(damage)}";
        txtDamage.alpha = 1;
        PlayAnimation();
    }

    private void PlayAnimation() {
        Sequence sequence = DOTween.Sequence();

        this.transform.localScale = Vector3.zero;

        sequence.Append(this.transform.DOScale(1.25f, 0.25f).SetEase(Ease.InOutBounce))
            .Join(this.transform.DOMove(this.transform.position + Vector3.up, 0.25f).SetEase(Ease.Linear))
            .Append(this.transform.DOScale(1.0f, 0.25f).SetEase(Ease.InOutBounce))
            .Join(txtDamage.DOFade(0, 0.25f).SetEase(Ease.InQuint))
            .OnComplete(() => {
                Main.Resource.Destroy(this.gameObject);
            });
    }
}