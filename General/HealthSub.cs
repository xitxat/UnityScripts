using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SUBTRACT HEALTH
/// attach to obj to be destroyed
/// adjust Damage insp var on Gun
/// 
/// </summary>




public class HealthSub : MonoBehaviour
{
    public float Health = 10f;

    public void SubtractHealth(float Damage)
    {
        Health -= Damage;

        if (Health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}