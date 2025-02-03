using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class CardManager : MonoBehaviour
{

    [SerializeField] private GameObject actionPhaseManagerPrefab;  // アクションフェーズマネージャーのプレハブ
    [SerializeField] private GameObject movePhaseManagerPrefab;  // 移動フェーズマネージャーのプレハブ
    public CardDatabase cardDatabase;  // 駒カードデータベース
    public Transform[] piaceCardSlots;      // 駒カードの3つのカードスロット (Inspectorで設定)
    public Transform[] skillCardSlots;      // スキルカードの5つのカードスロット (Inspectorで設定)
    private Transform[] cardSlots;
    public GameObject handcardsPC;
    public GameObject handcardsSC;
    private GameObject handcards;
    private static List<(CardAsset, GameObject)> currentPCs = new List<(CardAsset, GameObject)>();//現在の手札中の駒カード
    private static List<(CardAsset, GameObject)> currentSCs = new List<(CardAsset, GameObject)>();//現在の手札中のスキルカード
    private List<(CardAsset, GameObject)> dealtCards = new List<(CardAsset, GameObject)>(); // 配布されたカードとそのオブジェクトのペア
    private static int currentCardSlotIndexPC = 0; // 現在の駒カードスロットインデックス
    private static int currentCardSlotIndexSC = 0; // 現在のスキルカードスロットインデックス
    private int currentCardSlotIndex = 0; // 現在のカードスロットインデックス
    private bool isAnimating = false; // アニメーション中かどうか 
    private int phase;
    private bool isFirstPhase;






    void Awake()
    {
        // カードデータベースを取得
        cardDatabase = GameObject.Find("Card Database").GetComponent<CardDatabase>();
        handcardsPC = GameObject.Find("PCs");
        handcardsSC = GameObject.Find("SCs");
        
        piaceCardSlots = new Transform[3];
        for (int i = 0; i < piaceCardSlots.Length; i++)
        {
            piaceCardSlots[i] = GameObject.Find("Piece Card "+(i+1)).transform;
        }

        skillCardSlots = new Transform[5];
        for (int i = 0; i < skillCardSlots.Length; i++)
        {
            skillCardSlots[i] = GameObject.Find("Skill Card "+(i+1)).transform;
        }
        
    }








    public IEnumerator Draw(int phase, bool isFirstPhase)
    {
        this.phase = phase;
        this.isFirstPhase = isFirstPhase;
        
        if(phase == 1)
        {
            cardSlots = piaceCardSlots;
            handcards = handcardsPC;
            currentCardSlotIndex = currentCardSlotIndexPC;
        }
        else
        {
            cardSlots = skillCardSlots;
            handcards = handcardsSC;
            currentCardSlotIndex = currentCardSlotIndexSC;
        }

        
        // ランダムにカードを配布
        yield return StartCoroutine(DrawCoroutine());
        InstantiatePhase();
    }



    public IEnumerator DestroyUsedCard(int phase, GameObject card){
        
        if(phase == 1){
            currentCardSlotIndexPC--;
            currentPCs.RemoveAt(currentPCs.FindIndex(item => item.Item2 == card));
            Destroy(card);
            dealtCards = currentPCs;
            cardSlots = piaceCardSlots;
            yield return StartCoroutine(SortCardsWithAnimation());
            dealtCards = currentSCs;
            StartCoroutine(Draw(2, false));
        }else{
            currentCardSlotIndexSC--;
            currentSCs.RemoveAt(currentSCs.FindIndex(item => item.Item2 == card));
            Destroy(card);
            dealtCards = currentSCs;
            cardSlots = skillCardSlots;
            yield return StartCoroutine(SortCardsWithAnimation());
            dealtCards = currentPCs;
            StartCoroutine(Draw(1, false));
        }
    }




    // 初期のカードを3枚配布
    IEnumerator DrawCoroutine()
    {
        
        while (currentCardSlotIndex < cardSlots.Length)
        {
            yield return StartCoroutine(DrawCard());
        }

        if(phase == 1)
        {
            currentCardSlotIndexPC = currentCardSlotIndex;
        }
        else
        {
            currentCardSlotIndexSC = currentCardSlotIndex;
        }

        //Destroy(this.gameObject);

        yield break;
    }









    // ランダムにカードを3枚配布してスロットに配置
    // 一枚のカードをドローしてスロットに配置
    IEnumerator DrawCard()
    {

        //Debug.Log(GameManager.Instance.phase);

        isAnimating = true;
        // ランダムにカードアセットを取得
        CardAsset randomCard = cardDatabase.GetRandomCard(this.phase);


        // カードプレハブを一時的な位置にインスタンス化
        GameObject cardObject = Instantiate(
            randomCard.CardPrefab,
            new Vector3(
                100f,
                100f,
                0f
                ), 
            Camera.main.transform.rotation,
            handcards.transform
        );

        cardObject.transform.localPosition = new Vector3(
            cardSlots[currentCardSlotIndex].localPosition.x,
            cardSlots[currentCardSlotIndex].localPosition.y + 0.5f,
            cardSlots[currentCardSlotIndex].localPosition.z
        );

        // Card スクリプトのフィールドを設定
        cardObject.GetComponentInChildren<Card>().Initialize(randomCard);

        Transform cardSlot = cardSlots[currentCardSlotIndex];
        // 親のローカル座標を基準にスロット位置を計算
        Vector3 targetPosition = handcards.transform.InverseTransformPoint(cardSlot.position);
        // アニメーションで移動
        cardObject.transform.DOLocalMove(targetPosition, 0.3f).SetEase(Ease.InOutQuad).OnComplete(() => isAnimating = false);

        // アニメーションが終了するまで待機
        yield return new WaitUntil(() => !isAnimating);


        // リストに追加
        dealtCards.Add((randomCard, cardObject));

        // 次のスロットに進む
        currentCardSlotIndex++;
        
        yield return StartCoroutine(SortCardsWithAnimation());
    }










    IEnumerator SortCardsWithAnimation()
    {

        // 配布されたカードを強さ順にソート（昇順）
        List<(CardAsset, GameObject)> sortedDealtCards = dealtCards.OrderBy(pair => pair.Item1.Strength).ToList();
        

        // 並び替えたカードをスロットにアニメーションで再配置
        for (int i = 0; i < sortedDealtCards.Count; i++)
        {
            isAnimating = true;
            var (card, cardObject) = sortedDealtCards[i];
            Transform cardSlot = cardSlots[i];

            // 親のローカル座標を基準にスロット位置を計算
            Vector3 targetPosition = handcards.transform.InverseTransformPoint(cardSlot.position);

            // アニメーションで移動
            cardObject.transform.DOLocalMove(targetPosition, 0.3f).SetEase(Ease.InOutQuad).OnComplete(() => isAnimating = false);
        }

        // アニメーションが終了するまで待機
        yield return new WaitUntil(() => !isAnimating);

        // dealtCards と をソート後の順番に更新
        dealtCards = sortedDealtCards;

        yield break;
    }







    public IEnumerator WaitForCardDistribution()
    {
        // カードの配布アニメーションが終了するまで待機
        yield return StartCoroutine(DrawCoroutine());

    }







    void InstantiatePhase()
    {
        if(isFirstPhase){
            if(phase != 1){
                currentSCs = dealtCards;
                Instantiate(movePhaseManagerPrefab);
                GameManager.Instance.setTimerObject();
            }else{
                currentPCs = dealtCards;
                Destroy(this.gameObject);
                
            }
        }else{
            if(phase == 1)
            {
                GameManager.Instance.setTimerObject();
                Instantiate(movePhaseManagerPrefab);
                currentPCs = dealtCards;
            }
            else
            {
                GameManager.Instance.setTimerObject();
                Instantiate(actionPhaseManagerPrefab);
                currentSCs = dealtCards;
            }
        }

    }





    
}
