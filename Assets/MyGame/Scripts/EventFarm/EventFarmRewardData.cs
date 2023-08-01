using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventFarmRewardData", menuName = "ScriptableObjects/EventFarmRewardData")]
public class EventFarmRewardData : ScriptableObject
{
    public List<FarmZoneReward> allZoneRewards;
}

[Serializable]
public class FarmZoneReward
{
    public List<RewardDatabase.Reward> zoneRewardList;
}