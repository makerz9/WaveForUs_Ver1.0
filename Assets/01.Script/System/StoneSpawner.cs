using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // <- 추가

public class StoneSpawner : MonoBehaviour
{

    [SerializeField] private GameObject[] stonePrefab;
    private float lastSpawnTime = -0.5f; // 시작 시 즉시 생성 가능하도록

    public string StoneType = "";

    [SerializeField] private GameObject btnNoStone;
    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {


            // 마지막 생성 후 1초가 지났는지 체크
            if (Time.time - lastSpawnTime < 1.0f)
            {
                return;
            }
            // 
            if (StoneType == "Normal")
            {
                // UI 위에 마우스가 있으면 생성 안 함
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
            }

            // spawnPos를 먼저 계산
            Vector2 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Normal 타입일 때만 x < 8 체크
            if (StoneType == "Normal" && spawnPos.x < 8)
            {
                // UI 위에 마우스가 있으면 생성 안 함
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                return;


            }

            // 실제 생성 로직
            GameObject stoneObj = null;

            if (StoneType == "Normal")
            {
                stoneObj = Instantiate(stonePrefab[0], spawnPos, Quaternion.identity);
            }
            else if (StoneType == "Item1")
            {
                stoneObj = Instantiate(stonePrefab[1], spawnPos, Quaternion.identity);
            }
            else if (StoneType == "Item2")
            {
                stoneObj = Instantiate(stonePrefab[2], spawnPos, Quaternion.identity);
            }
            else if (StoneType == "Item3")
            {
                stoneObj = Instantiate(stonePrefab[3], spawnPos, Quaternion.identity);
            }

            // 생성 성공 시 처리
            if (stoneObj != null)
            {
                Stone stoneScript = stoneObj.GetComponent<Stone>();
                if (stoneScript != null)
                {
                    stoneScript.SetSpawnPosition(spawnPos);
                }

                lastSpawnTime = Time.time;

                if (StoneType != "Normal")
                {
                    StoneType = "Normal";
                    btnNoStone.SetActive(true);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Time.timeScale = 10.0f;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Time.timeScale = 1.0f;
        }
    }

}
