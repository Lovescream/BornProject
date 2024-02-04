using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEx : MonoBehaviour {

    public float length = 1;

    private SpriteRenderer spriter;
    private Sprite sprite;
    private float unitRatio;

    void Awake() {
        spriter = this.GetComponent<SpriteRenderer>();
        sprite = spriter.sprite;
        float spriteSize = sprite.textureRect.size.x;
        float ppu = sprite.pixelsPerUnit;
        unitRatio = spriteSize / ppu;
    }
    void Update() {
        spriter.size = new(unitRatio * length, unitRatio);
    }

}