using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Photon.Pun;
using Photon.Realtime;

using System.Collections;

namespace Com.MyCompany.MyGame
{
    /// <summary>
    /// プレイヤー名入力フィールド。ユーザーが名前を入力でき、ゲーム内でプレイヤーの上に表示されます。
    /// </summary>
    [RequireComponent(typeof(TMP_InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Constants

        // タイプミスを避けるためにPlayerPrefキーを保存
        const string playerNamePrefKey = "PlayerName";

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// Unityの初期化フェーズ中にGameObjectで呼び出されるMonoBehaviourメソッドです。
        /// </summary>
        void Start () {

            string defaultName = string.Empty;
            TMP_InputField _inputField = this.GetComponent<TMP_InputField>();
            if (_inputField!=null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName =  defaultName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// プレイヤーの名前を設定し、将来のセッションのためにPlayerPrefsに保存します。
        /// </summary>
        /// <param name="value">プレイヤーの名前</param>
        public void SetPlayerName(string value)
        {
            // #重要
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("プレイヤー名が空またはnullです");
                return;
            }
            PhotonNetwork.NickName = value;

            PlayerPrefs.SetString(playerNamePrefKey,value);
        }

        #endregion
    }
}
