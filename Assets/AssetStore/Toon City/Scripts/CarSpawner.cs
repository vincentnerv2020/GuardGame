using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{

    [SerializeField] GameObject[] carPrefabs;
    [SerializeField] float startDelay;
    [SerializeField] float delay;
    
    [SerializeField] Transform placeToSpawn;
    // Start is called before the first frame update
    void Start()
    {
        startDelay = Random.Range(3, 10);
        delay = Random.Range(5  , 10);
        InvokeRepeating("SpawnCar", startDelay, delay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnCar()
    {
        startDelay = Random.Range(3, 10);
        delay = Random.Range(5, 10);
        CancelInvoke();
        InvokeRepeating("SpawnCar", startDelay, delay);
        GameObject NewCar = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)], placeToSpawn.position, placeToSpawn.rotation);
    }
}
