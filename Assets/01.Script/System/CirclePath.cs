using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))] // 이 스크립트를 GameObject에 붙일 때 LineRenderer가 없으면 Unity가 자동으로 추가

public class CirclePath : MonoBehaviour
{
    [Header("원 설정")]
    [SerializeField] private float radius = 3f; //원 크기
    [SerializeField] private int segmentCount = 64; // 점 개수, 많을수록 부드러움

    private LineRenderer lineRenderer; //라인 렌더러
    private Vector3[] pathPoints; // 경로를 구성하는 점 배열 (포인트)
    private Vector3[] originalPoints; // 변형 전 원본 경로 (베지어 기준점으로 사용) // ????
    [SerializeField] private CharacterPlayer player; // Inspector에서 연결

    public float Radius => radius; //원 늘이기 최대 크기 관련

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        //라인 렌더러 선언
        SetupLineRenderer();
    }


    // Start is called before the first frame update
    void Start()
    {
        // 원생성
        GenerateCirclePath();
    }


    // 원 + 길 만들기 !!!!!
    #region SetupLineRenderer + GenerateCirclePath


    // 현재 pathPoints 상태를 스냅샷으로 반환 ///???  그리고 스냅샷은 뭐지??
    // 핸들 생성 시점의 경로 상태를 저장하기 위해 사용
    public Vector3[] TakeSnapshot()
    {
        Vector3[] snapshot = new Vector3[pathPoints.Length];
        System.Array.Copy(pathPoints, snapshot, pathPoints.Length);
        return snapshot;
    }




    //라인 생성!!!
    private void SetupLineRenderer()
    {
        lineRenderer.loop = true;       // 시작점과 끝점을 연결해서 닫힌 원을 만듦
        lineRenderer.useWorldSpace = true; //월드스페이스 기준

        lineRenderer.startWidth = 0.1f; // ?
        lineRenderer.endWidth = 0.1f; // ?
        //LineRenderer는 선의 시작점과 끝점 두께를 따로 설정할 수 있어. 둘 다 0.1f로 맞춰서 균일한 두께
    }


    //원형 경로 생성!!
    private void GenerateCirclePath()
    {
        pathPoints = new Vector3[segmentCount]; //아까 점 개수 그거
        //경로 구성 점 배열 갯수 = 64개라는뜻

        originalPoints = new Vector3[segmentCount]; // 원본도 같은 크기로 생성 // ????

        for (int i = 0; i < segmentCount; i++)
        {
            // 전체 360도를 segmentCount로 나눠서 각 점의 각도를 계산
            // Mathf.PI * 2 = 360도 (라디안 단위)
            float angle = (float)i / segmentCount * Mathf.PI * 2f;

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            pathPoints[i] = transform.position + new Vector3(x, y, 0f); // 
            originalPoints[i] = pathPoints[i]; // 원본에도 똑같이 저장 // ????
            //transform.position을 더하는 이유는 원의 중심을 GameObject 위치 기준으로 잡기
        }

        // LineRenderer에 점 배열을 넘겨서 실제로 선을 그림
        lineRenderer.positionCount = segmentCount; //
        //LineRenderer한테 "점을 몇 개 쓸 건지" 먼저 알려주는 거야. 이걸 먼저 선언해야 SetPositions가 배열을 받을 수 있음

        lineRenderer.SetPositions(pathPoints); // "그 공간에 실제 좌표 넣기"
        // pathPoints 배열에 담긴 64개 좌표를 LineRenderer에 한 번에 넘기는 거야. 이걸 받아서 점들을 순서대로 연결해서 선 그림
    }

    // PathHandle에서 변형된 점 배열을 받아서 LineRenderer 갱신
    public void UpdatePath(Vector3[] newPoints)  // ?????
    {
        pathPoints = newPoints;
        lineRenderer.SetPositions(pathPoints);

        if(player != null)
        {
            player.OnPathChanged();
        }
    }


    public void OnDragStarted()
    {
        if (player != null)
            player.OnDragStarted();
    }

    public void OnDragEnded()
    {
        if (player != null)
            player.OnDragEnded();
    }


    // 외부(다른 스크립트)에서 경로 점 배열을 읽을 수 있게 프로퍼티로 공개
    public Vector3[] PathPoints => pathPoints;
    public Vector3[] OriginalPoints => originalPoints; // ???
    public int SegmentCount => segmentCount;




    #endregion

    // Update is called once per frame
    void Update()
    {
        
    }
}
