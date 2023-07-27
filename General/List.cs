 using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// LISTS are dynamically sized
/// 
/// Rytech P6
/// https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?redirectedfrom=MSDN&view=net-6.0
/// </summary>
public class List : MonoBehaviour
{
    public List<int> mList;

    
    void Start()
    {
        mList = new List<int>();  // empty    
   
        mList.Add(10);   
        mList.Add(112);
        mList.Add(21);
        mList.Add(31);

        Debug.Log("List for ");

        // array - length
        // list - count
        // for loops are much faster compared to foreach

        for (int i =0; i<mList.Count; i++)
        {
            Debug.Log(mList[i]);
        }


        Debug.Log("List foreach ");

        foreach (int mNumber in mList)
        {
            Debug.Log(mNumber);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
