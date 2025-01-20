using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Lobby{
    public class LobbyAnimatorManager : MonoBehaviourPun
    {

        private Vector2 move;
        private Vector3 targetPos;
        [SerializeField] private float _speed = 5.0f;
        private float distance = 2.0f;
        private int xAdjust = 1;
        private int yAdjust = 1;
        private Transform p;

        void Start()
        {
            Debug.Log(("<color=yellow>PAM.start</color>"));
            //gm = FindObjectOfType<GameManager>();
            Debug.Log(PhotonNetwork.PlayerList.Length);
            p = GameObject.Find("Avatar (Clone)").GetComponent<Transform>();
            if(PlayerInfo.playerId == 2){
                xAdjust = -1;
                yAdjust = -1;
                targetPos = new Vector3(14, 14, 0);
            }else if(PlayerInfo.playerId == 3){
                //xAdjust = -1;
                yAdjust = -1;
                targetPos = new Vector3(14, 0, 0);
            }else if(PlayerInfo.playerId == 4){
                xAdjust = -1;
                //yAdjust = -1;
                targetPos = new Vector3(0, 14, 0);
            }
        }


        

        void Update()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }

            if(PlayerInfo.playerId <= 2){
                move.x = Input.GetAxisRaw("Horizontal")*xAdjust;
                move.y = Input.GetAxisRaw("Vertical")*yAdjust;
            }else{
                move.y = Input.GetAxisRaw("Horizontal")*xAdjust;
                move.x = Input.GetAxisRaw("Vertical")*yAdjust;
            }
            if (move != Vector2.zero && p.position == targetPos && p.position.x+move.x >= 0 && p.position.x+move.x <= 14 && p.position.y+move.y >= 0 && p.position.y+move.y <= 14)
            {
                targetPos += new Vector3(move.x, move.y, 0) * distance;
            }
            Move(targetPos);
        }

        private void Move(Vector3 targetPosition)
        {
            p.position = Vector3.MoveTowards(p.position, targetPosition, _speed * Time.deltaTime);
        }

    }
}