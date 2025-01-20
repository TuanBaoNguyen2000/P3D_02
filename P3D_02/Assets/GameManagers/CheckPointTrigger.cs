using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointTrigger : MonoBehaviour
{
    [SerializeField] private int checkPointIndex;

    internal int Index { get; private set; }
    internal Vector3 Position { get => this.transform.position; }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        IRacerInformation racer = other.GetComponent<IRacerInformation>();

        if (racer != null )
            racer.Information.currentCheckpoint = checkPointIndex;
    }
}
