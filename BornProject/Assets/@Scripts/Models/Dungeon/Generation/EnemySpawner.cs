using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    //#region Inspector

    ////[SerializeField]
    ////private string enemyKey;

    //#endregion

    //#region Properties

    //public string Key => enemyKey;

    #region Fields

    public string Key;
    private int case1Counter = 0;
    private string case2Selection = null;
    public Vector2 Position => this.transform.position;

    #endregion
    private void Awake()
    {
        SpawnEnemyType(Main.Quest.ClearStageCount);
    }
    public void SpawnEnemyType(int stage)
    {

        System.Random random = new System.Random();
        double randomNumber;
        
        switch (stage)
        {
            case 0:
                randomNumber = random.NextDouble(); 
                Key = randomNumber < 0.67 ? "Beatle" : "Snake";
                break;
            case 1:
                if (case1Counter == 0 || case1Counter == 1) Key = "Beatle";
                else if (case1Counter == 2) Key = "Wolf";
                case1Counter = (case1Counter + 1) % 3;
                break;
            case 2:
                if (case2Selection == null)
                {
                    case2Selection = random.Next(2) == 0 ? "Snake" : "Wolf";
                }
                Key = case2Selection;
                break;
            default:
                case2Selection = null;
                break;
            case 3:
                Key = random.Next(2) == 0 ? "Snake" : "Bear";
                break;
            case 4:
                randomNumber = random.NextDouble();
                if (randomNumber < 0.20)
                    Key = "Snake";
                else if (randomNumber < 0.50)
                    Key = "Wolf";
                else
                    Key = "Bear";
                break;
        }
    }

}