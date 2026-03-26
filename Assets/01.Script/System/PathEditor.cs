using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathEditor : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private CirclePath circlePath; //원 만드는 스크립트 참조
    [Space(10f)]

    [Header("포인트 설정")]
    [SerializeField] private GameObject handlePrefab;   // 드래그할 포인트 오브젝트
    [SerializeField] private float clickThreshold = 0.3f; // 선 위로 인정하는 거리 범위

    private List<GameObject> spawnedHandles = new List<GameObject>(); // 생성된 핸들 목록

    private Camera mainCamera; //카메라

    private GameManager gameManager => GameManager.Instance;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryCreateHandle();
        }
    }

    #region Create Handle

    private void TryCreateHandle()
    {
        // 마우스 스크린 좌표 -> 월드 좌표 변환
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;



        // 이미 생성된 핸들 근처면 새로 생성하지 않음
        foreach (GameObject handle in spawnedHandles)
        {
            if (Vector3.Distance(mouseWorld, handle.transform.position) <= clickThreshold)
                return; // 기존 핸들 근처 클릭이면 함수 종료
        }




        // pathPoints 배열을 전부 순회하면서 가장 가까운 점 찾기
        Vector3[] points = circlePath.PathPoints; //원 만드는 스크립트에서 선언한거 가져온거임

        int closestIndex = -1; // 초기값을 -1로 설정하는 건 "아직 아무것도 못 찾았다"는 의미야. 배열 인덱스는 0부터 시작
        
        float closestDist = float.MaxValue; // 비교 시작값을 최대로 설정
        // 쓰는이유:  float가 가질 수 있는 최댓값이야. 첫 번째 점과 거리를 비교할 때 "어떤 거리든 이것보다 작다"는 게 보장되니까 비교 시작값



        for (int i = 0; i < points.Length; i++) //아까 선언한 Vector3[] points 배열
        {
            float dist = Vector3.Distance(mouseWorld, points[i]); //
            //두 점 사이의 거리를 계산하는 Unity 내장 함수야. 마우스 위치와 경로의 각 점 사이 거리를 구하는 것

            if (dist < closestDist) // 지금 계산한 거리가 지금까지 찾은 최솟값보다 작으면 갱신하는 거. 루프가 끝나면 자연스럽게 가장 가까운 점이 남음
            {
                closestDist = dist;
                closestIndex = i;
            }
        }



        // 가장 가까운 점이 임계값 안에 있으면 포인트 생성
        if (closestDist <= clickThreshold)
        // 가장 가까운 점을 찾았더라도, 그 거리가 임계값보다 크면 "선 근처를 클릭한 게 아님"으로 판단해. 선에서 멀리 떨어진 곳을 클릭해도 반응하면 안 되니까
        {
            SpawnHandle(closestIndex, points[closestIndex]); //가장 가까운 점의 인덱스 번호와 그 점의 좌표를 넘기는 거
            // 핸들이 경로의 몇 번째 점을 담당하는지" 알기 위해 필요
        }
    }

    private void SpawnHandle(int pointIndex, Vector3 position)
    {
        //if(GameManager.Instance.UseHandle())
        if(gameManager.UseHandle())
        {

            GameObject handle = Instantiate(handlePrefab, position, Quaternion.identity); //핸들 생성

            spawnedHandles.Add(handle); // 목록에 추가

            // 나중에 3단계에서 이 핸들이 어느 인덱스를 담당하는지 알아야 하므로
            // 컴포넌트에 인덱스 정보를 넘겨줌
            PathHandle handleComp = handle.GetComponent<PathHandle>(); // 방금 생성한 핸들 오브젝트에서 PathHandle 스크립트를 꺼내옴
            handleComp.Init(pointIndex, circlePath); // 꺼낸 스크립트에 "너는 몇 번 인덱스 담당이고, 경로 데이터는 이거야" 알려줌
        }


    }


    #endregion



}
