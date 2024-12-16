using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.MyGame
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields

        /// <summary>
        /// 現在のプロセスを追跡します。接続は非同期であり、Photonからのいくつかのコールバックに基づいているため、
        /// Photonからのコールバックを受け取ったときに適切に動作を調整するためにこれを追跡する必要があります。
        /// 通常、これはOnConnectedToMaster()コールバックで使用されます。
        /// </summary>
        bool isConnecting;

        /// <summary>
        /// 各ルームの最大プレイヤー数です。ルームが満員になると、新しいプレイヤーは参加できず、新しいルームが作成されます。
        /// </summary>
        [Tooltip("各ルームの最大プレイヤー数です。ルームが満員になると、新しいプレイヤーは参加できず、新しいルームが作成されます")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        #endregion



        #region Private Fields

        /// <summary>
        /// このクライアントのバージョン番号です。ユーザーはgameVersionによって分離されます（これにより、破壊的な変更を行うことができます）。
        /// </summary>
        string gameVersion = "1";

        #endregion



        #region Public Fields

        [Tooltip("ユーザーが名前を入力し、接続してプレイするためのUIパネル")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("接続が進行中であることをユーザーに知らせるUIラベル")]
        [SerializeField]
        private GameObject progressLabel;

        #endregion



        #region MonoBehaviour CallBacks

        /// <summary>
        /// Unityの初期化フェーズ中にGameObjectで呼び出されるMonoBehaviourメソッドです。
        /// </summary>
        void Awake()
        {
            // #重要
            // これにより、マスタークライアントでPhotonNetwork.LoadLevel()を使用でき、同じルーム内のすべてのクライアントが自動的にレベルを同期します。
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        /// <summary>
        /// Unityの初期化フェーズ中にGameObjectで呼び出されるMonoBehaviourメソッドです。
        /// </summary>
        void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        #endregion



        #region Public Methods

        /// <summary>
        /// 接続プロセスを開始します。
        /// - すでに接続されている場合は、ランダムなルームへの参加を試みます。
        /// - まだ接続されていない場合、このアプリケーションインスタンスをPhoton Cloud Networkに接続します。
        /// </summary>
        public void Connect()
        {
            // ルームに参加する意思を追跡します。ゲームから戻ってきたときに接続されているというコールバックを受け取るため、その時に何をすべきかを知る必要があります。
            //isConnecting = PhotonNetwork.ConnectUsingSettings();


            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            // 接続されているかどうかを確認し、接続されている場合は参加し、そうでない場合はサーバーへの接続を開始します。
            if (PhotonNetwork.IsConnected)
            {
                // #重要 この時点でランダムなルームへの参加を試みる必要があります。失敗した場合は、OnJoinRandomFailed()で通知され、新しいルームを作成します。
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #重要 まず最初にPhoton Online Serverに接続する必要があります。
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
            // ルームに参加しようとしていない場合は何もしません。
            // isConnectingがfalseの場合は、通常、ゲームに負けたか、ゲームを終了したときです。このレベルがロードされると、OnConnectedToMasterが呼び出されますが、その場合は何もしません。
            if (isConnecting)
            {
                // #重要: 最初に試みるのは、既存のルームに参加することです。もしあれば良いですが、なければOnJoinRandomFailedでコールバックされます。
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }

        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
            // ---------------------PUN Basics Tutorial/Launcher: OnDisconnected() がPUNによって呼び出されました。理由: {0}",
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
            //-------- PUN Basics Tutorial/Launcher: OnJoinRandomFailed() がPUNによって呼び出されました。ランダムなルームが利用できないため、新しいルームを作成します。\n呼び出し: PhotonNetwork.CreateRoom


            // #重要: ランダムなルームへの参加に失敗しました。既存のルームがないか、すべて満室かもしれません。心配いりません、新しいルームを作成します。
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            // #重要: 最初のプレイヤーである場合のみロードします。それ以外の場合は、`PhotonNetwork.AutomaticallySyncScene`を使用してインスタンスシーンを同期します。
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("'Room'をロードします");

                // #重要
                // ルームレベルをロードします。
                PhotonNetwork.LoadLevel("Room");
            }

            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            //---------PUN Basics Tutorial/Launcher: OnJoinedRoom() がPUNによって呼び出されました。現在、このクライアントはルームにいます。
        }

        #endregion


    #endregion

    }
}
