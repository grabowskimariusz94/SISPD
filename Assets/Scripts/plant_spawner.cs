using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plant_spawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject myPrefab;
    //public int square_len = 100;
    public static int field_x = 80;
    public static int field_y = 190;
    public static int spacing_x = 4;
    public static int spacing_y = 2;
    //public int plant_num = 1000;
    public static int rows = field_x/spacing_x;
    public static int row_len = field_y/spacing_y;
    
    // Start is called before the first frame update
    void Start()
    {
        /*for (int i = -rows/2; i<rows/2; i++) {
            for (int j = -row_len/2; j<row_len/2; j++) {
                make_plant(i*spacing_x, j*spacing_y);
            }
        }*/
        for (int i = -rows/2; i<rows/2; i++) {
            for (int j = -row_len/2; j<row_len/2; j++) {
                make_plant(i*spacing_x+j*j/150-5, j*spacing_y);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void make_plant(float x, float z)
    {
        //int x = Random.Range(-square_len / 2, square_len / 2);
        //int z = Random.Range(-square_len / 2, square_len / 2);
        var position = new Vector3(x, 0, z);
        var plant = Instantiate(myPrefab, position, Quaternion.identity);
    }
}
