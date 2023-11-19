using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="RewardDB",menuName ="Rewards/Create Reward",order =1)]
public class Rewards : ScriptableObject
{
    public Reward[] rewards;
}
