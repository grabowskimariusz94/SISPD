using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject myPrefab;
    //public int square_len = 100;
    public static int fieldX = 100;
    public static int fieldY = 150;
    public static int spacingX = 4;
    public static int spacingY = 2;
    //public int plant_num = 1000;
    public static int rows = fieldX/spacingX;
    public static int rowLen = fieldY/spacingY;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = -rows/2; i<rows/2; i++) {
            for (int j = -rowLen/2+i; j<rowLen/2-i; j++) {
                MakePlant(i*spacingX+j*j/150, j*spacingY);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MakePlant(float x, float z)
    {
        //int x = Random.Range(-square_len / 2, square_len / 2);
        //int z = Random.Range(-square_len / 2, square_len / 2);
        var position = new Vector3(x, 0, z);
        var plant = Instantiate(myPrefab, position, Quaternion.identity);
    }
}
