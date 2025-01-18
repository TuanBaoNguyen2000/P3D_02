using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHandleCarInput 
{
    bool EnableControl { get; set; }

    void HandleInput();
}
