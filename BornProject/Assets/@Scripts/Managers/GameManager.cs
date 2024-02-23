using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager {

    public GameData Current {
        get {
            _current ??= new();
            return _current;
        }
        set => _current = value;
    }

    public Player Player { get; set; }

    private GameData _current;
}