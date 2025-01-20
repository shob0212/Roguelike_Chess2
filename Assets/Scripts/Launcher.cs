using UnityEngine;
using Photon.Pun;     
using Photon.Realtime;
using System.Threading;

public class Launcher : MonoBehaviourPunCallbacks
{
    bool isConnecting = false;
    //bool connectedMasterflg = false;

    [Tooltip("ユーザーが名前を入力し、接続してプレイするためのUIパネル")]
    [SerializeField]
    private GameObject controlPanel;
    [Tooltip("接続が進行中であることをユーザーに知らせるUIラベル")]
    [SerializeField]
    private GameObject progressLabel;

    [Tooltip("プレイヤーを表現するために使用するプレハブ")]public GameObject playerPrefab;


    void Awake()
        {
            Debug.Log(("<color=yellow>L.awake</color>"));
        }

    void Start()
        {
            Debug.Log(("<color=yellow>L.start</color>"));
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.Log(PhotonNetwork.NetworkClientState);
        }
    public void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log(("<color=yellow>L.connect</color>"));
        Debug.Log(PhotonNetwork.NetworkClientState);
        if(PhotonNetwork.NetworkClientState == ClientState.Disconnected || PhotonNetwork.NetworkClientState == ClientState.PeerCreated)
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
        }

        progressLabel.SetActive(true);
        controlPanel.SetActive(false);


        if (!isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }

        /*if(!isConnecting)
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }*/
    }
    


    public override void OnConnectedToMaster()
    {
        Debug.Log(("<color=yellow>L.OnConnectedToMaster</color>"));
        
        if(isConnecting){
            PhotonNetwork.JoinRandomRoom();
        }
        isConnecting = false;

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(("<color=yellow>L.OnJoinRandomFailed</color>"));
        var option = new RoomOptions();
        option.MaxPlayers = 4;      
        PhotonNetwork.CreateRoom(null,option);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(("<color=yellow>L.OnJoinedRoom</color>"));
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("'Room'をロードします");

            PhotonNetwork.LoadLevel("Lobby");
        }
        
    }
}
