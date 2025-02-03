using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class PlayerInfo : MonoBehaviour
{
    public static string playerName;
    public static int playerId;
    public static int playerTurnOrder;
    public static int hp = 10;

    //--------------------特殊効果---------------------------------------

    //ターン継続毒ダメージ用
    public static List<(int, String)> poisons = new List<(int, String)>();
    public static List<int> overDoses = new List<int>();
    public static List<int> fairys = new List<int>();
    public static List<(int, String)> MistletoeBuf = new List<(int, String)>();
    public static List<(int, String)> MistletoeDebuf = new List<(int, String)>();
    public static bool stun;
    public static bool stimulant;
    public static int fryingPan;
    public static bool greatShield;
    public static bool parry;
    public static bool counter;
    public static bool shelter;
    public static bool stunGrenade;
    public static bool taunt;
    public static bool castling;



}
