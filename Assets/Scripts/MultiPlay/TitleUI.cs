using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    public GameObject serverPrefab; // TCPServerUnity ������
    public GameObject clientPrefab; // TCPClient ������

    public void OnHostClicked()
    {
        // 1. TCP ���� ����
        if (serverPrefab != null)
        {
            GameObject server = Instantiate(serverPrefab);
            DontDestroyOnLoad(server);
            Debug.Log("[TitleUI] ���� �ν��Ͻ� ������");
        }

        // 2. Ŭ���̾�Ʈ�� �ڱ� �ڽŵ� ����
        if (clientPrefab != null)
        {
            GameObject client = Instantiate(clientPrefab);
            DontDestroyOnLoad(client);
            Debug.Log("[TitleUI] ȣ��Ʈ�� Ŭ���̾�Ʈ�� ���ӵ�");
        }

        // 3. �� ������ �̵�
        SceneManager.LoadScene("RoomScene");
    }

    public void OnJoinClicked()
    {
        // �ʿ� �� Ŭ���̾�Ʈ �����յ� DontDestroyOnLoad�� ����
        if (clientPrefab != null)
        {
            GameObject client = Instantiate(clientPrefab);
            DontDestroyOnLoad(client);
            Debug.Log("[TitleUI] Ŭ���̾�Ʈ �ν��Ͻ� ������");
        }

        SceneManager.LoadScene("RoomScene");
    }
}