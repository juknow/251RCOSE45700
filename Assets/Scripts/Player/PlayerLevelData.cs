using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsData", menuName = "Data/Player Stats")]
public class PlayerLevelData : ScriptableObject
{
    public int startingLevel = 1;
    public float startingExpThreshold = 3f;

    [Tooltip("레벨 1 ~ 20까지의 경험치 요구량 리스트")]
    public List<float> expTable = new List<float>(20);
}