using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

/// <summary>
/// Simple script to fade an Ui popup and destroy it afterwards
/// </summary>
public class ItemUsedPopup : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _fadeDelay = 1f;

    private TMP_Text _text;
    private CanvasGroup _canvasGroup;
    
    private void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;      
    }

    /// <summary>
    /// called when the popup is created, starts the fade out process
    /// </summary>
    public void Show(string itemName)
    {
        _text.text = $"{itemName} was used!";
        _canvasGroup.alpha = 1f;
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(_fadeDelay);

        float elapsedTime = 0f;
        while (elapsedTime < _fadeDuration)
        {
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / _fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
