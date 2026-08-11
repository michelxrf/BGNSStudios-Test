using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Handles the pause screen UI
/// </summary>
public class PauseScreenUi : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private IntroFade _fader;

    private CanvasGroup _canvasGroup;
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Hide(true);
    }


    /// <summary>
    /// Pauses the game and shows the pause screen
    /// </summary>
    public void Pause()
    {
        PauseManager.instance.PauseGame();
        Show();
    }

    /// <summary>
    /// Resumes the game and hides the pause screen
    /// </summary>
    public void Resume()
    {
        PauseManager.instance.ResumeGame();
        Hide();
    }

    /// <summary>
    /// Listens for the pause input and toggles the pause screen
    /// </summary>
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1)) {
            if(PauseManager.instance.IsPaused)
            {
                Resume();
            } 
            else
            { 
                Pause();
            }
        }
    }

    /// <summary>
    /// Shows the pause screen
    /// </summary>
    /// <param name="skipAnimation">Use to skip the popup animations, for initial setup</param>
    private void Show(bool skipAnimation = false)
    {
        if(!skipAnimation)
        {
            // popup effect
            _panel.transform.localScale = Vector3.zero;
            LeanTween.alphaCanvas(_canvasGroup, 1f, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.scale(_panel, Vector3.one, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            ;
        }
        else
        {
            // just show the panel without any animations
            _canvasGroup.alpha = 1f;
        }

        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        // allow the cursor to move
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// Hides the pause screen
    /// </summary>
    /// <param name="skipAnimation">Use to skip the popup animations, for initial setup</param>
    private void Hide(bool skipAnimation = false)
    {
        if(!skipAnimation)
        {
            // popup effect
            _panel.transform.localScale = Vector3.one;
            LeanTween.alphaCanvas(_canvasGroup, 0f, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.scale(_panel, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        }
        else
        {
            // just hide the panel without any animations
            _canvasGroup.alpha = 0f;
        }

        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        // lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void Quit()
    {
        
        StartCoroutine(TransitionToScene("MainMenu"));
    }

    IEnumerator TransitionToScene(string sceneName)
    {
        _fader.FadeToBlack();
        yield return new WaitForSeconds(_fader._fadeDuration);
        SceneManager.LoadScene(sceneName);
    }
}