using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void SceneLoad()
    {
        SceneManager.LoadScene(1);
    }

    public void EndLoad()
    {
        SceneManager.LoadScene(2);
    }
}
