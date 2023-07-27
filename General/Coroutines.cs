using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Unity C# Basics P5 Coroutines
///  Coroutines act like a delay
///  Instead of multi threading
///  
/// </summary>

public class Coroutines : MonoBehaviour
{
    public bool Run;                            // WaitUntil

    // Start is called before the first frame update
    void Start()
    {
        // call the delay(name)

        //Time.timeScale = 5f;                  // WaitForSecondsRealtime

        //StartCoroutine(MyCoroutine());
        //StartCoroutine(mWaitFS());
        //StartCoroutine(mRealT());

        StartCoroutine(mWaitUt());

    }



    IEnumerator MyCoroutine()                   // pause for 1 frame
    {
        Debug.Log(Time.frameCount);
        yield return null;                      //  1 frame
        Debug.Log(Time.frameCount);

    }



    IEnumerator mWaitFS()                       // WaitForSerconds takes float
    {
        Debug.Log(Time.time);
        yield return new WaitForSeconds(3f);              
        Debug.Log(Time.time);

    }

    IEnumerator mRealT()                       // real time Timescale bypass
    {
        Debug.Log(Time.time);
        yield return new WaitForSecondsRealtime(3f);
        Debug.Log(Time.time);

    }

    IEnumerator mWaitUt()                       // Wait until   Check Var in inspector
    {
        Debug.Log("Run is False");
        yield return new WaitUntil(() => Run == true);     // Predicate. =>  (Lamda expression undeclared Fun.)?
        Debug.Log("Run is True");                              // A condition to check
                                                           // Wait Until Run equals true

    }

    // Update is called once per frame
    void Update()
    {
        
    }


} // end class