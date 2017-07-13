using UnityEngine;
using System.Collections;

// In this example we show how to invoke a coroutine and continue executing
// the function in parallel.

public class IMGUI : MonoBehaviour
{
    void FixedUpdate()
    {
        Debug.Log("FixedUpdate time :" + Time.deltaTime);
    }


    void Update()
    {
        Debug.Log("Update time :" + Time.deltaTime);
    }
}