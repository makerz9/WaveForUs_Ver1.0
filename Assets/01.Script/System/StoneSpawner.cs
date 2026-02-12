using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSpawner : MonoBehaviour
{

    [SerializeField] private GameObject stonePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // x가 6보다 작으면 생성 안 함
            if (spawnPos.x < 8)
            {
                return; // 함수 종료
            }

            GameObject stoneObj = Instantiate(stonePrefab, spawnPos, Quaternion.identity);

            Stone stoneScript = stoneObj.GetComponent<Stone>();
            if( stoneScript != null )
            {
                stoneScript.SetSpawnPosition(spawnPos);
            }

            Debug.Log($"돌 생성 위치 {spawnPos}");
        }



        if( Input.GetKeyDown(KeyCode.Q) )
        {
            Time.timeScale = 10.0f;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Time.timeScale = 1.0f;
        }

    }

}
