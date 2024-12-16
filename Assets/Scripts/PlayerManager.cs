using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;

using System.Collections;

namespace Com.MyCompany.MyGame
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {

        #region Public Fields
        [Tooltip("ローカルプレイヤーのインスタンス。このインスタンスを使用して、ローカルプレイヤーがシーンに表示されているかどうかを確認します")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("プレイヤーのUIゲームオブジェクトプレハブ")]
        [SerializeField]
        private GameObject playerUiPrefab;

        [Tooltip("プレイヤーの現在の体力")]
        public float Health = 10f;

        private PlayerManager target;
        #endregion

        void awake(){
            // #重要
            // GameManager.csで使用されます: レベルが同期されるときにインスタンス化を防ぐためにlocalPlayerインスタンスを追跡します
            if (photonView.IsMine) {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            // #重要
            // レベルの同期中にインスタンスが生き残るように、ロード時に破棄しないようにフラグを立てます。これにより、レベルがロードされるときにシームレスな体験が得られます。
            DontDestroyOnLoad(this.gameObject);
        }

        void start(){
            if (playerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(playerUiPrefab);
                _uiGo.SendMessage("PlayerUI.SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>プレイヤーのプレハブにPlayerUiPrefabの参照がありません</a></Color>。", this);
            }
        }

        void update(){
            // ターゲットがnullの場合は自分自身を破壊します。これは、Photonがネットワーク上でプレイヤーのインスタンスを破壊しているときのフェイルセーフです。
            if (target == null)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

            if (stream.IsWriting)
            {
                //このプレイヤーを所有しています。データをほかのものに送ります。
                stream.SendNext(Health);
            }
            else
            {
                // ネットワークプレイヤー。データ受信
                this.Health = (float)stream.ReceiveNext();
            }

        }

        void calledOnLevelWasLoaded(){
            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

        #endregion

    } 
}