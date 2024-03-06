using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum AudioType {
    BGM,
    SFX,
    COUNT,
}

public class AudioManager {

    #region Properties

    public Transform Root {
        get {
            if (_root == null) {
                _root = new GameObject("@AudioPlayer").transform;
                Object.DontDestroyOnLoad(_root);
            }
            return _root;
        }
    }

    #endregion

    #region Fields

    private Transform _root;

    private Dictionary<AudioType, List<AudioSource>> _audioSources = new();

    private bool _isInitialized;

    #endregion

    public void Initialize() {
        if (_isInitialized) return;
        _isInitialized = true;

        for (int i = 0; i < (int)AudioType.COUNT; i++)
            _audioSources[(AudioType)i] = new();
    }

    #region Play

    // BGM을 재생합니다. audioSourceKey를 지정하면 해당 AudioSource가 재생하도록 합니다.
    public bool PlayBGM(string key, string audioSourceKey = "") {
        // #1. 음소거 설정 시 재생하지 않음.
        if (Main.Game.Current[AudioType.BGM, isMuteInfo: false]) return false;

        // #2. 오디오 소스 찾기.
        AudioSource source;
        if (!string.IsNullOrEmpty(audioSourceKey)) {
            source = FindPlayer(audioSourceKey, AudioType.BGM);
            if (source == null) source = NewSource(AudioType.BGM, audioSourceKey);
        }
        else source = GetAvailablePlayer(AudioType.BGM);

        // #3. 클립 로드.
        //AudioClip clip = Main.Resource.LoadAudioClip(key);
        AudioClip clip = Main.Resource.Get<AudioClip>(key);
        if (clip == null) {
            Debug.LogError($"[AudioManager] PlaySFX({key}): Not found AudioClip.");
            return false;
        }

        // #4. 클립 재생.
        source.clip = clip;
        source.Play();
        source.loop = true;
        return true;
    }

    // SFX를 재생합니다.
    public void PlaySFX(string key) {
        // #1. 음소거 설정 시 재생하지 않음.
        if (Main.Game.Current[AudioType.SFX, isMuteInfo: false]) return;

        // #2. 재생 가능한 오디오 소스 찾기.
        AudioSource source = GetAvailablePlayer(AudioType.SFX);

        // #3. 클립 로드.
        //AudioClip clip = Main.Resource.LoadAudioClip(key);
        AudioClip clip = Main.Resource.Get<AudioClip>(key);
        if (clip == null) {
            Debug.LogError($"[AudioManager] PlaySFX({key}): Not found AudioClip.");
            return;
        }

        // #4. 클립 재생.
        source.clip = clip;
        source.loop = false;
        source.Play();
    }

    // Creature의 상태 SFX를 재생합니다.
    public void Play(Creature creature, CreatureState state) {
        if (creature is Player) PlaySFX($"SFX_Player_{state}");
        else PlaySFX($"SFX_Enemy_{creature.Data.Key}_{state}");
    }
    // Skill의 상태 SFX를 재생합니다.
    // Skill의 전체 Key를 기준으로 클립을 찾고, 찾지 못하면 BaseName을 기준으로 클립을 찾습니다.
    // Ex) Laser 스킬을 하나의 효과음으로 돌려 쓸 수 있고, 특정 스킬은 특정 효과음을 사용하도록 할 수 있습니다.
    public void Play(Skill skill, string state = "") {
        if (string.IsNullOrEmpty(state)) state = "Shot";
        //if (Main.Resource.IsExistAudioClip($"SFX_Skill_{skill.Data.Name}_{state}"))
        //    PlaySFX($"SFX_Skill_{skill.Data.Name}_{state}");
        //else if (Main.Resource.IsExistAudioClip($"SFX_Skill_{skill.BaseKey}_{state}"))
        //    PlaySFX($"SFX_Skill_{skill.BaseKey}_{state}");
        if (Main.Resource.IsExist<AudioClip>($"SFX_Skill_{skill.Data.Name}_{state}"))
            PlaySFX($"SFX_Skill_{skill.Data.Name}_{state}");
        else if (Main.Resource.IsExist<AudioClip>($"SFX_Skill_{skill.BaseKey}_{state}"))
            PlaySFX($"SFX_Skill_{skill.BaseKey}_{state}");
    }
    public void PlayOnButton() => PlaySFX("SFX_OnButton");

    // 모든 AudioSource의 재생을 멈춥니다.
    // type을 지정하면, 해당 type의 AudioSource의 재생을 멈춥니다.
    // audioSourceKey를 지정하면, 해당 key를 가진 AudioSource의 재생을 멈춥니다.
    public void Stop(AudioType type = AudioType.COUNT, string audioSourceKey = "") {
        if (string.IsNullOrEmpty(audioSourceKey)) {
            if (type == AudioType.COUNT)
                foreach (List<AudioSource> list in _audioSources.Values)
                    list.ForEach(x => x.Stop());
            else _audioSources[type].ForEach(x => x.Stop());
        }
        else {
            AudioSource source = FindPlayer(audioSourceKey, type);
            if (source == null) {
                Debug.LogError($"[AudioManager] Stop({type}, {audioSourceKey}): Not found AudioSource.");
                return;
            }
            source.Stop();
        }
    }

    #endregion

    #region Settings

    // 모든 AudioSource를 삭제합니다.
    public void Clear() {
        foreach (List<AudioSource> sources in _audioSources.Values) {
            for (int i = sources.Count - 1; i >= 0; i--) {
                sources[i].Stop();
                Object.Destroy(sources[i].gameObject);
            }
            sources.Clear();
        }
    }

    // 해당 AudioType의 음소거 여부를 설정합니다.
    public void SetMute(AudioType type, bool isMute) {
        foreach (AudioSource source in _audioSources[type])
            source.mute = isMute;
    }

    // 해당 AudioType의 음량을 설정합니다.
    public void SetVolume(AudioType type, float volume) {
        foreach (AudioSource source in _audioSources[type])
            source.volume = volume;
    }

    #endregion

    #region AudioSource

    // 새 AudioSource를 생성합니다. 생성되는 AudioSource의 Key를 지정하지 않으면 기본값으로 설정됩니다.
    private AudioSource NewSource(AudioType type, string key = "Jihu") {
        // #1. 새 게임 오브젝트 생성.
        GameObject obj = new($"{type}Player[{_audioSources[type].Count:00}]:{key}");

        // #2. Transform 설정
        obj.transform.SetParent(Root);

        // #3. AudioSource 생성 및 설정.
        AudioSource newSource = obj.AddComponent<AudioSource>();
        newSource.mute = Main.Game.Current[type, isMuteInfo: true];
        newSource.volume = Main.Game.Current[type];

        // #4. 리스트에 추가.
        _audioSources[type].Add(newSource);

        return newSource;
    }

    // 재생중이지 않은 AudioSource를 찾습니다. Key를 지정하지 않으면 기본값으로 설정됩니다.
    // 재생중이지 않은 AudioSource가 없다면 새로 생성합니다.
    private AudioSource GetAvailablePlayer(AudioType type, string key = "Jihu") {
        AudioSource source = _audioSources[type].Where(x => x.isPlaying == false).FirstOrDefault();
        if (source == null) source = NewSource(type, key);
        return source;
    }

    // Key값을 가지는 AudioSource를 찾습니다. type을 지정하지 않으면 모든 타입에서 찾습니다.
    private AudioSource FindPlayer(string key, AudioType type = AudioType.COUNT) {
        if (type != AudioType.COUNT) {
            return _audioSources[type].FirstOrDefault(x => x.name.Split(':')[1].Equals(key));
        }
        else {
            foreach (List<AudioSource> sources in _audioSources.Values) {
                AudioSource source = sources.Where(x => x.name.Split(':')[1].Equals(key)).FirstOrDefault();
                if (source != null) return source;
            }
            return null;
        }
    }

    #endregion
}