using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.MyGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {

        #region Private Methods

        void Start()
        {
            Instance = this;

            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>プレイヤープレハブの参照がありません</a></Color>。GameObject 'Game Manager' に設定してください", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("現在のシーンからローカルプレイヤーをインスタンス化しています: {0}", SceneManagerHelper.ActiveSceneName);
                    // ルームにいます。ローカルプレイヤーのキャラクターを生成します。PhotonNetwork.Instantiateを使用して同期されます。
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("シーンのロードを無視しています: {0}", SceneManagerHelper.ActiveSceneName);
                }
            }

        }

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : レベルをロードしようとしていますが、私たちはマスタークライアントではありません");
                return;
            }
            Debug.LogFormat("PhotonNetwork : レベルをロード中 : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room");
        }

        #endregion

        #region Public Fields

        [Tooltip("プレイヤーを表現するために使用するプレハブ")]
        public GameObject playerPrefab;


        public static GameManager Instance;
        #endregion



        #region Photon Callbacks

        /// <summary>
        /// ローカルプレイヤーがルームを退出したときに呼び出されます。ランチャーシーンをロードする必要があります。
        /// </summary>

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion


        #region Photon Callbacks

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // あなたが接続しているプレイヤーでない場合に表示されます

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // OnPlayerLeftRoomの前に呼び出されます

                //LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // 他のプレイヤーが切断したときに表示されます

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // OnPlayerLeftRoomの前に呼び出されます

                //LoadArena();
            }
        }

        #endregion

    }
}