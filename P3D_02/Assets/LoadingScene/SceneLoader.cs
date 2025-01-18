using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader 
{
    public static string TargetScene; 

    public static void LoadScene(string sceneName)
    {
        TargetScene = sceneName;
        SceneManager.LoadScene("LoadingScene"); 
    }
}
