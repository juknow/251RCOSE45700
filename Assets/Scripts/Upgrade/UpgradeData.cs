using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Upgrade/New Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea]
    public string description;
    public Sprite icon;

    // ���� ȿ���� ������ ���� (enum �Ǵ� �ĺ� ID)
    public UpgradeType upgradeType;
}

public enum UpgradeType
{
    IncreaseMaxHp,
    IncreaseDamage,
    IncreaseFireRate,
    IncreaseFirePoint,
    // ��� Ȯ�� ����
}