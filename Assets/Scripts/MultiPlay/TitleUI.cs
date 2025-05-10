using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    public GameObject serverPrefab; // TCPServerUnity 프리팹
    public GameObject clientPrefab; // TCPClient 프리팁

    public void OnHostClicked()
    {
        // 1. TCP 서버 실행
        if (serverPrefab != null)
        {
            GameObject server = Instantiate(serverPrefab);
            DontDestroyOnLoad(server);
            Debug.Log("[TitleUI] 서버 인스턴스 생성됨");
        }

        // 2. 클라이언트로 자기 자신도 접속
        if (clientPrefab != null)
        {
            GameObject client = Instantiate(clientPrefab);
            DontDestroyOnLoad(client);
            Debug.Log("[TitleUI] 호스트도 클라이언트로 접속됨");
        }

        // 3. 룸 씬으로 이동
        SceneManager.LoadScene("RoomScene");
    }

    public void OnJoinClicked()
    {
        // 필요 시 클라이언트 프리팹도 DontDestroyOnLoad로 유지
        if (clientPrefab != null)
        {
            GameObject client = Instantiate(clientPrefab);
            DontDestroyOnLoad(client);
            Debug.Log("[TitleUI] 클라이언트 인스턴스 생성됨");
        }

        SceneManager.LoadScene("RoomScene");
    }
}