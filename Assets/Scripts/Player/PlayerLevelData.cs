using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsData", menuName = "Data/Player Stats")]
public class PlayerLevelData : ScriptableObject
{
    public int startingLevel = 1;
    public float startingExpThreshold = 3f;

    [Tooltip("���� 1 ~ 20������ ����ġ �䱸�� ����Ʈ")]
    public List<float> expTable = new List<float>(20);
}