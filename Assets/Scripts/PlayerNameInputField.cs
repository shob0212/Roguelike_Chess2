using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Photon.Pun;
using Photon.Realtime;

using System.Collections;

namespace Room
{
    /// <summary>
    /// プレイヤー名入力フィールド。ユーザーが名前を入力でき、ゲーム内でプレイヤーの上に表示されます。
    /// </summary>
    [RequireComponent(typeof(TMP_InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        const string playerNamePrefKey = "PlayerName";

        void Start () {
            Debug.Log(("<color=yellow>PNIF.start</color>"));

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

         public void SetPlayerName(string value)
        {
            Debug.Log(("<color=yellow>PNIF.setPlayerName</color>"));
            Debug.Log((value));
            if (value == "")
            {
                PhotonNetwork.LocalPlayer.NickName = "DefaultPlayer";
            }else{
                PhotonNetwork.LocalPlayer.NickName = value;
            }
            PlayerPrefs.SetString(playerNamePrefKey,value);
        }

    }
}
