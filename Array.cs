using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrays : MonoBehaviour
{

    public int[] Numbers;



    // Start is called before the first frame update
    void Start()
    {
     // init array w. 3 elements
     Numbers = new int[3];

        Numbers[0] = 1;
        Numbers[1] = 2; 
        Numbers[2] = 3;

        // create pointer
        // assign array position to var i 
        // debug with  loop,
        for (int i = 0; i < Numbers.Length; i++)
        {
            Debug.Log(Numbers[i]);
        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
