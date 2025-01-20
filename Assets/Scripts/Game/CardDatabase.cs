using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public List<CardAsset> pieceCardAssets; // 駒カードアセットのリスト (Inspectorで設定)
    public List<CardAsset> skillCardAssets; // スキルカードアセットのリスト (Inspectorで設定)

    // ランダムなカードアセットを取得
    public CardAsset GetRandomCard(int cardTypeNum)
    {
        if(cardTypeNum == 1)
        {
            int index = Random.Range(0, pieceCardAssets.Count);
            Debug.Log("index: " + index);
            return pieceCardAssets[index];
        }
        else
        {
            int index = Random.Range(0, skillCardAssets.Count);
            return skillCardAssets[index];
        }
        
    }
}
