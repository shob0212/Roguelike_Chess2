using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MovePhase : MonoBehaviour
{
    private GameObject[] cardObjs;
    public String pieceName;
    public List<Tile> allTiles;
    private List<Tile> movableTiles;
    private GameObject[] allAvatars;


    void Awake()
    {
        cardObjs = GameObject.FindGameObjectsWithTag("Card");
        allTiles = GameManager.Instance.allTiles;
        foreach (Tile tile in allTiles)
        {
            tile.selectedHighlightObject.GetComponent<BlinkingEffect>().StopBlinking();
        }
        allAvatars = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject cardObj in cardObjs)
        {
            cardObj.GetComponent<Card>().BlockClick(1,false);
            
        }
        GameManager.Instance.okBottun.SetActive(true);
        GameManager.Instance.cancelBottun.SetActive(true);
    }

    void Start(){
        if(PhotonNetwork.IsMasterClient){
            ActionExecute.Instance.Log("System：移動フェーズ", 0);
        }
    }

    public void isPhaseReady(){
        activeCancelBottun(true);
        MoveExecute.Instance.pieceName = pieceName;
        foreach (var tile in movableTiles)
        {
            tile.selectableHighlight(false);
        }
    }

    public void cancelIsPhaseReady(){
        activeOkBottun(true);
        foreach (var tile in movableTiles)
        {
            tile.selectableHighlight(true);
        }
    }

    void OnDestroy()
    {
        GameManager.Instance.okBottun.SetActive(false);
        GameManager.Instance.cancelBottun.SetActive(false);
        GameObject selectedCardObj = null;
        foreach (GameObject cardObj in cardObjs)
        {
            if(cardObj.GetComponent<Card>().isSelected){
                selectedCardObj = cardObj;
            }
        }
        ResetUI();
        foreach (GameObject cardObj in cardObjs)
        {
            if(selectedCardObj == cardObj){
                cardObj.GetComponent<Card>().isSelected = true;
            }
            cardObj.GetComponent<Card>().BlockClick(1,true);
        }
        foreach (Tile tile in allTiles)
        {
            if(tile.isSelected){
                tile.selectedHighlightObject.SetActive(true);
                Debug.Log("ここ絶対行ってへんやろ");
            }
        }
    }

    public void activeOkBottun(bool okActive){
        GameManager.Instance.okBottun.GetComponent<Button>().interactable = okActive;
        GameManager.Instance.okBottunClickBlocker.SetActive(!okActive);
        if(okActive) {activeCancelBottun(!okActive);}
    }

    public void activeCancelBottun(bool cancelActive){
        GameManager.Instance.cancelBottun.GetComponent<Button>().interactable = cancelActive;
        GameManager.Instance.cancelBottunClickBlocker.SetActive(!cancelActive);
        if(cancelActive){activeOkBottun(!cancelActive);}
    }

    public void OnCardClicked(Card card)
    {
        // 一旦UIをリセット
        ResetUI(); 
        // 選択されたカードをハイライト
        card.GetComponent<Card>().Highlight(true);
        card.GetComponent<Card>().isSelected = true;
        //選択可能なタイルをハイライト
        movableTiles = CalculateMovableTiles(card);
        foreach (var tile in movableTiles)
        {
            tile.selectableHighlight(true);
        }
    }

    public void OnTileClicked(Tile clickedTile)
    {
        activeOkBottun(true);
        // 2つ以上のタイルがハイライトされないように選択中ハイライトをリセット
        foreach (Tile tile in movableTiles)
        {
            if(tile != clickedTile){
                Debug.Log("何回出てる");
                tile.transform.Find("Selected Highlight").GetComponentInChildren<BlinkingEffect>().StopBlinking();
                tile.isSelected = false;
            }
        }
    }

    private void ResetUI()
    {
        GameManager.Instance.okBottunClickBlocker.SetActive(true);
        GameManager.Instance.okBottun.GetComponent<Button>().interactable = false;
        GameManager.Instance.cancelBottunClickBlocker.SetActive(true);
        GameManager.Instance.cancelBottun.GetComponent<Button>().interactable = false;
        cardObjs = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject card in cardObjs)
        {
            if(card != null){
                card.GetComponent<Card>().Highlight(false);
                card.GetComponent<Card>().isSelected = false;
            }
        }
        foreach (Tile tile in allTiles)
        {
            tile.selectableHighlight(false);
            tile.selectedHighlightObject.SetActive(false);
            tile.isSelected = false;
            tile.selectedHighlightObject.GetComponent<BlinkingEffect>().StopBlinking();
        }
    }




    public List<Tile> CalculateMovableTiles(Card card)
    {
        GameObject myAvatar = GameManager.Instance.player;

        List<Tile> movableTiles = new List<Tile>();

        switch (card.cardName)
        {
            case "ポーン":
                // ポーンの動きに基づいて移動可能なマスを計算
                foreach (var tile in allTiles)
                {
                    if (isPawnMoveValid(myAvatar, tile))
                    {
                        movableTiles.Add(tile);
                    }
                }
                break;

            case "ナイト":
                // ナイトの動きに基づいて移動可能なマスを計算
                foreach (var tile in allTiles)
                {
                    if (isKnightMoveValid(myAvatar, tile))
                    {
                        movableTiles.Add(tile);
                    }
                }
                break;

            case "ビショップ":
                // ビショップの動きに基づいて移動可能なマスを計算
                foreach (var tile in allTiles)
                {
                    if (isBishopMoveValid(myAvatar, tile))
                    {
                        movableTiles.Add(tile);
                    }
                }
                break;

            case "ルーク":
                // ルークの動きに基づいて移動可能なマスを計算
                foreach (var tile in allTiles)
                {
                    if (isRookMoveValid(myAvatar, tile))
                    {
                        movableTiles.Add(tile);
                    }
                }
                break;

            case "クイーン":
                // クイーンの動きに基づいて移動可能なマスを計算
                foreach (var tile in allTiles)
                {
                    if (isQueenMoveValid(myAvatar, tile))
                    {
                        movableTiles.Add(tile);
                    }
                }
                break;

            case "キング":
                // キングの動きに基づいて移動可能なマスを計算
                foreach (var tile in allTiles)
                {
                    if (isKingMoveValid(myAvatar, tile))
                    {
                        movableTiles.Add(tile);
                    }
                }
                break;
        }

        return movableTiles;
    }


    private bool isPawnMoveValid(GameObject myAvatar, Tile tile)
    {
        // ポーンの動きを計算するロジック (例: 前方1マス移動)
        Vector2 avatarPos = myAvatar.transform.position;
        Vector2 tilePos = tile.transform.position;

        int dx = (int)(tilePos.x - avatarPos.x);
        int dy = (int)(tilePos.y - avatarPos.y);
        Debug.Log(dx+" + "+dy);
        if(PlayerInfo.playerId == 1){
            return dx == 0 && dy == 2;
        }else if(PlayerInfo.playerId == 2){
            return dx == 0 && dy == -2;
        }else if(PlayerInfo.playerId == 3){
            return dx == -2 && dy == 0;
        }else {
            return dx == 2 && dy == 0;
        }
    }

    private bool isKnightMoveValid(GameObject myAvatar, Tile tile)
    {
        // ナイトの動きを計算するロジック (例: L字型移動)
        Vector2 avatarPos = myAvatar.transform.position;
        Vector2 tilePos = tile.transform.position;

        int dx = (int)Mathf.Abs(tilePos.x - avatarPos.x);
        int dy = (int)Mathf.Abs(tilePos.y - avatarPos.y);
        Debug.Log(dx+" + "+dy);
        return ((dx == 4 && dy == 2) || (dx == 2 && dy == 4)) && checkOverlapAvatars((int)tilePos.x, (int)tilePos.y);
    }

    private bool isBishopMoveValid(GameObject myAvatar, Tile tile)
    {
        // ビショップの動きを計算するロジック (例: 斜め移動)
        Vector2 avatarPos = myAvatar.transform.position;
        Vector2 tilePos = tile.transform.position;

        int dx = (int)Mathf.Abs(tilePos.x - avatarPos.x);
        int dy = (int)Mathf.Abs(tilePos.y - avatarPos.y);
        Debug.Log(dx+" + "+dy);
        return dx == dy && checkOverlapAvatars((int)tilePos.x, (int)tilePos.y)&& IsPathClear(avatarPos, tilePos);
    }

    private bool isRookMoveValid(GameObject myAvatar, Tile tile)
    {
        // ルークの動きを計算するロジック (例: 横または縦移動)
        Vector2 avatarPos = myAvatar.transform.position;
        Vector2 tilePos = tile.transform.position;

        int dx = (int)Mathf.Abs(tilePos.x - avatarPos.x);
        int dy = (int)Mathf.Abs(tilePos.y - avatarPos.y);
        Debug.Log(dx+" + "+dy);
        return (dx == 0 || dy == 0) && checkOverlapAvatars((int)tilePos.x, (int)tilePos.y) && IsPathClear(avatarPos, tilePos);
    }

    private bool isQueenMoveValid(GameObject myAvatar, Tile tile)
    {
        // クイーンの動きを計算するロジック (例: 斜めまたは横または縦移動)
        Vector2 avatarPos = myAvatar.transform.position;
        Vector2 tilePos = tile.transform.position;

        int dx = (int)Mathf.Abs(tilePos.x - avatarPos.x);
        int dy = (int)Mathf.Abs(tilePos.y - avatarPos.y);
        Debug.Log(dx+" + "+dy);
        return (dx == dy || dx == 0 || dy == 0) && checkOverlapAvatars((int)tilePos.x, (int)tilePos.y) && IsPathClear(avatarPos, tilePos);
    }

    private bool isKingMoveValid(GameObject myAvatar, Tile tile)
    {
        // キングの動きを計算するロジック (例: 1マス移動)
        Vector2 avatarPos = myAvatar.transform.position;
        Vector2 tilePos = tile.transform.position;

        int dx = (int)Mathf.Abs(tilePos.x - avatarPos.x);
        int dy = (int)Mathf.Abs(tilePos.y - avatarPos.y);
        Debug.Log(dx+" + "+dy);
        return dx <= 2 && dy <= 2 && (dx != 0 || dy != 0) && checkOverlapAvatars((int)tilePos.x, (int)tilePos.y);
    }

    private bool checkOverlapAvatars(float x, float y)
    {
        foreach (var avatar in allAvatars)
        {
            if (avatar.transform.position.x == x && avatar.transform.position.y == y)
            {
                Debug.Log(avatar.transform.position.x+"と"+x+"と"+avatar.transform.position.y+"と"+y);
                return false;
            }
        }
        return true;

    }

    private bool IsPathClear(Vector2 startPos, Vector2 endPos)
    {
        Vector2 direction = (endPos - startPos).normalized/new Vector2(Mathf.Abs((endPos - startPos).normalized.x), Mathf.Abs((endPos - startPos).normalized.y)); // 方向ベクトルを正規化
        if(float.IsNaN(direction.x)){
            direction.x = 0f;
        }
        if(float.IsNaN(direction.y)){
            direction.y = 0f;
        }
        int distance;
        if((startPos - endPos).x != 0){
            distance = (int)Mathf.Abs((startPos - endPos).x);
        }else{
            distance = (int)Mathf.Abs((startPos - endPos).y);
        }
        Vector2 currentPos = startPos;
        Debug.Log(distance);

        // 距離分ループしてチェック
        for (float step = 2; step < distance; step+=2)
        {
            currentPos = startPos + direction * step; // 途中のタイル座標を計算

            // 現在のタイルがブロックされているかをチェック
            if (IsTileBlocked(currentPos))
            {
                return false;
            }
        }

        return true; // ルート上にブロックがなければ移動可能
    }

    // タイルがブロックされているかチェック
    private bool IsTileBlocked(Vector2 position)
    {
        if (new Vector2(GameManager.Instance.block1_1,GameManager.Instance.block1_2)==position || new Vector2(GameManager.Instance.block2_1,GameManager.Instance.block2_2)==position)
        {
            return true;
        }

        return false;
    }
}
