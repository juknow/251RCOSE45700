using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

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

    // Button이 클릭될 때 이 함수 연결
    public void OnUpgradeSelected()
    {
        if (GameManager.Instance.isUpgradeSelected) return;  // 이미 선택했으면 무시
        GameManager.Instance.isUpgradeSelected = true;
        var player = NetworkClient.connection.identity.GetComponent<PlayerController>();

        player.CmdRequestUpgrade(upgradeData.upgradeType);

    }


}