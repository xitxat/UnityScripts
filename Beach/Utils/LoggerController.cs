using UnityEngine;

//  Attach this script to a logical GameObject in  scene

public class LoggerController : MonoBehaviour
{
    public bool logEnabled = false;

    private void Awake()
    {
        Debug.unityLogger.logEnabled = logEnabled;
    }
}