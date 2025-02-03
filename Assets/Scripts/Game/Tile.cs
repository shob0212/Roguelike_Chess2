using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    
    [SerializeField] private GameObject movableHighlightObject; // 移動可能表示ハイライト用のオブジェクト
    [SerializeField] public GameObject selectedHighlightObject; // 選択中表示ハイライト用のオブジェクト
    private bool isSelectable; // 選択可能かどうか
    private MoveExecute moveExe;
    public bool isSelected = false;    //選択中かどうか


    void Start()
    {
        // ハイライト用のオブジェクトを非表示にする
        movableHighlightObject.SetActive(false);
        selectedHighlightObject.SetActive(false);
    }


    public void click()
    {
        if(!GameManager.Instance.isReady){
            Debug.Log("Clicked: " + gameObject.name);
            if(isSelectable){
                if(GameManager.Instance.phase == 1){
                    FindObjectOfType<MovePhase>().OnTileClicked(this);
                    if(isSelected){
                        selectedHighlightObject.GetComponent<BlinkingEffect>().StopBlinking();
                        MoveExecute.Instance.targetPos = GameObject.Find("Avatar"+ PlayerInfo.playerTurnOrder+"(Clone)").transform.position;
                        GameManager.Instance.okBottun.GetComponent<Button>().interactable = false;
                        GameManager.Instance.okBottunClickBlocker.SetActive(true);
                        isSelected = false;
                    }else{
                        selectedHighlightObject.GetComponent<BlinkingEffect>().StartBlinking();
                        MoveExecute.Instance.targetPos = transform.position;
                        GameManager.Instance.okBottun.GetComponent<Button>().interactable = true;
                        GameManager.Instance.okBottunClickBlocker.SetActive(false);
                        isSelected = true;
                    }
                }else{
                    FindObjectOfType<ActionPhase>().OnTileClicked(this);
                    if(isSelected){
                        selectedHighlightObject.GetComponent<BlinkingEffect>().StopBlinking();
                        GameManager.Instance.okBottun.GetComponent<Button>().interactable = false;
                        GameManager.Instance.okBottunClickBlocker.SetActive(true);
                        isSelected = false;
                    }else{
                        selectedHighlightObject.GetComponent<BlinkingEffect>().StartBlinking();
                        ActionExecute.Instance.attackTargetTile = this;
                        GameManager.Instance.okBottun.GetComponent<Button>().interactable = true;
                        GameManager.Instance.okBottunClickBlocker.SetActive(false);
                        isSelected = true;
                    }
                }
    
            }
        }
    }

    public void selectableHighlight(bool isSelectable)
    {
        movableHighlightObject.SetActive(isSelectable);
        this.isSelectable = isSelectable;
    }
}
