using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class weed_spawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject myPrefab;
    public int square_len = 100;
    public int weed_num = 100;
    public float weed_respawn_time = 5;

    // This script will simply instantiate the Prefab when the game starts.
    void Start()
    {
        for(int i =0; i<weed_num; i++) {
            make_weed();
        }

        InvokeRepeating("make_weed", 2.0f, weed_respawn_time);

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void make_weed()
    {
        var position = new Vector3(Random.Range(-square_len / 2, square_len / 2), 0.5f, Random.Range(-square_len / 2, square_len / 2));
        var weed = Instantiate(myPrefab, position, Quaternion.identity);
        weed.transform.localScale = new Vector3(0.3f, 1, 0.3f);
    }
}
