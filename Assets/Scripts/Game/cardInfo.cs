using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Info;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Info.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Info.SetActive(false);
    }
}
