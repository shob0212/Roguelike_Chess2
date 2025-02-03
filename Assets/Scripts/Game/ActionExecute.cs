using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;
using TMPro;

public class ActionExecute : MonoBehaviour
{
    public static ActionExecute Instance { get; private set; }
    private GameObject player;
    public PhotonView photonView;
    public int turnFinish;
    public int healFinish;
    public int defendFinish;
    public String skillName;
    private int received;

    //ログを追加するオブジェクト
	[SerializeField] public RectTransform content;
	// 生成するログ
	[SerializeField] private RectTransform originalElement;
	[SerializeField] private TMP_Text elementOriginalText;
    private Color color;
    private int playerId = PlayerInfo.playerId;
    private string logText;    //ログ全体を格納する変数
    private List<string> logTextReceivePlayers = new List<string>();  //効果を受けたプレイヤーのリスト
    private string logTextResult;   //スキルの効果を表示

    //-----------------スキルカードの変数一覧-----------------

    //移動フェーズに使った駒の種類
    public String usedPieace;
    //使用したスキルカード
    public String usedSkill;
    //使用したスキルカードの型
    public int skillType;
    //プレイヤーが指定した攻撃対象のタイル
    public Tile attackTargetTile;
    //攻撃範囲の座標
    public List<Tile> attackRangeTiles = new List<Tile>();
    //攻撃対象のactorNumber
    public int attackTargetActorNum;



    void Awake()
    {
        originalElement.gameObject.SetActive(false);
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

    void Start(){
        photonView = GetComponent<PhotonView>();
    }

    public IEnumerator execute(){
        
        player = GameManager.Instance.player;
        logText = PhotonNetwork.LocalPlayer.NickName + "：" + usedSkill + "  を使用\n\n";
        logTextReceivePlayers.Clear();
        healFinish = 0;
        turnheal();
        yield return new WaitUntil(() => (healFinish >= PhotonNetwork.CurrentRoom.PlayerCount));
        for(int type = 1; type <= 4; type++){
            for(int turn = 1; turn <= PhotonNetwork.CurrentRoom.PlayerCount; turn++){
                if(PlayerInfo.playerTurnOrder == turn){
                    Debug.Log("ここいく？");
                    if(skillType == type){
                        switch(type){
                            case 1:
                                StartCoroutine(Heal());
                                break;
                            case 2:
                                StartCoroutine(Defend());
                                break;
                            case 3:
                                StartCoroutine(Attack());
                                break;
                            case 4:
                                StartCoroutine(Special());
                                break;
                        }

                    }else{
                        photonView.RPC("UpdateTurnFinish", RpcTarget.AllBuffered);
                    }
                }else{
                    photonView.RPC("UpdateTurnFinish", RpcTarget.AllBuffered);
                }
                yield return new WaitUntil(() => (turnFinish >= PhotonNetwork.CurrentRoom.PlayerCount));
                turnFinish = 0;
                yield return new WaitForSeconds(1);

            }
        }
        GameManager.Instance.photonView.RPC("UpdatePhaseReadyAll", RpcTarget.AllBuffered, true, true);
        yield break;
    }

    [PunRPC]
    void UpdateTurnFinish()
    {
        turnFinish++;
    }






    //-------------自分が使用したスキルを処理するメソッド群---------------------------------



    //ターン持続回復処理
    void turnheal(){
        if(PlayerInfo.fairys.Count != 0){
            int rm = -1;
            for(int i = 0; i < PlayerInfo.fairys.Count; i++){
                if(0 == --PlayerInfo.fairys[i]){
                    rm = i;
                }
                photonView.RPC("SetPlayerHps", RpcTarget.AllBuffered, ++PlayerInfo.hp, PlayerInfo.playerTurnOrder);
                Log(PhotonNetwork.LocalPlayer.NickName + "： 妖精 の効果により１回復\n残り " + PlayerInfo.fairys[i] + "ターン", PlayerInfo.playerTurnOrder);
            }
            if(rm != -1){PlayerInfo.fairys.RemoveAt(rm);}
            
        }
        photonView.RPC("UpdateHealFinish", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void UpdateHealFinish()
    {
        healFinish++;
    }




    //回復系
    IEnumerator Heal(){
        switch(usedSkill){
            case "治療薬":
                break;
            case "オーバードーズ":
                break;
            case "気付け薬":
                break;
            case "妖精":
                PlayerInfo.fairys.Add(5);
                break;
        }
        Log(logText, PlayerInfo.playerTurnOrder);
        photonView.RPC("UpdateTurnFinish", RpcTarget.AllBuffered);
        yield break;
    }


    //防御系
    IEnumerator Defend(){
        switch(usedSkill){
            case "大盾":
                PlayerInfo.greatShield = true;
                break;
            case "パリィ":
                break;
            case "カウンター":
                break;
            case "シェルター":
                break;
            case "スタングレネード":
                break;
        }
        Log(logText, PlayerInfo.playerTurnOrder);
        photonView.RPC("UpdateTurnFinish", RpcTarget.AllBuffered);
        yield break;
    }


    //攻撃系
    IEnumerator Attack(){
        received = 0;
        string vecStr;
        switch(usedSkill){
            case "剣":
                vecStr = SerializeVector2(attackTargetTile.transform.position);
                photonView.RPC("ReceiveSkill", RpcTarget.Others, usedSkill, vecStr, PhotonNetwork.LocalPlayer.ActorNumber);
                logTextResult = "に３ダメージを与えた！";
                break;
            case "槍":
                break;
            case "ピストル":
                break;
            case "スナイパーライフル":
                break;
            case "散弾銃":
                vecStr = SerializeVector2List(attackRangeTiles);
                photonView.RPC("ReceiveSkill", RpcTarget.Others, usedSkill, vecStr, PhotonNetwork.LocalPlayer.ActorNumber);
                logTextResult = "に３ダメージを与えた！";
                break;
            case "妖刀":
                vecStr = SerializeVector2(attackTargetTile.transform.position);
                photonView.RPC("ReceiveSkill", RpcTarget.Others, usedSkill, vecStr, PhotonNetwork.LocalPlayer.ActorNumber);
                photonView.RPC("SetPlayerHps", RpcTarget.AllBuffered, PlayerInfo.hp -= 2, PlayerInfo.playerTurnOrder);
                logTextResult = "に５ダメージを与え、２ダメージを負った！";
                break;
            case "聖剣":
                break;
            case "スタンガン":
                break;
            case "手榴弾":
                vecStr = SerializeVector2(attackTargetTile.transform.position);
                photonView.RPC("ReceiveSkill", RpcTarget.Others, usedSkill, vecStr, PhotonNetwork.LocalPlayer.ActorNumber);
                logTextResult = "に２ダメージを与えた！";
                break;
            case "毒弓矢":
                break;
            case "革命":
                photonView.RPC("ReceiveSkill", RpcTarget.Others, usedSkill, null, PhotonNetwork.LocalPlayer.ActorNumber);
                logTextResult = "に２ダメージを与えた！";
                break;
            case "暗殺依頼":
                vecStr = SerializeVector2(attackTargetTile.transform.position);
                photonView.RPC("ReceiveSkill", RpcTarget.Others, usedSkill, vecStr, PhotonNetwork.LocalPlayer.ActorNumber);
                logTextResult = "に１ダメージを与えた！";
                break;
            case "天災":
                break;
            case "宗教戦争":
                break;
            case "決闘":
                break;
            case "ヤドリギ":
                break;
            case "フライパン":
                vecStr = SerializeVector2(attackTargetTile.transform.position);
                photonView.RPC("ReceiveSkill", RpcTarget.Others, usedSkill, vecStr, PhotonNetwork.LocalPlayer.ActorNumber);
                logTextResult = "に３ダメージを与えた！";
                break;
        }
        yield return new WaitUntil(() => (received >= PhotonNetwork.PlayerListOthers.Length));
        if(logTextReceivePlayers.Count != 0){
            foreach(string name in logTextReceivePlayers){
                logText += "→ " + name + "\n";
            }
            logText += logTextResult;
        }else{
            logText += "何も起こらなかった";
            if(usedSkill == "妖刀"){
                logText += "\n２ダメージを負った";
            }
        }
        
        Log(logText, PlayerInfo.playerTurnOrder);
        photonView.RPC("UpdateTurnFinish", RpcTarget.AllBuffered);
    }


    //特殊スキル
    IEnumerator Special(){
        received = 0;
        switch(usedSkill){
            case "草結び":
                break;
            case "挑発":
                break;
            case "キャスリング":
                break;
            case "プロモーション":
                break;
        }
        yield return new WaitUntil(() => (received >= PhotonNetwork.PlayerListOthers.Length));
        if(logTextReceivePlayers.Count != 0){
            foreach(string name in logTextReceivePlayers){
                logText += "→ " + name + "\n";
            }
            logText += logTextResult;
        }else{
            logText += "何も起こらなかった";
        }
        Log(logText, PlayerInfo.playerTurnOrder);
        photonView.RPC("UpdateTurnFinish", RpcTarget.AllBuffered);
    }





    string SerializeVector2(Vector2 vec2){
        return vec2.x + "," + vec2.y;
    }

    string SerializeVector2List(List<Tile> attackTiles){
        List<string> tilePosList = new List<string>();
        foreach(var tile in attackTiles){
            tilePosList.Add(tile.transform.position.x + "," + tile.transform.position.y);
        }
        return string.Join(";", tilePosList);
    }





    //--------------------他人が使用したスキルの影響を受け取るメソッド群----------------------------------
    [PunRPC]
    void ReceiveSkill(string skillName, string vecStr, int actNum){
        Vector2 playerPos = (Vector2)player.transform.position;
        if(vecStr != null){
            foreach(Vector2 vec in DeserializeVector2List(vecStr)){
                if(vec == playerPos){
                    if(PlayerInfo.greatShield){
                        PlayerInfo.greatShield = false;
                        photonView.RPC("SkillReceivedPlayerName", PhotonNetwork.CurrentRoom.GetPlayer(actNum), PhotonNetwork.LocalPlayer.NickName+"(大盾により無効化)");
                        break;
                    }
                    switch (skillName)
                    {
                        case "剣":
                            photonView.RPC("SetPlayerHps", RpcTarget.AllBuffered, PlayerInfo.hp -= 3, PlayerInfo.playerTurnOrder);
                            break;
                        case "槍":
                            break;
                        case "ピストル":
                            break;
                        case "スナイパーライフル":
                            break;
                        case "散弾銃":
                            if(PlayerInfo.fryingPan > 0){
                                PlayerInfo.fryingPan--;
                                photonView.RPC("SkillReceivedPlayerName", PhotonNetwork.CurrentRoom.GetPlayer(actNum), PhotonNetwork.LocalPlayer.NickName+"(フライパンにより無効化)");
                                GameManager.Instance.cm.DestroyUsedCard(2, GameObject.Find("21. FryingPan SC (Clone)"));
                                goto fryingPanLabel;
                            }
                            photonView.RPC("SetPlayerHps", RpcTarget.AllBuffered, PlayerInfo.hp -= 3, PlayerInfo.playerTurnOrder);
                            break;
                        case "妖刀":
                            photonView.RPC("SetPlayerHps", RpcTarget.AllBuffered, PlayerInfo.hp -= 5, PlayerInfo.playerTurnOrder);
                            break;
                        case "聖剣":
                            break;
                        case "スタンガン":
                            break;
                        case "手榴弾":
                            photonView.RPC("SetPlayerHps", RpcTarget.AllBuffered, PlayerInfo.hp -= 2, PlayerInfo.playerTurnOrder);
                            break;
                        case "毒弓矢":
                            break;
                        case "暗殺依頼":
                            photonView.RPC("SetPlayerHps", RpcTarget.AllBuffered, PlayerInfo.hp -= 1, PlayerInfo.playerTurnOrder);
                            break;
                        case "天災":
                            break;
                        case "宗教戦争":
                            break;
                        case "決闘":
                            break;
                        case "ヤドリギ":
                            break;
                        case "フライパン":
                            photonView.RPC("SetPlayerHps", RpcTarget.AllBuffered, PlayerInfo.hp -= 1, PlayerInfo.playerTurnOrder);
                            break;
                        case "スタングレネード":
                            break;
                        case "キャスリング":
                            break;
                        case "プロモーション":
                            break;
                    }
                    photonView.RPC("SkillReceivedPlayerName", PhotonNetwork.CurrentRoom.GetPlayer(actNum), PhotonNetwork.LocalPlayer.NickName);
                    break;
                }
            }
            fryingPanLabel:
            photonView.RPC("SkillReceived", PhotonNetwork.CurrentRoom.GetPlayer(actNum));
        }else{
            switch (skillName)
            {
                case "革命":
                    if(usedPieace == "キング" || usedPieace == "クイーン"){
                        if(PlayerInfo.greatShield){
                            PlayerInfo.greatShield = false;
                            photonView.RPC("SkillReceivedPlayerName", PhotonNetwork.CurrentRoom.GetPlayer(actNum), PhotonNetwork.LocalPlayer.NickName+"(大盾により無効化)");
                            break;
                        }
                        photonView.RPC("SetPlayerHps", RpcTarget.AllBuffered, PlayerInfo.hp -= 1, PlayerInfo.playerTurnOrder);
                        photonView.RPC("SkillReceivedPlayerName", PhotonNetwork.CurrentRoom.GetPlayer(actNum), PhotonNetwork.LocalPlayer.NickName);
                    }
                    break;
            }
            photonView.RPC("SkillReceived", PhotonNetwork.CurrentRoom.GetPlayer(actNum));
        }
        
    }

    [PunRPC]
    void SkillReceivedPlayerName(string name){
        logTextReceivePlayers.Add(name);
    }

    [PunRPC]
    void SkillReceived(){
        received++;
    }


    // 攻撃範囲をリストで受け取るメソッド
    List<Vector2> DeserializeVector2List(string serializedList)
    {
        List<Vector2> list = new List<Vector2>();
        string[] stringList = serializedList.Split(';');
        foreach (string str in stringList)
        {
            string[] values = str.Split(',');
            int x = (int)float.Parse(values[0]);
            int y = (int)float.Parse(values[1]);
            list.Add(new Vector2(x, y));
        }
        return list;
    }

    //全プレイヤーに表示されているHPテキストの同期
    [PunRPC]
    public void SetPlayerHps(int hp, int order){
        Debug.Log(hp+"はは"+order+"いってるここ？HP"+GameManager.Instance.playerHpTexts[order-1].text);
        GameManager.Instance.playerHpTexts[order-1].text = hp.ToString();
    }



    public void Log(String log, int id){
        photonView.RPC("SetLog", RpcTarget.AllBuffered, log, id);
    }

    [PunRPC]
    public void SetLog(String text, int playerId){
        elementOriginalText.text = text;
        switch (playerId)
            {
                case 1:
                    ColorUtility.TryParseHtmlString("#FC5D66", out color);
                    break;
                case 2:
                    ColorUtility.TryParseHtmlString("#FFB600", out color);
                    break;
                case 3:
                    ColorUtility.TryParseHtmlString("#37DA8C", out color);
                    break;
                case 4:
                    ColorUtility.TryParseHtmlString("#9179FF", out color);
                    break;
                default:
                    ColorUtility.TryParseHtmlString("#000000", out color);
                    break;
            }
        
        var element = GameObject.Instantiate<RectTransform>(originalElement);
		element.SetParent (content, false);
        element.gameObject.SetActive (true);
        color.a = 55/255f;
        element.GetComponent<Image>().color = color;
    }

}
