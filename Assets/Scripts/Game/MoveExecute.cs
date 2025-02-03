using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public class MoveExecute : MonoBehaviour
{
    public static MoveExecute Instance { get; private set; }
    private GameObject player;
    public String pieceName;
    public Vector3 targetPos;
    private Vector3 startPos;
    List<Vector3> allPlayersPos = new List<Vector3>();
    public bool turnFinish;
    private PhotonView photonView;

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

    void Start(){
        photonView = GetComponent<PhotonView>();
    }
    
    public IEnumerator execute(){
        allPlayersPos.Clear();
        player = GameManager.Instance.player;
        startPos = player.transform.position;
        for(int turn = 1; turn <= PhotonNetwork.CurrentRoom.PlayerCount; turn++){
            if(PlayerInfo.playerTurnOrder == turn){
                player.transform.DOLocalMove(targetPos, Mathf.Abs(Vector2.Distance(player.transform.position, targetPos)/5f)).SetEase(Ease.Linear).OnComplete(() => photonView.RPC("UpdateTurnFinish", RpcTarget.AllBuffered, targetPos));
                ActionExecute.Instance.Log(PhotonNetwork.LocalPlayer.NickName + "：" + pieceName + "を使用", PlayerInfo.playerId);
                ActionExecute.Instance.usedPieace = pieceName;
            }
            yield return new WaitUntil(() => (turnFinish));
            turnFinish = false;
        }
        int duplicationPos = 0;
        foreach(Vector3 pos in allPlayersPos){
            if(pos == targetPos){
                duplicationPos++;
            }
        }
        if(duplicationPos > 1){
            player.transform.DOLocalMove(startPos, Mathf.Abs(Vector2.Distance(player.transform.position, startPos)/10f)).SetEase(Ease.Linear).OnComplete(() => GameManager.Instance.photonView.RPC("UpdatePhaseReadyAll", RpcTarget.AllBuffered, true, true));
        }else{
            GameManager.Instance.photonView.RPC("UpdatePhaseReadyAll", RpcTarget.AllBuffered, true, true);
        }
    }

    [PunRPC]
    void UpdateTurnFinish(Vector3 playerTargetPos)
    {
        turnFinish = true;
        allPlayersPos.Add(playerTargetPos);
    }

}