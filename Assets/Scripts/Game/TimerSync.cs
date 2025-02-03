using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using TMPro;
using Photon.Pun;

public class TimerSync : MonoBehaviourPunCallbacks
{
    public Slider timerSlider;
    public Image fill;
    public TextMeshProUGUI timerText; // 残り秒数を表示するテキスト
    public float timer = 30f; // 30秒の制限時間
    private Color greenColor;
    private Color redColor;
    private int startTime;
    private float timeLimit;
    private float elapsedTime;
    private bool isReadyTimerSet = false;



    void Start()
    {
        Debug.Log("<color=yellow>TimerSync.Start</color>");

        startTime = PhotonNetwork.ServerTimestamp;
        isReadyTimerSet = true;

        ColorUtility.TryParseHtmlString("#57E919", out greenColor); // 緑色
        ColorUtility.TryParseHtmlString("#B30100", out redColor); // 赤色
    }

    void Update()
    { 
        if(!isReadyTimerSet) return;

        // 経過時間の計算
        elapsedTime = Mathf.Max(0f , (PhotonNetwork.ServerTimestamp - startTime) / 1000f);

        float t = elapsedTime / timer; // スライダーの値ー正規化
        timerSlider.value = Mathf.Lerp(1f, 0f, t);
        timeLimit = timer - elapsedTime; // 残り時間
        timeLimit = Mathf.Max(timeLimit, 0f);
        string timeLog = timeLimit.ToString("F0");
        timerText.text = timeLog;
        
        
        fill.color = (timeLimit > 10.5f) ? greenColor : redColor; // スライダーの色（10.5秒以上は緑、未満は赤）
        timerText.color = (timeLimit > 10.5f) ? Color.white : Color.red; // 文字の色（10.5秒以上は黒、未満は赤）


        
        if(timeLimit <= 0)
        {
            Debug.Log("timeLimit："+timeLimit+"startTime："+startTime);
            // GameManagerのインスタンスを取得し、コルーチンを開始
            GameManager.Instance.FinishPhaseReady();
            isReadyTimerSet = false;
            startTime = 0;
            // タイマーを削除
            Destroy(gameObject);
            
        }
    }


}
