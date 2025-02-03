using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour
{
    private int phase; // フェーズ
    public string cardName; // カードの名前
    private string cardType; // カードの種類
    private int skillType;  //スキルの型
    public int cardNum;  //カードの番号
    private bool isClickEnabled; // クリックが有効かどうか
    public bool isSelected;    //選択されたカードかどうか
    //public bool isHighlighted;
    [SerializeField] private GameObject highlightObject; // ハイライト用のオブジェクト
    [SerializeField] private GameObject clickBlocker; // クリックをブロックするオブジェクト
    

    void Start()
    {
        // ハイライト用のオブジェクトを非表示にする
        highlightObject.SetActive(false);
    }

    void OnDestroy(){
        if(cardName == "フライパン"){
            PlayerInfo.fryingPan--;
        }
    }



    //フェーズが移行されたときクリックをブロックする処理
    public void BlockClick(int phase, bool exe)
    {
        if(exe){
            isClickEnabled = false;
            if(isSelected){
                highlightObject.SetActive(true);
            }else{
                clickBlocker.SetActive(true);
            }
        }else{
            if(phase == 1){
                if(cardType == "Piece"){
                    clickBlocker.SetActive(false);
                    isClickEnabled = true;
                }else{
                    clickBlocker.SetActive(true);
                    isClickEnabled = false;
                }
            }else{
                if(cardType == "Skill"){
                    clickBlocker.SetActive(false);
                    isClickEnabled = true;
                }else{
                    clickBlocker.SetActive(true);
                    isClickEnabled = false;
                }
            }
        }
        
        this.phase = phase;
    }


    // クリックされた時の処理
    public void Click()
    {
        if(!GameManager.Instance.isReady){
            Debug.Log("Clicked: " + cardName);
            if(isClickEnabled){
                if(phase == 1){
                    FindObjectOfType<MovePhase>().OnCardClicked(this);
                    FindObjectOfType<MovePhase>().pieceName = cardName;
                }else{
                    FindObjectOfType<ActionPhase>().OnCardClicked(this);
                    FindObjectOfType<ActionPhase>().skillName = cardName;
                    FindObjectOfType<ActionPhase>().skillType =skillType;
                }
                
            }
        }
    }


    // ハイライト処理
    public void Highlight(bool isHighlighted)
    {
        highlightObject.SetActive(isHighlighted);
    }



    // カードの初期化
    public void Initialize(CardAsset cardAsset)
    {
        this.cardName = cardAsset.CardName;
        this.cardType = cardAsset.CardType;
        this.skillType = cardAsset.skillType;
        this.cardNum = cardAsset.Strength;
        if(cardName == "フライパン"){
            PlayerInfo.fryingPan++;
        }
    }
}
