using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "CardFolder/Card")]
public class CardAsset : ScriptableObject
{
    public string CardName;          // カードの名前 (例: King, Queen)
    public string CardType;          // カードの種類 (例: Piece, Skill)
    public int skillType;        //スキルの型
    public int Strength;             // カードの強さ
    public GameObject CardPrefab;    // カードのプレハブ
}
