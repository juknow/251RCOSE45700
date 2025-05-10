using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TitleUI : MonoBehaviour
{
    public GameObject serverPrefab; // TCPServerUnity 프리팹
    public GameObject clientPrefab; // TCPClient 프리팁

    public void OnHostClicked()
    {
        StartCoroutine(HostRoutine());
    }

    private IEnumerator HostRoutine()
    {
        GameObject server = Instantiate(serverPrefab);
        DontDestroyOnLoad(server);

        yield return new WaitForSeconds(0.2f); // 서버가 먼저 Listen 하도록 대기

        GameObject client = Instantiate(clientPrefab);
        DontDestroyOnLoad(client);

        yield return new WaitForSeconds(0.2f); // 클라이언트가 서버에 접속 시작하도록 잠깐 대기

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

        yield return new WaitForSeconds(0.2f); // 접속 시도 시작 여유

        SceneManager.LoadScene("RoomScene");
    }

}