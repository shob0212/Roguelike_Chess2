using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    public CardDatabase cardDatabase;  // 駒カードデータベース
    public Transform[] piaceCardSlots;      // 駒カードの3つのカードスロット (Inspectorで設定)
    public Transform[] skillCardSlots;      // スキルカードの5つのカードスロット (Inspectorで設定)
    private Transform[] cardSlots;
    public GameObject handcardsPC;
    public GameObject handcardsSC;
    private GameObject handcards;
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


    public void Draw(int phase, bool isFirstPhase)
    {
        this.phase = phase;
        this.isFirstPhase = isFirstPhase;
        
        if(phase == 1)
        {
            Debug.Log("PieceCard");
            cardSlots = piaceCardSlots;
            handcards = handcardsPC;
            currentCardSlotIndex = currentCardSlotIndexPC;
        }
        else
        {
            Debug.Log("SkillCard");
            cardSlots = skillCardSlots;
            handcards = handcardsSC;
            currentCardSlotIndex = currentCardSlotIndexSC;
        }

        // ランダムにカードを配布
        StartCoroutine(DrawCoroutine());
        

    }

    // 初期のカードを3枚配布
    IEnumerator DrawCoroutine()
    {
        
        while (currentCardSlotIndex < cardSlots.Length)
        {
            yield return StartCoroutine(DrawCard());
        }

        if(isFirstPhase){
            if(phase == 2){
                GameManager.Instance.setTimerObject();
            }
        }else{
            GameManager.Instance.setTimerObject();
        }
        
        if(phase == 1)
        {
            currentCardSlotIndexPC = currentCardSlotIndex;
        }
        else
        {
            currentCardSlotIndexSC = currentCardSlotIndex;
        }

        Destroy(this.gameObject);

        yield break;
    }


    // ランダムにカードを3枚配布してスロットに配置
    // 一枚のカードをドローしてスロットに配置
    IEnumerator DrawCard()
    {

        Debug.Log(GameManager.Instance.phase);

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
        /*sortedDealtCards.Sort((card1, card2) =>
        {
            return -card1.Strength.CompareTo(card2.Strength); // 強さの昇順でソート
        });*/
        /*sortedDealtCards = dealtCards
        .OrderBy(card => card.Strength) // 強さの昇順
        .ToList();

        // sortedDealtCards の順番に従って cardObjects を並び替え
        List<GameObject> sortedCardObjects = new List<GameObject>();
        foreach (var card in sortedDealtCards)
        {
            int index = dealtCards.IndexOf(card);
            sortedCardObjects.Add(cardObjects[index]);
        }*/

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

        // dealtCards と cardObjects をソート後の順番に更新
        dealtCards = sortedDealtCards;

        yield break;
    }

}
