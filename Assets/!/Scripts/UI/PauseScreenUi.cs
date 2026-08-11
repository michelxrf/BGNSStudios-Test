using UnityEngine;

public class PauseScreenUi : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _panel;
    
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
        Hide(true);
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
            _canvasGroup.alpha = 1f;
        }

        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        Cursor.lockState = CursorLockMode.None;
    }

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
            _canvasGroup.alpha = 0f;
        }
        

        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Quit()
    {
        Application.Quit();
    }

}