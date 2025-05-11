using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Upgrade/New Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea]
    public string description;
    public Sprite icon;

    // 실제 효과를 적용할 종류 (enum 또는 식별 ID)
    public UpgradeType upgradeType;
}

public enum UpgradeType
{
    IncreaseMaxHp,
    IncreaseDamage,
    IncreaseFireRate,
    IncreaseFirePoint,
    // 등등 확장 가능
}