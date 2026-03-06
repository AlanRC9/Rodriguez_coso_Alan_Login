using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChageSceneButton : MonoBehaviour
{
    [SerializeField] private string GoToSceneName;

    public void GoToScene() => SceneManager.LoadScene(GoToSceneName);
}
