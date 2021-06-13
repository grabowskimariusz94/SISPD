using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class WeedSpawner : MonoBehaviour
{
    public GameObject myPrefab;
    public int squareLen = 100;
    public int weedNum = 100;
    public float weedRespawnTime = 5;

    // Start is called before the first frame update
    void Start()
    {
        for(int i =0; i<weedNum; i++) {
            makeWeed();
        }

        // InvokeRepeating("makeWeed", 2.0f, weedRespawnTime);

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void makeWeed()
    {
        var position = new Vector3(Random.Range(-squareLen / 2, squareLen / 2), 0.5f, Random.Range(-squareLen / 2, squareLen / 2));
        var weed = Instantiate(myPrefab, position, Quaternion.identity);
        weed.transform.localScale = new Vector3(0.3f, 1, 0.3f);
    }
}
