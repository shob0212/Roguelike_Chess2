using UnityEngine;
using System.Collections;

public class BlinkingEffect : MonoBehaviour
{
    public float blinkDuration = 1.0f; // 点滅の周期（秒）
    public float minAlpha = 0.0f; // 最小アルファ値
    public float maxAlpha = 1.0f; // 最大アルファ値

    private SpriteRenderer objectRenderer;
    private Color originalColor;
    private bool isBlinking = false;

    void Start()
    {
        objectRenderer = GetComponent<SpriteRenderer>();
        if(objectRenderer!=null){
            Debug.Log("aaa");
        }
    }

    public void StartBlinking()
    {
        if (!isBlinking)
        {
            Debug.Log("StartBilinking　");
            this.gameObject.SetActive(true);
            StartCoroutine(SmoothBlink());
        }
    }

    public void StopBlinking()
    {
        if (isBlinking)
        {
            Debug.Log("StopBilinking　");
            StopCoroutine(SmoothBlink());
            this.gameObject.SetActive(false); // 点滅を停止したら元の色に戻す
            isBlinking = false;
        }
    }

    private IEnumerator SmoothBlink()
    {
        isBlinking = true; int value = 244; bool increasing = true;
        while (isBlinking)
        {
            while (true) {  
                if (increasing) { 
                    value+=1; 
                    GetComponent<SpriteRenderer>().color = new Color(235/255f,(byte)value/255f,75/255f,(byte)value/255f);
                    if (value >= 245) { 
                        value = 245;
                        yield return new WaitForSeconds(1f);
                        increasing = false;
                    } 
                } else { 
                    value-=1; 
                    GetComponent<SpriteRenderer>().color = new Color(235/255f,(byte)value/255f,75/255f,(byte)value/255f);
                    if (value <= 220) { 
                        value = 220;
                        GetComponent<SpriteRenderer>().color = new Color(235/255f,(byte)value/255f,75/255f,20/255f);
                        yield return new WaitForSeconds(0.5f);
                        increasing = true;
                        
                    } 
                }
                yield return null;
            }
        }
        
    }
}
