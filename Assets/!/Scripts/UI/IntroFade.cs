using UnityEngine;

/// <summary>
/// User for smoother transitions between scenes, fading in and out of black
/// </summary>
public class IntroFade : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] public float _fadeDuration = .3f;
    [SerializeField] private float _startingAlpha = 1f;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = _startingAlpha;
    }

    private void Start()
    {
        if(_startingAlpha == 1f)
        {
            FadeFromBlack();
        }
        else
        {
            FadeToBlack();
        }
    }

    public void FadeToBlack()
    {
        _canvasGroup.alpha = 0f;
        LeanTween.alphaCanvas(_canvasGroup, 1f, _fadeDuration).setEaseInOutQuad();
    }

    public void FadeFromBlack()
    {
        _canvasGroup.alpha = 1f;
        LeanTween.alphaCanvas(_canvasGroup, 0f, _fadeDuration).setEaseInOutQuad();
    }
}
