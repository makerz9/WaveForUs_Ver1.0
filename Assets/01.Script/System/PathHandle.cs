using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHandle : MonoBehaviour
{
    private int pointIndex;       // 내가 담당하는 pathPoints 배열 인덱스 // 내가 몇 번째 경로 점을 담당하는지
    private CirclePath circlePath; // 경로 데이터에 접근하기 위한 참조

    [SerializeField] private int influenceRange = 32;
    // 핸들이 영향을 미치는 범위 (앞뒤로 몇 개의 점까지 휘어지게 할지)

    private Camera mainCamera;
    private bool isDragging = false; // 현재 드래그 중인지 여부


    // 이 핸들이 생성된 시점의 경로 스냅샷
    // CirclePath.originalPoints(최초 원 모양)가 아니라
    // "내가 생성될 당시의 경로 상태"를 기준으로 변형해야
    // 다른 핸들의 변형이 유지됨
    private Vector3[] snapshotPoints;

    // 핸들의 원본 위치 (선 위에 처음 생성된 위치)
    private Vector3 originalHandlePos;




    public void Init(int index, CirclePath path) //Init()은 생성 직후 외부에서 데이터를 주입하는 함수
    {
        pointIndex = index;
        circlePath = path;
        mainCamera = Camera.main;


        // 생성 시점의 경로 상태 스냅샷 저장  //??
        snapshotPoints = circlePath.TakeSnapshot();

        // 생성 시점의 핸들 위치 저장  //??
        originalHandlePos = snapshotPoints[pointIndex];

        // 영향 범위를 전체 경로의 절반으로 설정 (= 전체가 자연스럽게 반응)  //??
        influenceRange = circlePath.SegmentCount / 4; // 절반 -> 1/4로 줄임

    }

    private void OnMouseDown()
    {
        // 이 오브젝트를 클릭했을 때 드래그 시작
        isDragging = true;

        // 드래그 시작 시점의 경로 상태를 새로 스냅샷
        // 다른 핸들이 만든 변형까지 포함한 현재 상태 기준으로 변형 시작
        snapshotPoints = circlePath.TakeSnapshot();

        // snapshotPoints[pointIndex] 대신 실제 핸들 오브젝트 위치로 설정
        originalHandlePos = transform.position;

        circlePath.OnDragStarted(); // 드래그 시작 알림
    }

    private void OnMouseUp()
    {
        // 마우스를 떼면 드래그 종료
        isDragging = false;

        circlePath.OnDragEnded(); // 드래그 끝 알림
    }


    private void SnapToPath()
    {
        Vector3[] points = circlePath.PathPoints; //패스포인트 참조?
        int closestIdx = 0;
        float closestDist = float.MaxValue;

        for (int i = 0; i < points.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, points[i]);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestIdx = i;
            }
        }

        // 핸들 위치를 선 위 가장 가까운 점으로 이동
        transform.position = points[closestIdx];

        // pointIndex도 갱신 (다음 드래그 때 기준점이 정확해짐)
        pointIndex = closestIdx;
    }


    private void Update()
    {
        if (isDragging)
        {
            DragHandle();
        }
        else
        {
            // 드래그 중이 아닐 때는 항상 선 위 pointIndex 위치에 붙어있음
            transform.position = circlePath.PathPoints[pointIndex];
        }
    }

    private void DragHandle()
    {
        // 마우스 위치로 핸들 이동
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;



        // 원본 위치에서 너무 멀리 못 가게 거리 제한
        // circlePath의 radius보다 멀어지면 클램프
        // 거리 제한
        float maxDragDistance = circlePath.Radius * 2f;
        if (Vector3.Distance(mouseWorld, originalHandlePos) > maxDragDistance)
        {
            Vector3 dir = (mouseWorld - originalHandlePos).normalized;
            mouseWorld = originalHandlePos + dir * maxDragDistance;
        }


        // 핸들은 마우스를 따라감 (선 위 고정 없음)
        transform.position = mouseWorld;

        // 핸들 위치 기준으로 주변 경로 점들을 베지어 곡선으로 변형
        // 핸들 위치 기준으로 주변 경로 점들을 베지어 곡선으로 변형
        ApplyBezierDeformation(mouseWorld);


    }

    #region LineMove

    private void ApplyBezierDeformation(Vector3 handlePos)
    {
        int count = snapshotPoints.Length;
        Vector3[] points = new Vector3[count];
        System.Array.Copy(snapshotPoints, points, count);

        // displacement를 originalHandlePos(핸들 실제 위치) 기준으로 계산
        Vector3 displacement = handlePos - originalHandlePos;

        if (displacement.magnitude < 0.01f) return;

        for (int i = 0; i < influenceRange * 2 + 1; i++)
        {
            int idx = (pointIndex - influenceRange + i + count) % count;

            float t = 1f - Mathf.Abs(i - influenceRange) / (float)influenceRange;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            smoothT = smoothT * smoothT;

            points[idx] = snapshotPoints[idx] + displacement * smoothT;
        }

        circlePath.UpdatePath(points);
    }

    #endregion

    public int PointIndex => pointIndex;
}
