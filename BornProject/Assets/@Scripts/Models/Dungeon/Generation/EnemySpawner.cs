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
                Key = random.Next(2) == 0 ? "Snake" : "Wolf";
                break;
            case 2:
                randomNumber = random.NextDouble();
                if (randomNumber < 0.20)
                    Key = "Snake";
                else if (randomNumber < 0.50)
                    Key = "Wolf";
                else
                    Key = "Bear";
                break;
            case 3:
                randomNumber = random.NextDouble();
                if (randomNumber < 0.20)
                    Key = "Snake";
                else if (randomNumber < 0.50)
                    Key = "Wolf";
                else
                    Key = "Bear";
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