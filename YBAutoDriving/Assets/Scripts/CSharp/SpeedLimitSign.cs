using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLimitSign : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // This function is called when the GameObject starts colliding with another GameObject
    private void OnCollisionEnter(Collision collision)
    {
        // Print the name of the object the GameObject starts colliding with
        Debug.Log(gameObject.name + " has collided with " + collision.gameObject.name);
    }

    // This function is called when the GameObject stops colliding with another GameObject
    private void OnCollisionExit(Collision collision)
    {
        // Print the name of the object the GameObject stopped colliding with
        Debug.Log(gameObject.name + " has stopped colliding with " + collision.gameObject.name);
    }

    public void GetReward() { }
    public void GetPunish() { }

    public void Stop() { }
}
