using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    // SINGLETON
    // Allows access a Specific Instance of  this class from anywhere in the prj while
    // also insuring that there is only 1 instance in existance.

    public static WaveManager instance;

    public float amplitude = 1f;
    public float length = 2f;
    public float speed = 1f;
    public float offset = 0f;



    private void Awake()
    {
        if (instance ==null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists. Destroying Obj.");

        }

    }

    private void Update()
    {
        offset += Time.deltaTime * speed;
    }


    // return height at X coord
    public float GetWaveHeight(float _x)
    {
        return amplitude * Mathf.Sin(_x / length + offset);
    }


}
 