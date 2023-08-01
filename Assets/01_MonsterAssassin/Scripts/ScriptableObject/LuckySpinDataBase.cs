using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/LuckySpinDataBase", fileName = "LuckySpinDataBase.asset")]
public class LuckySpinDataBase : ScriptableObject
{
    public List<RewardDatabase.Reward> rewards;

    public RewardDatabase.Reward PickRandomReward()
    {
        //Get random following weight
        int sumWeight = rewards.Sum(rw => rw.weight);

        int radWeight = Random.Range(0, sumWeight);
        int curWeight = 0;

        for (int i = 0; i < rewards.Count; i++)
        {

            if (curWeight + rewards[i].weight > radWeight)
            {

                return rewards[i];
            }

            curWeight += rewards[i].weight;

        }

        return null;
    }
}
