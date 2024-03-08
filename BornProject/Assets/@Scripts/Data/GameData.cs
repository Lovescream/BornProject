using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData {

    public bool FirstPlay { get; set; }

    private Dictionary<SkillType, string> _skills = new();
    private Dictionary<AudioType, float> _audioVolume = new();
    private Dictionary<AudioType, bool> _audioMute = new();

    public string this[SkillType type] {
        get {
            if (!_skills.TryGetValue(type, out string skillKey)) return "";
            return skillKey;
        }
        set {
            _skills[type] = value;
        }
    }
    public float this[AudioType type] {
        get {
            if (!_audioVolume.TryGetValue(type, out float volume)) return 0.5f;
            return volume;
        }
        set {
            _audioVolume[type] = value;
            Main.Audio.SetVolume(type, value);
        }
    }
    public bool this[AudioType type, bool isMuteInfo] {
        get {
            if (!_audioMute.TryGetValue(type, out bool isMute)) return false;
            return isMute;
        }
        set {
            _audioMute[type] = value;
            Main.Audio.SetMute(type, value);
        }
    }

    public GameData() {
        FirstPlay = true;
        for (int i = 0; i < (int)AudioType.COUNT; i++) {
            _audioVolume[(AudioType)i] = 0.5f;
            _audioMute[(AudioType)i] = false;
        }
    }

    public void NewGame() {
        _skills.Clear();
    }

}