using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ActionPhase : MonoBehaviour
{
    private GameObject[] cardObjs;
    public String skillName;
    public int skillType;
    public List<Tile> allTiles;
    private GameObject[] allAvatars;
    List<Tile> attackRange = new List<Tile>();
    List<Tile> attackableRange = new List<Tile>();


    void Awake()
    {
        cardObjs = GameObject.FindGameObjectsWithTag("Card");
        allTiles = GameManager.Instance.allTiles;
        allAvatars = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject cardObj in cardObjs)
        {
            cardObj.GetComponent<Card>().BlockClick(2,false);
        }
        GameManager.Instance.okBottun.SetActive(true);
        GameManager.Instance.cancelBottun.SetActive(true);
    }

    void Start(){
        if(PhotonNetwork.IsMasterClient){
            ActionExecute.Instance.Log("System：アクションフェーズ", 0);
        }
    }

    public void isPhaseReady(){
        ActionExecute.Instance.skillName = skillName;
        ActionExecute.Instance.skillType = skillType;
        activeCancelBottun(true);
    }

    public void cancelIsPhaseReady(){
        activeOkBottun(true);
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
            cardObj.GetComponent<Card>().BlockClick(2,true);
        }
        foreach (Tile tile in ActionExecute.Instance.attackRangeTiles)
        {
            tile.selectedHighlightObject.SetActive(true);
        }
        if(ActionExecute.Instance.attackTargetTile != null){
            ActionExecute.Instance.attackTargetTile.selectedHighlightObject.SetActive(true);
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
        CheckSkill(card);
        
    }

    public void OnTileClicked(Tile clickedTile)
    {
        // 2つ以上のタイルがハイライトされないように選択中ハイライトをリセット
        foreach (Tile tile in attackableRange)
        {
            if(tile != clickedTile){
                Debug.Log("何回出てる");
                tile.transform.Find("Selected Highlight").GetComponentInChildren<BlinkingEffect>().StopBlinking();
                //tile.selectedHighlightObject.SetActive(false);
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


    public void CheckSkill(Card card){
        GameObject myAvatar = GameManager.Instance.player;
        ActionExecute.Instance.usedSkill = card.cardName;

        switch (card.cardName)
        {
            case "剣":
                attackableRange = GetTilesWithinRange(myAvatar.transform.position, 1);
                foreach (var tile in attackableRange)
                {
                    tile.selectableHighlight(true);
                }
                break;
                
            case "槍":
                break;

            case "ピストル":
                break;

            case "スナイパーライフル":
                break;

            case "散弾銃":
                ActionExecute.Instance.attackRangeTiles = GetTilesWithinRange(myAvatar.transform.position, 1);
                foreach (var tile in ActionExecute.Instance.attackRangeTiles)
                {
                    tile.selectedHighlightObject.GetComponent<BlinkingEffect>().StartBlinking();
                }
                GameManager.Instance.okBottun.GetComponent<Button>().interactable = true;
                GameManager.Instance.okBottunClickBlocker.SetActive(false);
                break;

            case "妖刀":
                attackableRange = GetTilesWithinRange(myAvatar.transform.position, 1);
                foreach (var tile in attackableRange)
                {
                    tile.selectableHighlight(true);
                }
                break;

            case "聖剣":
                break;

            case "スタンガン":
                break;

            case "手榴弾":
                attackableRange = GetTilesWithinRange(myAvatar.transform.position, 3);
                foreach (var tile in attackableRange)
                {
                    tile.selectableHighlight(true);
                }
                break;

            case "毒弓矢":
                break;

            case "革命":
                GameManager.Instance.okBottun.GetComponent<Button>().interactable = true;
                GameManager.Instance.okBottunClickBlocker.SetActive(false);
                break;

            case "暗殺依頼":
                foreach (var tile in allTiles)
                {
                    foreach(var avatar in allAvatars){
        
                        if(tile.transform.position == avatar.transform.position){
                            if(avatar == myAvatar){
                                continue;
                            }
                            tile.selectableHighlight(true);
                        }
                    }
                }
                break;

            case "天災":
                break;

            case "宗教戦争":
                break;

            case "決闘":
                break;

            case "治療薬":
                break;

            case "オーバードーズ":
                break;

            case "気付け薬":
                break;

            case "妖精":
                GameManager.Instance.okBottun.GetComponent<Button>().interactable = true;
                GameManager.Instance.okBottunClickBlocker.SetActive(false);
                break;

            case "ヤドリギ":
                break;

            case "フライパン":
                attackableRange = GetTilesWithinRange(myAvatar.transform.position, 1);
                foreach (var tile in attackableRange)
                {
                    tile.selectableHighlight(true);
                }
                break;

            case "大盾":
                GameManager.Instance.okBottun.GetComponent<Button>().interactable = true;
                GameManager.Instance.okBottunClickBlocker.SetActive(false);
                break;

            case "パリィ":
                break;

            case "カウンター":
                break;

            case "シェルター":
                break;

            case "スタングレネード":
                break;

            case "草結び":
                break;

            case "挑発":
                break;

            case "キャスリング":
                break;

            case "プロモーション":
                break;
        }
    }




    public List<Tile> GetTilesWithinRange(Vector2 avatarPos, int range){
        List<Tile> validTiles = new List<Tile>();

        foreach (var tile in allTiles)
        {
            Vector2 tilePos = tile.transform.position;
            int dx = Mathf.Abs((int)(tilePos.x - avatarPos.x));
            int dy = Mathf.Abs((int)(tilePos.y - avatarPos.y));
            Debug.Log("avatar"+avatarPos.x +"+"+ avatarPos.y);
            Debug.Log("tile"+tilePos.x +"+"+ tilePos.y);
            Debug.Log("dx:"+dx +"   dy:"+ dy);
            if (dx <= range*2 && dy <= range*2 && (dx != 0 || dy != 0))
            {
                validTiles.Add(tile);
            }
        }
        return validTiles;
    }
}