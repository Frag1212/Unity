using UnityEngine;
using System.Collections;

public class GameRulesScript : MonoBehaviour
{
    int NextEnemyNumber = 1;
    public GameObject EnemyPrefab1;
    public GameObject EnemyPrefab2;
    float TimeUntilSpawn = float.MaxValue;
    /*GameObject player;
    float SpawnDeltaTime = 5f;
    float TimeUntilNextSpawn;*/
    //float Time = 0;

    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        //TimeUntilNextSpawn = SpawnDeltaTime;
        if (EnemyPrefab1 == null || EnemyPrefab2 == null)
            Destroy(gameObject);
    }
    void Update ()
    {
        return;//lala
        if(TimeUntilSpawn < float.MaxValue)
            TimeUntilSpawn -= Time.deltaTime;
        EnemyWalkingScript[] ews = GameObject.FindObjectsOfType<EnemyWalkingScript>();
        EnemyScript[] es = GameObject.FindObjectsOfType<EnemyScript>();
        if (ews.Length == 0 && es.Length == 0 && TimeUntilSpawn == float.MaxValue)
        {
            TimeUntilSpawn = 5;
        }
        if (TimeUntilSpawn <= 0)
        {
            TimeUntilSpawn = float.MaxValue;
            for (int i = 0; i < NextEnemyNumber; i++)
            {
                int x = (i + 1)/2;
                if (i % 2 != 0)
                    x = -x;
                x *= 5;
                int z = 20;
                if (EnemyPrefab1 == null)
                {
                    if (EnemyPrefab2 == null)
                        Destroy(gameObject);
                    else
                        Instantiate(EnemyPrefab2, new Vector3(x, 5, z), Quaternion.identity);
                }
                else
                {
                    if (EnemyPrefab2 == null)
                        Instantiate(EnemyPrefab1, new Vector3(x, 5, z), Quaternion.identity);
                    else
                    {
                        if (Random.Range(0, 2) == 0)
                            Instantiate(EnemyPrefab1, new Vector3(x, 5, z), Quaternion.identity);
                        else
                            Instantiate(EnemyPrefab2, new Vector3(x, 5, z), Quaternion.identity);
                    }
                }
            }
            NextEnemyNumber++;
        }
    }
}
