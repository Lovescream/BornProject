using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour {

    [Header("Muzzle")]
    public Transform muzzle;
    [Header("LaserPieces")]
    public GameObject laserStart;
    public GameObject laserBody;
    public GameObject laserEnd;

    private readonly float laserLength_Start = 0.25f;
    private readonly float laserLength_Body = 0.25f;
    private readonly float laserLength_End = 0.5f;

    private GameObject start;
    private GameObject end;
    private List<GameObject> bodies = new();
    private List<GameObject> deactiveBodies = new();

    void Update() {
        if (start == null) {
            start = Instantiate(laserStart);
            start.transform.SetParent(this.transform);
        }
        if (end == null) {
            end = Instantiate(laserEnd);
            end.transform.SetParent(this.transform);
        }

        float maxLaserSize = 20f;

        Vector2 laserDirection = this.transform.right;
        RaycastHit2D hit = Physics2D.Raycast(muzzle.position, laserDirection, maxLaserSize);
        Vector2 hitPoint = hit ? hit.point : new(muzzle.position.x + laserLength_Start + laserLength_Body * 20 + laserLength_End, 0);
        
        Vector2 startPosition = muzzle.position;
        start.transform.position = startPosition;

        Vector2 endPosition = new(hitPoint.x - laserLength_End, hitPoint.y);
        end.transform.position = endPosition;

        Vector2 bodyStartPosition = startPosition + new Vector2(laserLength_Start, 0);
        Vector2 bodyEndPosition = endPosition;
        Vector2 bodyPosition = bodyStartPosition;
        int bodyCount = 0;
        while (bodyPosition.x + laserLength_Body <= endPosition.x) {
            if (bodies.Count > bodyCount) {
                bodies[bodyCount].transform.position = bodyPosition;
                bodies[bodyCount].name = $"body[{bodyCount}]";
                bodyPosition.x += laserLength_Body;
                bodyCount++;
                continue;
            }
            GameObject newBody = GetBody();
            newBody.transform.position = bodyPosition;
            newBody.name = $"body[{bodyCount}]";
            bodyPosition.x += laserLength_Body;
            bodyCount++;
        }
        Debug.Log($"bodyPosition = {bodyPosition.x}, bodyEndPosition = {bodyEndPosition.x}");
        if (bodyPosition.x < bodyEndPosition.x) {
            Debug.Log($"bodies.Count = {bodies.Count}, bodyCount = {bodyCount}");
            GameObject lastBody;
            if (bodies.Count > bodyCount) lastBody = bodies[bodyCount];
            else lastBody = GetBody();
            bodyPosition.x = bodyEndPosition.x - laserLength_Body;
            lastBody.transform.position = bodyPosition;
            lastBody.name = $"body[{bodyCount}] [last]";
            bodyCount++;
        }
        else {
            GameObject lastBody = bodies[bodyCount - 1];
            lastBody.name = $"body[{bodyCount}] [last]";
        }
        for (int i = bodies.Count - 1; i >= bodyCount; i--) {
            GameObject surplusBody = bodies[i];
            bodies.Remove(surplusBody);
            deactiveBodies.Add(surplusBody);
            surplusBody.SetActive(false);
        }
    }

    private GameObject GetBody() {
        if (deactiveBodies.Count <= 0) {
            GameObject newBody = Instantiate(laserBody);
            newBody.transform.SetParent(this.transform);
            bodies.Add(newBody);
            return newBody;
        }
        GameObject body = deactiveBodies[0];
        deactiveBodies.Remove(body);
        bodies.Add(body);
        body.SetActive(true);
        return body;
        
    }
    


}