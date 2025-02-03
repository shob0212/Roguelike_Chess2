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

    private System.Random rdm = new System.Random();
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
    [SerializeField] public GameObject okBottun;
    [SerializeField] public GameObject okBottunClickBlocker;
    [SerializeField] public GameObject cancelBottun;
    [SerializeField] public GameObject cancelBottunClickBlocker;
    [SerializeField]private TMP_Text isReadyAllText;    //準備完了人数の表示
    public GameObject player;  // プレイヤーのインスタンス
    public int gameOverPlayer;  //負けた人の数
    public List<PhotonView> playerPhotonViews;  //全てのプレイヤーのPhotonViewコンポーネント
    public List<Tile> allTiles;//全てのタイル
    private List<int> playerTurnOrders = new List<int>();   // プレイヤーの行動順
    public List<TMP_Text> playerHpTexts = new List<TMP_Text>();  //各プレイヤーのhp表示用テキスト
    private GameObject timer;   //現在のタイマーオブジェクト
    private int roundCount = 1;  // 現在のラウンド数
    public int phase;  // 現在のフェーズ
    private int isReadyAll;    // フェーズの準備が完了した人数
    public bool isReady;    //自分の準備状態
    private GameObject[] cardObjs;  //現在の手札
    private Coroutine startPhaseCoroutine;    // プレイヤーの行動を管理するコルーチンの参照
    public int turnPlayerNum = 1;   // 現在のプレイヤーの番号
    private bool isFirstPhase = true;  // 最初のフェーズかどうか
    public CardManager cm;
    public int block1_1, block1_2, block2_1, block2_2;
    


    void Awake()
    {
        // インスタンスが存在しない場合は設定し、存在する場合は破棄する
        if (Instance == null){
            Instance = this;
        }else{
            Destroy(gameObject);
        }

        if(PhotonNetwork.IsMasterClient){
            block1_1 = (rdm.Next(6)+1)*2;
            block1_2 = (rdm.Next(6)+1)*2;
            block2_1 = (rdm.Next(6)+1)*2;
            block2_2 = (rdm.Next(6)+1)*2;
            if(block1_1 == block2_1 && block1_2 == block2_2){
                while(block1_1 == block2_1 && block1_2 == block2_2){
                    block2_1 = (rdm.Next(6)+1)*2;
                    block2_2 = (rdm.Next(6)+1)*2;
                }
            }
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
            photonView.RPC("SyncBlocks", RpcTarget.AllBuffered, block1_1, block1_2, block2_1, block2_2);
        }
        //-----------------------------------------------------------------------------------------
    }

    [PunRPC]
    void SyncBlocks(int b1, int b2, int b3, int b4){
        this.block1_1 = b1;
        this.block1_2 = b2;
        this.block2_1 = b3;
        this.block2_2 = b4;
        Ground.Instance.SetTile();
    }

    void setPlayerObject()
    {
        int rotation=0;
        int vectorX=0;
        int vectorY=0;
        int camX=7;
        int camY=4;
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
            Plate.GetComponentInChildren<Canvas>().transform.Find("Player Name").GetComponent<TMP_Text>().text = PhotonNetwork.PlayerList.FirstOrDefault(p => p.ActorNumber == playerNum).NickName;
            playerHpTexts.Add(Plate.GetComponentInChildren<Canvas>().transform.Find("HP").GetComponent<TMP_Text>());
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


            if(playerNum == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                PlayerInfo.playerTurnOrder = index;
            }
        }
        Debug.Log("PlayerTurnOrder: " + PlayerInfo.playerTurnOrder);
        ActionExecute.Instance.photonView.RPC("SetPlayerHps", RpcTarget.AllBuffered, PlayerInfo.hp, PlayerInfo.playerTurnOrder);
        setPlayerObject();
        
    }

    
    IEnumerator RoundManager()
    {
        Debug.Log("<color=red>GM.TurnManager</color>");
        while(true)
        {

            for(int phase = 1; phase <= 2; phase++)
            {
                isReadyAll = 0;
                photonView.RPC("StartPhase", RpcTarget.AllBuffered, phase, false);

                // 全員のフェーズ準備が終わるまでるまで待機
                Debug.Log("最初のisPhaseReady"+isReadyAll);
                yield return new WaitUntil(() => (isReadyAll>=PhotonNetwork.PlayerList.Length));
                Debug.Log("最後のisPhaseReady"+isReadyAll);
                photonView.RPC("FinishPhaseReady", RpcTarget.AllBuffered);

                isReadyAll = 0;
                photonView.RPC("StartPhase", RpcTarget.AllBuffered, phase, true);
                yield return new WaitUntil(() => (isReadyAll>=PhotonNetwork.PlayerList.Length));
            }
        }
    }



    [PunRPC]
    void StartPhase(int phase, bool exe)
    {
        this.phase = phase;
        Debug.Log("<color=yellow>GM.StartPhase</color>");
        if(isFirstPhase){
            phaseText.text = "移動フェーズ";
            isReadyAllText.text = isReadyAll +"/"+ PhotonNetwork.CurrentRoom.PlayerCount +" - 準備完了";
            CardManager drawPC = Instantiate(cardDrawPrefab).GetComponent<CardManager>();
            StartCoroutine(drawPC.Draw(1, isFirstPhase));
            cm = Instantiate(cardDrawPrefab).GetComponent<CardManager>();
            StartCoroutine(cm.Draw(2, isFirstPhase));
            isFirstPhase = false;

        }else{

            isReadyAll = 0;
            if(!exe){
                isReady = false;
                if(phase == 1){
                    isReadyAllText.text = isReadyAll +"/"+ PhotonNetwork.CurrentRoom.PlayerCount +" - 準備完了";
                    Debug.Log("<color=green>roundCount++</color>");
                    roundCount++;
                    RoundNum.text = roundCount.ToString();
                    phaseText.text = "移動フェーズ";
                    
                    cardObjs = GameObject.FindGameObjectsWithTag("Card");
                    int i = 0;
                    foreach (GameObject cardObj in cardObjs)
                    {
                        cardObj.GetComponent<Card>().BlockClick(1,false);
                        if(cardObj.GetComponent<Card>().isSelected){
                            Debug.Log(cardObj.transform.parent.gameObject);
                            StartCoroutine(GameManager.Instance.cm.DestroyUsedCard(2, cardObj.transform.parent.gameObject));
                        }else{
                            i++;
                        }
                    }
                    if(i == 3){
                        StartCoroutine(cm.Draw(1, isFirstPhase));
                    }
                }else{
                    Debug.Log(isReadyAll);
                    isReadyAllText.text = isReadyAll +"/"+ PhotonNetwork.CurrentRoom.PlayerCount +" - 準備完了";
                    phaseText.text = "アクションフェーズ";
                    cardObjs = GameObject.FindGameObjectsWithTag("Card");
                    int i = 0;
                    foreach (GameObject cardObj in cardObjs)
                    {
                        cardObj.GetComponent<Card>().BlockClick(1,false);
                        if(cardObj.GetComponent<Card>().isSelected){
                            Debug.Log(cardObj.transform.parent.gameObject);
                            StartCoroutine(GameManager.Instance.cm.DestroyUsedCard(1, cardObj.transform.parent.gameObject));
                        }else{
                            i++;
                        }
                    }
                    if(i == 3){
                        StartCoroutine(cm.Draw(2, isFirstPhase));
                    }
                }
            }else{
                if(phase == 1){
                    isReadyAllText.text = "";
                    StartCoroutine(MoveExecute.Instance.execute());
                }else{
                    isReadyAllText.text = "";
                    StartCoroutine(ActionExecute.Instance.execute());
                }
            }
            
        }
        
    }



    [PunRPC]
    public void FinishPhaseReady()
    {
        Debug.Log("<color=yellow>GM.FinishPhase</color>");
        DestroyTimerObject();

        // 各フェーズを終了
        if(!isReady){
            isReady = true;
            photonView.RPC("UpdatePhaseReadyAll", RpcTarget.AllBuffered, isReady, false);
        }
        
        
        Destroy(GameObject.Find("Move Phase Manager(Clone)"));
        Destroy(GameObject.Find("Action Phase Manager(Clone)"));
        
        
    }

    
    public void PhaseReady(){
        Debug.Log("なんで3回きてんねんあほ");
        if(isReady){
            if(phase == 1){
                GameObject.Find("Move Phase Manager(Clone)").GetComponent<MovePhase>().cancelIsPhaseReady();;
            }else{
                GameObject.Find("Action Phase Manager(Clone)").GetComponent<ActionPhase>().cancelIsPhaseReady();;
            }
            isReady = false;
            Debug.Log("isReady = false");
            photonView.RPC("UpdatePhaseReadyAll", RpcTarget.AllBuffered, isReady, false);   // 自分の準備完了のキャンセルを全員に同期
        }else{
            if(phase == 1){
                GameObject.Find("Move Phase Manager(Clone)").GetComponent<MovePhase>().isPhaseReady();;
            }else{
                GameObject.Find("Action Phase Manager(Clone)").GetComponent<ActionPhase>().isPhaseReady();;
            }
            isReady = true;
            Debug.Log("isReady = true");
            photonView.RPC("UpdatePhaseReadyAll", RpcTarget.AllBuffered, isReady, false);   // 自分の準備完了を全員に同期
        }
    }
    

    [PunRPC]
    void UpdatePhaseReadyAll(bool phaseReady, bool exe)
    {
        Debug.Log("isReadyAll" + isReadyAll);
        if(phaseReady){
            isReadyAll++;   //準備完了人数を増やす
        }else{
            isReadyAll--;   //準備完了人数を減らす
        }
        if(!exe){
            isReadyAllText.text = isReadyAll +"/"+ PhotonNetwork.CurrentRoom.PlayerCount +" - 準備完了";
        }
    }


    public void setTimerObject()
    {
        timer = Instantiate(timerPrefab, playerUI.transform);
    }

    public void DestroyTimerObject(){
        Destroy(timer);
    }


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