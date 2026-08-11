using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private IntroFade _fader;
    public void TransitionToGame()
    {
        StartCoroutine(TransitionToScene("MainScene"));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator TransitionToScene(string sceneName)
    {
        _fader.FadeToBlack();
        yield return new WaitForSeconds(_fader._fadeDuration);
        SceneManager.LoadScene(sceneName);
    }
}
