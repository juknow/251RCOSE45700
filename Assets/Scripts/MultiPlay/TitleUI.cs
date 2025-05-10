using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TitleUI : MonoBehaviour
{
    public GameObject serverPrefab; // TCPServerUnity ������
    public GameObject clientPrefab; // TCPClient ������

    public void OnHostClicked()
    {
        StartCoroutine(HostRoutine());
    }

    private IEnumerator HostRoutine()
    {
        GameObject server = Instantiate(serverPrefab);
        DontDestroyOnLoad(server);

        yield return new WaitForSeconds(0.2f); // ������ ���� Listen �ϵ��� ���

        GameObject client = Instantiate(clientPrefab);
        DontDestroyOnLoad(client);

        yield return new WaitForSeconds(0.2f); // Ŭ���̾�Ʈ�� ������ ���� �����ϵ��� ��� ���

        SceneManager.LoadScene("RoomScene");
    }


    public void OnJoinClicked()
    {
        StartCoroutine(JoinRoutine());
    }

    private IEnumerator JoinRoutine()
    {
        GameObject client = Instantiate(clientPrefab);
        DontDestroyOnLoad(client);

        yield return new WaitForSeconds(0.2f); // ���� �õ� ���� ����

        SceneManager.LoadScene("RoomScene");
    }

}