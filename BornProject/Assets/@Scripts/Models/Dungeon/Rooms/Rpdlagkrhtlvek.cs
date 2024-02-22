using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rpdlagkrhtlvek : MonoBehaviour {

    public event Action OnEnteredPlayer;

    protected void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<Player>()) {
            OnEnteredPlayer?.Invoke();
        }
    }

}