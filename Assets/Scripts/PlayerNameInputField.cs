using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;


/// <summary>
/// プレイヤー名入力フィールド。ユーザーが名前を入力でき、ゲーム内でプレイヤーの上に表示されます。
/// </summary>
[RequireComponent(typeof(TMP_InputField))]
public class PlayerNameInputField : MonoBehaviour
{
    const string playerNamePrefKey = "PlayerName";
    TMP_InputField _inputField;

    void Start () {
        Debug.Log(("<color=yellow>PNIF.start</color>"));

        string defaultName = string.Empty;
        _inputField = this.GetComponent<TMP_InputField>();
        if (_inputField!=null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                _inputField.text = defaultName;
            }
        }
        //PhotonNetwork.NickName =  defaultName;
    }

        public void SetPlayerName()
    {
        Debug.Log(("<color=yellow>PNIF.setPlayerName</color>"));
        Debug.Log((_inputField.text));
        if (_inputField.text == "")
        {
            PhotonNetwork.LocalPlayer.NickName = "DefaultPlayer";
        }else{
            PhotonNetwork.LocalPlayer.NickName = _inputField.text;
        }

        PlayerPrefs.SetString(playerNamePrefKey,_inputField.text);
    }

}