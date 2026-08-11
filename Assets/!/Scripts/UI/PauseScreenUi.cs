using UnityEngine;

public class PauseScreenUi : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    public void Pause()
    {
        PauseManager.instance.PauseGame();
        Show();
    }

    public void Resume()
    {
        PauseManager.instance.ResumeGame();
        Hide();
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }

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

    private void Show()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Hide()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Quit()
    {
        Application.Quit();
    }

}