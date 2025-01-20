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

        /*if(startTime==0 && isReadyTimerSet)
        {
            //開始時刻を取得
            if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("startTime", out object startTimeValue)){
                startTime = (int)startTimeValue;
            }
        } */
        // 経過時間の計算
        elapsedTime = Mathf.Max(0f , (PhotonNetwork.ServerTimestamp - startTime) / 1000f);

        /*timer -= Time.deltaTime;
        if (timer < 0) timer = 0;*/
        // 時間制御
        //now += Time.deltaTime; // タイマー
        //Debug.Log(now);
        float t = elapsedTime / timer; // スライダーの値ー正規化
        timerSlider.value = Mathf.Lerp(1f, 0f, t);
        timeLimit = timer - elapsedTime; // 残り時間
        timeLimit = Mathf.Max(timeLimit, 0f);
        string timeLog = timeLimit.ToString("F0");
        timerText.text = timeLog;
        
        /*timerSlider.value = timer;
        timerText.text = Mathf.Ceil(timer).ToString(); // 残り秒数を表示*/
        
        fill.color = (timeLimit > 10.5f) ? greenColor : redColor; // スライダーの色（10.5秒以上は緑、未満は赤）
        timerText.color = (timeLimit > 10.5f) ? Color.white : Color.red; // 文字の色（10.5秒以上は黒、未満は赤）


        
        if(timeLimit <= 0)
        {
            Debug.Log("timeLimit："+timeLimit+"startTime："+startTime);
            // GameManagerのインスタンスを取得し、コルーチンを開始
            GameManager.Instance.ManageTurnTimer();
            isReadyTimerSet = false;
            startTime = 0;
            // タイマーを削除
            Destroy(gameObject);
            
        }
    }

    /*public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        // カスタムプロパティが更新されたときの処理
        if (propertiesThatChanged.ContainsKey("startTime"))
        {
            startTime = (int)propertiesThatChanged["startTime"];
            Debug.Log("StartTime updated: " + startTime);
            isReadyTimerSet = true;
        }
        

    }

    /*void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("<color=yellow>TimerSync.OnPhotonSerializeView</color>");
        if (stream.IsWriting)
        {
            stream.SendNext(timerSlider.value);
            stream.SendNext(timerText.text);
        }
        else
        {
            timerSlider.value = (float)stream.ReceiveNext();
            timerText.text = (string)stream.ReceiveNext();
            Debug.Log(now);
        }
    }*/
}
