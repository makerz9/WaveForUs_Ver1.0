using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    // Start is called before the first frame update

    private Coroutine ObstacleSpawnTime;
    [SerializeField] private GameObject[] ObstaclePrefab;

    private void OnEnable()
    {
        ObstacleSpawn();
        ObstacleSpawnTime = StartCoroutine(ObstacleSpawnCoroutine());
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator ObstacleSpawnCoroutine()
    {
        float randomInterval = Random.Range(2.0f, 6.0f);
        yield return new WaitForSeconds(randomInterval);

        int randomPrefab = Random.Range(0, ObstaclePrefab.Length);

        Instantiate(ObstaclePrefab[randomPrefab], new Vector2(25, 0), Quaternion.identity);

        // 0번 프리팹이면 5초 추가 대기
        if (randomPrefab == 0)
        {
            yield return new WaitForSeconds(10f);
        }

        ObstacleSpawnTime = StartCoroutine(ObstacleSpawnCoroutine());
    }

    private void ObstacleSpawn()
    {
        int randomPrefab = Random.Range(0, ObstaclePrefab.Length);

        Instantiate(ObstaclePrefab[randomPrefab], new Vector2(25, 0), Quaternion.identity);
    }


}
