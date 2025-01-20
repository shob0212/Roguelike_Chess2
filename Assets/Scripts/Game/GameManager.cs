using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Lobby;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using System.Linq;


public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Camera sceneCamera;  // シーンのカメラ
    [SerializeField] private GameObject playerPrefab; // プレイヤーのプレハブ
    [SerializeField] private GameObject playerNamePlate;  // プレイヤーのネームプレート
    [SerializeField] private GameObject payerNameTilePrefab;  // プレイヤー名プレートのプレハブ
    [SerializeField] private GameObject playerUI; // プレイヤーのUI
    [SerializeField] private GameObject timerPrefab;  // タイマーのプレハブ
    [SerializeField] private GameObject actionPhaseManagerPrefab;  // アクションフェーズマネージャーのプレハブ
    [SerializeField] private GameObject movePhaseManagerPrefab;  // 移動フェーズマネージャーのプレハブ
    [SerializeField] private TMP_Text RoundNum;  // ラウンド数を表示するUI
    [SerializeField] private TMP_Text phaseText;  // フェーズを表示するUI
    [SerializeField] private GameObject cardDrawPrefab;  // カードを引くUIのプレハブ
    private GameObject player;  // プレイヤーのインスタンス
    private List<int> playerTurnOrders = new List<int>();   // プレイヤーの行動順
    private GameObject[] playerPlates = new GameObject[4];   // プレイヤー名プレートのインスタンスを格納する配列
    private System.Random rdm = new System.Random();
    private int roundCount = 1;  // 現在のラウンド数を格納
    public int phase;  // 現在のフェーズ
    private int isPhaseReady;    // ターンが終了したかどうか
    private Coroutine startPhaseCoroutine;    // プレイヤーの行動を管理するコルーチンの参照
    public int turnPlayerNum = 1;   // 現在のプレイヤーの番号
    private bool isFirstPhase = true;  // 最初のフェーズかどうか
    


    void Awake()
    {
        // インスタンスが存在しない場合は設定し、存在する場合は破棄する
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        Debug.Log("<color=yellow>GM.Start</color>");

        //----------------------ターンをランダムな順番で設定-------------------------------------

        if( PhotonNetwork.IsMasterClient)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                playerTurnOrders.Add(player.ActorNumber);
            }

            int n = playerTurnOrders.Count;
            while (n > 0) {
                n--;
                int k = rdm.Next(n);
                int value = playerTurnOrders[k];
                playerTurnOrders[k] = playerTurnOrders[n];
                playerTurnOrders[n] = value;
            }

            // int型のリストをカンマ区切りの文字列に変換
            string turnOrderString = string.Join(",", playerTurnOrders.ConvertAll(i => i.ToString()).ToArray());
            photonView.RPC("SyncPlayerTurnOrder", RpcTarget.AllBuffered, turnOrderString);
        }
        //-----------------------------------------------------------------------------------------
    }

    void setPlayerObject()
    {
        int rotation=0;
        int vectorX=0;
        int vectorY=0;
        int camX=7;
        int camY=4;
        //PlayerInfo.playerId = 4;
        if(PlayerInfo.playerId == 2){

            rotation = 180;  vectorX = 14;  vectorY = 14;  camX = 7;  camY = 10;

        }else if(PlayerInfo.playerId == 3){

            rotation = 90;  vectorX = 14;  vectorY = 0;  camX = 10;  camY = 7;

        }else if(PlayerInfo.playerId == 4){

            rotation = 270;  vectorX = 0;  vectorY = 14;  camX = 4;  camY = 7;

        }
        sceneCamera.transform.rotation = Quaternion.Euler(0, 0, rotation);
        sceneCamera.transform.position = new Vector3(camX, camY, -10);
        player = PhotonNetwork.Instantiate("Avatar"+PlayerInfo.playerTurnOrder, new Vector3(vectorX, vectorY, 0f), Quaternion.identity, 0);
        player.GetPhotonView().RPC("SetName", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);

        // マスタークライアントのみラウンド管理を開始
        if(PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(RoundManager());
        }
    }



    [PunRPC]
    void SyncPlayerTurnOrder(string playerTurnOrderString)
    {
        Debug.Log("<color=yellow>GM.SyncPlayerTurnOrder</color>");
        Debug.Log("Player: " + playerTurnOrderString);

        // カンマ区切りの文字列をint型の配列に変換
        int[] playerTurnOrders = Array.ConvertAll(playerTurnOrderString.Split(','), int.Parse);
        int index = 0;
        foreach (int playerNum in playerTurnOrders)
        {
            index++;
            GameObject Plate = Instantiate(payerNameTilePrefab, playerNamePlate.transform);
            Plate.transform.localPosition = new Vector3(-14f, (float)13-index*3, 0f);
            Plate.GetComponentInChildren<Canvas>().GetComponentInChildren<TMP_Text>().text = PhotonNetwork.PlayerList.FirstOrDefault(p => p.ActorNumber == playerNum).NickName;
            SpriteRenderer spriteRenderer = Plate.transform.Find("Background Player Color").GetComponent<SpriteRenderer>();
            Color color;
            switch (index)
            {
                case 2:
                    ColorUtility.TryParseHtmlString("#FFB600", out color);
                    spriteRenderer.color = color;
                    break;
                case 3:
                    ColorUtility.TryParseHtmlString("#37DA8C", out color);
                    spriteRenderer.color = color;
                    break;
                case 4:
                    ColorUtility.TryParseHtmlString("#9179FF", out color);
                    spriteRenderer.color = color;
                    break;
                default:
                    break;
            }
            playerPlates[index-1] = Plate;

            if(playerNum == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                PlayerInfo.playerTurnOrder = index;
            }
        }
        Debug.Log("PlayerTurnOrder: " + PlayerInfo.playerTurnOrder);
        setPlayerObject();
    }

    
    IEnumerator RoundManager()
    {
        Debug.Log("<color=red>GM.TurnManager</color>");
        while(true)
        {

            for(int phase = 1; phase <= 2; phase++)
            {
                
                isPhaseReady = 0;
                photonView.RPC("StartPhase", RpcTarget.AllBuffered, phase);

                // 全員の行動フェーズが終わるまでるまで待機
                Debug.Log("最初のisPhaseReady"+isPhaseReady);
                yield return new WaitUntil(() => (isPhaseReady>=PhotonNetwork.PlayerList.Length));
                Debug.Log("最後のisPhaseReady"+isPhaseReady);
            }
        }
    }



    [PunRPC]
    void StartPhase(int phase)
    {
        this.phase = phase;
        Debug.Log("<color=yellow>GM.StartPhase</color>");
        if(isFirstPhase){
            GameObject drawPC = Instantiate(cardDrawPrefab);
            drawPC.GetComponent<CardManager>().Draw(1, isFirstPhase);
            GameObject drawSC = Instantiate(cardDrawPrefab);
            drawSC.GetComponent<CardManager>().Draw(2, isFirstPhase);
            isFirstPhase = false;
            Instantiate(movePhaseManagerPrefab);
        }
        else{
            //移動フェーズかアクションフェーズかを判定
            if(phase == 1){
                Debug.Log("<color=green>roundCount++</color>");
                roundCount++;
                RoundNum.text = roundCount.ToString();

                phaseText.text = "移動フェーズ";
                GameObject drawPC = Instantiate(cardDrawPrefab);
                drawPC.GetComponent<CardManager>().Draw(1, isFirstPhase);
                Instantiate(movePhaseManagerPrefab);
            }else{
                phaseText.text = "アクションフェーズ";
                GameObject drawSC = Instantiate(cardDrawPrefab);
                drawSC.GetComponent<CardManager>().Draw(2, isFirstPhase);
                Instantiate(actionPhaseManagerPrefab);
            }
        }
            
        
    }




    public void ManageTurnTimer()
    {
        Debug.Log("<color=yellow>GM.ManageTurnTimer</color>");

        // 制限時間が切れたら各フェーズを強制終了
        Destroy(GameObject.Find("Move Phase Manager(Clone)"));
        Destroy(GameObject.Find("Action Phase Manager(Clone)"));

        isPhaseReady++;
        Debug.Log("isPhaseReady++");

        photonView.RPC("UpdatePhaseReady", RpcTarget.AllBuffered, isPhaseReady);   // フェーズ終了を同期
    }

    [PunRPC]
    void UpdatePhaseReady(int newPhaseReady)
    {
        isPhaseReady = newPhaseReady;
        Debug.Log("Updated isPhaseReady: " + isPhaseReady);
    }

    public void setTimerObject()
    {
        Instantiate(timerPrefab, playerUI.transform);
    }
    

    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("<color=yellow>GM.OnPhotonSerializeView</color>");
        if (stream.IsWriting)
        {
            // データの送信
            stream.SendNext(roundCount);
            Debug.Log("isPhaseReady: " + isPhaseReady);
        }
        else
        {
            // データの受信
            roundCount = (int)stream.ReceiveNext();
            Debug.Log("isPhaseReady: " + isPhaseReady);
        }
    }*/
    public void LeaveRoom()
    {
        Debug.Log(("<color=yellow>GM.LeaveRoom</color>"));
        PhotonNetwork.LeaveRoom();
        
    }

    public override void OnLeftRoom()
    {
        Debug.Log(("<color=yellow>GM.OnLeftRoom</color>"));
        SceneManager.LoadScene(0);
    }


}