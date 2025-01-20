using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "CardFolder/Card")]
public class CardAsset : ScriptableObject
{
    //public string CardName;          // 駒カードの名前 (例: King, Queen)
    public int Strength;             // カードの強さ
    public GameObject CardPrefab;    // カードのプレハブ
}
