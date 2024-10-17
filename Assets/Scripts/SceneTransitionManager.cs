using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public MeshSceneFade sceneFadeMeshObject;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void LoadScene(int sceneNumber)
    {
        /*  Scene Numbers:
         *  0: Main Menu
         *  1: Game Finish
         *  2: Simple Level
        */

        StartCoroutine(GoToSceneRoutine(sceneNumber));

    }

    IEnumerator GoToSceneRoutine(int sceneIndex)
    {
        sceneFadeMeshObject.FadeOut();
        yield return new WaitForSeconds(sceneFadeMeshObject.fadeDuration);

        //Launch the new scene
        SceneManager.LoadScene(sceneIndex);

        sceneFadeMeshObject.FadeIn();
        //yield return new WaitForSeconds(sceneFadeMeshObject.fadeDuration);

    }


    /// <summary>
    /// Closes the game
    /// </summary>
    public void ExitProgram()
    {
        Application.Quit();
    }

}