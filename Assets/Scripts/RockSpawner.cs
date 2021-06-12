using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    
    public GameObject myPrefab;
    public int squareLen = 100;
    public int rockNum = 10;
    public float rockRespawnTime = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < rockNum; i++) {
            MakeRock();
        }

        InvokeRepeating("MakeRock", 2.0f, rockRespawnTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MakeRock()
    {
        var position = new Vector3(Random.Range(-squareLen / 2, squareLen / 2), 0.0f, Random.Range(-squareLen / 2, squareLen / 2));
        var weed = Instantiate(myPrefab, position, Quaternion.identity);
        weed.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}
