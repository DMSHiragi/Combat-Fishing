using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] 
    private GameObject enemyPrefab;

    [SerializeField] 
    private float enemyInterval = 3.5f;

    [SerializeField] 
    private Transform spawnPosition;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawnEnemy(enemyInterval, enemyPrefab));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator spawnEnemy(float interval, GameObject enemy){
        yield return new WaitForSeconds(interval);
        GameObject newEnemy = Instantiate(enemy, spawnPosition.position, Quaternion.identity);
        StartCoroutine(spawnEnemy(interval, enemy));
    }
}
