using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using TMPro;


namespace Lobby{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        private GameObject player;
        public TMP_Text playerName;
        private Camera sceneCamera;
        private GameObject playerAll;
        [SerializeField] private GameObject startButton;
        [SerializeField] private GameObject leaveButton;
        List<GameObject> allPlayers;
        




        void Start()
        {
            Debug.Log(("<color=yellow>LM.Start</color>"));
            //gm = this;
            if(!PhotonNetwork.IsMasterClient){
                startButton.SetActive(false);
                leaveButton.SetActive(false);
            }
            sceneCamera = Camera.main;
            allPlayers = FindObjectsOfType<GameObject>().Where(obj => obj.name == "Avatar (Clone)").ToList();
            Debug.Log(allPlayers);



            //プレイヤーIDの管理
            int myInitId = PhotonNetwork.LocalPlayer.ActorNumber;
            for(int i = 1; i <= 4; i++){
                foreach (Player otherPlayer in PhotonNetwork.PlayerList){
                    Debug.Log(myInitId+" , "+otherPlayer.ActorNumber%4);
                    if((myInitId%4 == otherPlayer.ActorNumber%4) && myInitId != otherPlayer.ActorNumber){
                        myInitId++;
                        continue;
                    }else{
                        PlayerInfo.playerId = myInitId%4;
                        if(PlayerInfo.playerId == 0){
                            PlayerInfo.playerId = 4;
                        }
                        goto LOOP_END;
                    }
                }
            }LOOP_END:
            Debug.Log(PlayerInfo.playerId);




            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>プレイヤープレハブの参照がありません</a></Color>。GameObject 'Game Manager' に設定してください", this);
            }
            else
            {
                Debug.Log(PhotonNetwork.InRoom);
                if (PlayerManager.LocalPlayerInstance == null)
                {

                    if(PhotonNetwork.PlayerList.Length == 1){
                        Debug.LogFormat("ルームにいます。ローカルプレイヤーのキャラクターを生成します。PhotonNetwork.Instantiateを使用して同期されます。: {0}", SceneManagerHelper.ActiveSceneName);
                        player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                        player.GetPhotonView().RPC("SetName", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
                    }
                }
                else
                {
                    Debug.LogFormat("シーンのロードを無視しています: {0}", SceneManagerHelper.ActiveSceneName);
                }
            }

        }








        [Tooltip("プレイヤーを表現するために使用するプレハブ")]
        public GameObject playerPrefab;


        //public static LobbyManager gm;






        public override void OnJoinedRoom() 
        { 
            Debug.Log(("<color=yellow>LM.OnJoinedRoom</color>"));

            Debug.Log("ルームに参加しました。プレイヤーオブジェクトを生成します。"); 


            int rotation=0;
            int vectorX=0;
            int vectorY=0;
            if(PlayerInfo.playerId == 2){

                rotation = 180;  vectorX = 14;  vectorY = 14;

            }else if(PlayerInfo.playerId == 3){

                rotation = 90;  vectorX = 14;  vectorY = 0;

            }else if(PlayerInfo.playerId == 4){

                rotation = 270;  vectorX = 0;  vectorY = 14;

            }
            sceneCamera.transform.rotation = Quaternion.Euler(0, 0, rotation);
            player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(vectorX, vectorY, 0f), Quaternion.identity, 0);
            player.GetPhotonView().RPC("SetName", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        }



        public override void OnLeftRoom()
        {
            Debug.Log(("<color=yellow>LM.OnLeftRoom</color>"));
            SceneManager.LoadScene(0);
        }



        public void LeaveRoom()
        {
            Debug.Log(("<color=yellow>LM.LeaveRoom</color>"));
            PhotonNetwork.LeaveRoom();
        }




        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.Log(("<color=yellow>LM.OnPlayerEnteredRoom</color>"));
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log(("<color=yellow>LM.OnPlayerLeftRoom</color>"));
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); 
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); 
                startButton.SetActive(true);
                leaveButton.SetActive(true);
            }
        }

        public void StartGame(){
            PhotonNetwork.LoadLevel("Game");
        }

    }
}