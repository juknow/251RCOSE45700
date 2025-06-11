using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeContainer : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    private UpgradeData upgradeData;

    public void SetUpgrade(UpgradeData data)
    {
        upgradeData = data;
        iconImage.sprite = data.icon;
        nameText.text = data.upgradeName;
        descriptionText.text = data.description;

        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnUpgradeSelected);
    }

    public void OnUpgradeSelected()
    {
        if (PlayerStatManager.Instance != null)
        {
            PlayerStatManager.Instance.CmdApplyUpgrade(upgradeData.upgradeType);
        }
        else
        {
            Debug.LogWarning("[UpgradeContainer] PlayerStatManager 싱글톤이 없습니다");
        }
    }
}
