using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointTrigger : MonoBehaviour
{
    [SerializeField] private int checkPointIndex;

    internal int CheckPointIndex { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
    }
}
