using System.Collections;         // IEnumerator, Coroutine 같은 기능을 쓰기 위한 네임스페이스
using System.Diagnostics;         // (현재 코드에서는 안 쓰이고 있음)
using Unity.VisualScripting;      // (현재 코드에서는 안 쓰이고 있음)
using UnityEngine;                // Unity 기본 기능 (게임 오브젝트, 트랜스폼 등)

// 장애물(Obstacle) 객체의 동작을 정의한 클래스
public class Obstacle : MonoBehaviour
{
    private ObstacleSpawner spawner;  // 이 장애물을 만든 Spawner(생성기)를 기억해두는 변수

    // 장애물 설정 메서드: 스폰될 때 호출됨
    public void Setup(ObstacleSpawner spawner, Vector3 start, Vector3 end)
    {
        this.spawner = spawner;  // 어떤 Spawner에서 왔는지 저장

        StartCoroutine(Process(start, end));  // 장애물 동작 시작 (코루틴 실행)
    }

    // 장애물의 전체 동작을 순서대로 처리하는 메인 코루틴
    private IEnumerator Process(Vector3 start, Vector3 end) 
    {
        StartCoroutine(nameof(OnRotate));                 // 회전 코루틴 시작 (계속 돈다)
        yield return StartCoroutine(OnMove(start, end)); // 이동 코루틴 실행 (끝날 때까지 기다림)

        StopCoroutine(nameof(OnRotate));                 // 이동이 끝나면 회전 멈춤
        yield return StartCoroutine(nameof(Onscale));    // 스케일(크기 줄이기) 코루틴 실행 (끝날 때까지 기다림)
    }

    // 장애물을 계속 회전시키는 코루틴
    private IEnumerator OnRotate()
    {
        // 랜덤하게 Z축 앞으로 돌지 뒤로 돌지 결정
        Vector3 rotateAxis = Random.value > 0.4f ? Vector3.forward : Vector3.back;
        float rotateSpeed = Random.Range(10f, 720f);  // 회전 속도도 랜덤

        while (true)  // 무한 반복
        {
            transform.Rotate(rotateAxis, rotateSpeed * Time.deltaTime);  // 매 프레임마다 조금씩 회전
            yield return null;  // 한 프레임 기다림
        }
    }

    // 장애물을 일정 시간 동안 시작 위치에서 끝 위치로 이동시키는 코루틴
    private IEnumerator OnMove(Vector3 start, Vector3 end) 
    {
        float moveTime = 2f;  // 이동 시간 (2초)
        float percent = 0f;   // 이동 진행률 (0~1)

        while (percent < 1f)  // 100%에 도달할 때까지 반복
        {
            percent += Time.deltaTime / moveTime;  // 매 프레임마다 진행률 증가
            transform.position = Vector3.Lerp(start, end, percent);  // 위치 보간
            yield return null;  // 한 프레임 기다림
        }
    }

    // 장애물의 크기를 점점 줄이는 코루틴
    private IEnumerator Onscale()
    {
        Vector3 start = Vector3.one;   // 현재 크기 (1,1,1)
        Vector3 end = Vector3.zero;    // 최종 크기 (0,0,0) → 사라짐
        float scaleTime = 0.5f;        // 줄어드는 데 걸리는 시간 (0.5초)
        float percent = 0f;            // 진행률 (0~1)

        while (percent < 1f)
        {
            percent += Time.deltaTime / scaleTime;  // 매 프레임마다 진행률 증가
            transform.localScale = Vector3.Lerp(start, end, percent);  // 크기 보간
            yield return null;  // 한 프레임 기다림
        }
    }
}
