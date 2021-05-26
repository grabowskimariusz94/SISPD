using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plant_spawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject myPrefab;
    public int square_len = 100;
    public int plant_num = 1000;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < plant_num; i++)
        {
            make_plant();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void make_plant()
    {
        int x = Random.Range(-square_len / 2, square_len / 2);
        int y = Random.Range(-square_len / 2, square_len / 2);
        var position = new Vector3(x, 0, y);
        var plant = Instantiate(myPrefab, position, Quaternion.identity);
    }
}
