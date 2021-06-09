using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidCollision : MonoBehaviour
{
    // This is our direction we're travelling in.
    public Vector3 direction = new Vector3(0, 0, 1);
    public float velocity = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * velocity * Time.deltaTime);
    }

    // If we hit a left or right wall, invert x direction.
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name == "FencePanel")
        {
            direction.z *= -1;
        }      
    }
}
