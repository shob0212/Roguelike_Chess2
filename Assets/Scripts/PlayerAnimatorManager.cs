using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Room{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {

        private Vector2 move;
        private Vector3 targetPos;
        [SerializeField] private float _speed = 5.0f;
        private float distance = 2.0f;
        public GameManager gm;
        private int xAdjust = 1;
        private int yAdjust = 1;


        private Transform player;

        void Start()
        {
            Debug.Log(("<color=yellow>PAM.start</color>"));
            gm = FindObjectOfType<GameManager>();
            Debug.Log(PhotonNetwork.PlayerList.Length);
            player = GameObject.Find("Avatar (Clone)").GetComponent<Transform>();
            if(gm.playerId == 2){
                xAdjust = -1;
                yAdjust = -1;
                targetPos = new Vector3(14, 14, 0);
            }else if(gm.playerId == 3){
                //xAdjust = -1;
                yAdjust = -1;
                targetPos = new Vector3(14, 0, 0);
            }else if(gm.playerId == 4){
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

            if(gm.playerId <= 2){
                move.x = Input.GetAxisRaw("Horizontal")*xAdjust;
                move.y = Input.GetAxisRaw("Vertical")*yAdjust;
            }else{
                move.y = Input.GetAxisRaw("Horizontal")*xAdjust;
                move.x = Input.GetAxisRaw("Vertical")*yAdjust;
            }
            if (move != Vector2.zero && transform.position == targetPos && transform.position.x+move.x >= 0 && transform.position.x+move.x <= 14 && transform.position.y+move.y >= 0 && transform.position.y+move.y <= 14)
            {
                targetPos += new Vector3(move.x, move.y, 0) * distance;
            }
            Move(targetPos);
        }

        private void Move(Vector3 targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);
        }

    }
}