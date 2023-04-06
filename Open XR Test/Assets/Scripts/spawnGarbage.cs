using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnGarbage : MonoBehaviour
{

    public GameObject garbage1;
    public GameObject garbage2;
    public GameObject garbage3;
    public GameObject garbage4;
    private int garbageCount;

    // Start is called before the first frame update

    public void createGarbage(){
        Debug.Log("Spawn Garbage");

        int objectCount = CountObjectsWithWeaponTag();

        if(objectCount < 6){
            garbageCount = 2;
        }
        if(objectCount < 10){
            garbageCount = 1;
        }
        else{
            garbageCount = 0;
        }

        for (int i = 0; i < garbageCount; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(435, 524), 
                33, 
                Random.Range(-609, -576) 
            );

            Vector3 spawnPosition = transform.position + randomPosition;

            Instantiate(garbage1, spawnPosition, Quaternion.identity);
        }

        
        for (int i = 0; i < garbageCount; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(435, 524), 
                33, 
                Random.Range(-609, -576) 
            );

            Vector3 spawnPosition = transform.position + randomPosition;

            Instantiate(garbage2, spawnPosition, Quaternion.identity);
        }

        
        for (int i = 0; i < garbageCount; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(435, 524), 
                33, 
                Random.Range(-609, -576) 
            );

            Vector3 spawnPosition = transform.position + randomPosition;

            Instantiate(garbage3, spawnPosition, Quaternion.identity);
        }
        for (int i = 0; i < garbageCount; i++)
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


    private int CountObjectsWithWeaponTag()
    {
        // Find all objects with the "Weapon" tag
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Weapon");

        // Return the number of objects found
        return objectsWithTag.Length;
    }



}
