using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

public class ModelTargetController : MonoBehaviour
{

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

 
}
