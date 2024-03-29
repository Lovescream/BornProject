using ZerolizeDungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

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
        Camera.main.orthographicSize = 5f;
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

        if (SceneManager.GetActiveScene().name == "PlayerTestScene") {
            Tilemap map = FindObjectOfType<Grid>().transform.Find("WallTilemap").GetComponent<Tilemap>();
            Vector3 roomSize = map.size;
            Vector3 roomPos = map.origin + roomSize / 2;

            float _x, _y, _z;
            float _limitX = roomSize.x * 0.5f - CamWidth - 1;
            float _limitY = roomSize.y * 0.5f - CamHeight - 1;
            _x = _limitX >= 0 ? Mathf.Clamp(_target.position.x, roomPos.x - _limitX, roomPos.x + _limitX) : roomPos.x;
            _y = _limitY >= 0 ? Mathf.Clamp(_target.position.y, roomPos.y - _limitY, roomPos.y + _limitY) : roomPos.y;
            _z = -10;
            Vector3 newPos = new(_x, _y, _z);
            this.transform.position = Vector3.Lerp(this.transform.position, newPos, Time.deltaTime * 2f);

            return;
        }

        Room room = _target.GetComponent<Creature>().CurrentRoom;
        float x, y, z;
        if (room == null) {
            x = _target.position.x;
            y = _target.position.y;
            z = -10;
        }
        else {
            float limitX = room.Width * 0.5f - CamWidth + 1;
            float limitY = room.Height * 0.5f - CamHeight + 1;
            x = limitX >= 0 ? Mathf.Clamp(_target.position.x, room.CenterPosition.x - limitX, room.CenterPosition.x + limitX) : room.CenterPosition.x;
            y = limitY >= 0 ? Mathf.Clamp(_target.position.y, room.CenterPosition.y - limitY, room.CenterPosition.y + limitY) : room.CenterPosition.y;
            z = -10;
        }
        Vector3 newPosition = new(x, y, z);
        this.transform.position = Vector3.Lerp(this.transform.position, newPosition, Time.deltaTime * 8f);
    }

}