using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemies;  // ������ �迭

    private Transform[] arrEnemyTransform;  // SpawnPoint���� Transform (�۷ι� ����)

    [SerializeField]
    private Transform spawnContainer;  // SpawnContainer ������Ʈ (�θ�)

    [SerializeField]
    private float spawnInterval = 3f;  // ���� ����

    // Start is called before the first frame update
    void Start()
    {
        // SpawnContainer�� �ڽĵ鸸 �迭�� ���� (�ڱ� �ڽ� ����)
        arrEnemyTransform = new Transform[spawnContainer.childCount];
        for (int i = 0; i < spawnContainer.childCount; i++)
        {
            arrEnemyTransform[i] = spawnContainer.GetChild(i);
        }

        StartCoroutine(EnemyRoutine());
    }

    IEnumerator EnemyRoutine()
    {
        yield return new WaitForSeconds(3f);  // ó�� ���� �� ���

        while (true)
        {
            foreach (Transform t in arrEnemyTransform)
            {
                int index = Random.Range(0, enemies.Length);  // 0~2
                SpawnEnemy(t.position, index);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy(Vector3 globalPos, int index)
    {
        Vector3 spawnPos = new Vector3(globalPos.x, spawnContainer.position.y, 0f);  // X�� ��������Ʈ, Y�� GameManager ����
        Instantiate(enemies[index], spawnPos, Quaternion.identity);
    }


}
