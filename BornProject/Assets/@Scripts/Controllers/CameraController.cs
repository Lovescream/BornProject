using DungeonGenerate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    #region Properties

    public float CamWidth { get; private set; }
    public float CamHeight { get; private set; }
    public float ScreenRatio => (float)Screen.width / Screen.height;

    #endregion

    #region Fields

    private Transform _target;

    #endregion

    void Awake() {
        Camera.main.orthographicSize = 6f;
        CamHeight = Camera.main.orthographicSize;
        CamWidth = CamHeight * ScreenRatio;
    }

    void LateUpdate() {
        Follow();
    }

    public void SetTarget(Transform target) {
        this._target = target;
        this.transform.position = new(target.position.x, target.position.y, -10);
    }
    private void Follow() {
        if (_target == null) return;

        Room room = _target.GetComponent<Creature>().CurrentRoom;
        float x, y, z;
        if (room == null) {
            x = _target.position.x;
            y = _target.position.y;
            z = -10;
        }
        else {
            float limitX = room.Width * 0.5f - CamWidth;
            float limitY = room.Height * 0.5f - CamHeight;
            x = Mathf.Clamp(_target.position.x, room.CenterPosition.x - limitX - 1, room.CenterPosition.x + limitX + 1);
            y = Mathf.Clamp(_target.position.y, room.CenterPosition.y - limitY - 1, room.CenterPosition.y + limitY + 1);
            z = -10;
        }
        Vector3 newPosition = new(x, y, z);
        this.transform.position = Vector3.Lerp(this.transform.position, newPosition, Time.deltaTime * 2f);
    }
}