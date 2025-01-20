using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;


namespace Lobby{
    public class PlayerManager : MonoBehaviourPun
    {
        [Tooltip("ローカルプレイヤーのインスタンス。このインスタンスを使用して、ローカルプレイヤーがシーンに表示されているかどうかを確認します")]
        public static GameObject LocalPlayerInstance;
        [Tooltip("プレイヤーのUIゲームオブジェクトプレハブ")] [SerializeField] private GameObject playerUiPrefab;
        [Tooltip("プレイヤーの現在の体力")]  public float Health = 10f;
        private PlayerManager target;
        private Vector2 move;
        private Vector3 targetPos;
        //public GameManager gm;
        public TMP_Text nameText;
        
        

        void awake(){
            Debug.Log(("<color=yellow>PM.awake</color>"));
            if (photonView.IsMine) {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            DontDestroyOnLoad(this.gameObject);
        }

        void Start() {
            Debug.Log(("<color=yellow>PM.start</color>"));
            Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);

            if(PlayerInfo.playerId == 2){
                transform.rotation = Quaternion.Euler(0, 0, 180);
            }else if(PlayerInfo.playerId == 3){
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }else if(PlayerInfo.playerId == 4){
                transform.rotation = Quaternion.Euler(0, 0, 270);
            }
        }


        [PunRPC]
        void SetName(string name) {
            Debug.Log(("<color=yellow>PM.setName</color>"));
            nameText.text = name;
        }
        
    }
}