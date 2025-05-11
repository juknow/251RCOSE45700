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

        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnUpgradeSelected);
    }

    // Button�� Ŭ���� �� �� �Լ� ����
    public void OnUpgradeSelected()
    {
        GameManager.Instance.ApplyUpgrade(upgradeData.upgradeType);
    }
}