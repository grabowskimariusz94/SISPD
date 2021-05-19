using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Test 1, 2, 3");
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, GetComponent<Rigidbody>().velocity.y, 10);
    }

    private void OnTriggerEnter(Collider other){
        if (other.gameObject.layer==6) Destroy(other.gameObject);
    }
}
