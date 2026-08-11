using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _time = 0.1f;
    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, Vector3.one * _hoverScale, _time).setEaseInOutQuad();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, Vector3.one, _time).setEaseInOutQuad();
    }
}
