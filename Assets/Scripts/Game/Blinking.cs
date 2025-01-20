using UnityEngine;

public class Blinking : MonoBehaviour
{
    // マテリアルを取得するための変数
    private Material material;
    
    // 点滅スピードを設定
    public float blinkSpeed = 0.8f;
    
    // 透明度の変化値
    private float alphaValue = 1.0f;
    private bool isFading = false;

    void Start()
    {
        // オブジェクトのマテリアルを取得
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // 透明度を変更する
        if (isFading)
        {
            alphaValue -= Time.deltaTime * blinkSpeed;
            if (alphaValue <= 0.4f)
            {
                alphaValue = 0.4f;
                isFading = false;
            }
        }
        else
        {
            alphaValue += Time.deltaTime * blinkSpeed;
            if (alphaValue >= 1.0f)
            {
                alphaValue = 1.0f;
                isFading = true;
            }
        }

        // マテリアルの透明度を適用
        Color color = material.color;
        color.a = alphaValue;
        material.color = color;
    }

    public void StopBlinking(){
        Debug.Log("StopBlinking");
        Destroy(this);
    }
}