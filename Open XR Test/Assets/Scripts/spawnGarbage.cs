using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnGarbage : MonoBehaviour
{

    public GameObject garbage1;
    public GameObject garbage2;
    public GameObject garbage3;
    public GameObject garbage4;
    public int sodacanCount = 2;
    public int glassbottleCount = 3;
    public int plasticbottleCount = 3;

    // Start is called before the first frame update

    public void createGarbage(){
        Debug.Log("Spawn Garbage");

        for (int i = 0; i < glassbottleCount; i++)
{
            Vector3 randomPosition = new Vector3(
                Random.Range(435, 524), 
                33, 
                Random.Range(-609, -576) 
            );

            Vector3 spawnPosition = transform.position + randomPosition;

            Instantiate(garbage1, spawnPosition, Quaternion.identity);
        }

        
        for (int i = 0; i < sodacanCount; i++)
{
            Vector3 randomPosition = new Vector3(
                Random.Range(435, 524), 
                33, 
                Random.Range(-609, -576) 
            );

            Vector3 spawnPosition = transform.position + randomPosition;

            Instantiate(garbage2, spawnPosition, Quaternion.identity);
        }

        
        for (int i = 0; i < plasticbottleCount; i++)
{
            Vector3 randomPosition = new Vector3(
                Random.Range(435, 524), 
                33, 
                Random.Range(-609, -576) 
            );

            Vector3 spawnPosition = transform.position + randomPosition;

            Instantiate(garbage3, spawnPosition, Quaternion.identity);
        }
        for (int i = 0; i < plasticbottleCount; i++)
{
            Vector3 randomPosition = new Vector3(
                Random.Range(435, 524), 
                33, 
                Random.Range(-609, -576) 
            );

            Vector3 spawnPosition = transform.position + randomPosition;

            Instantiate(garbage4, spawnPosition, Quaternion.identity);
        }

    }





}
