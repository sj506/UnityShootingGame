using System.Buffers;
using System.Collections.Generic;
using UnityEngine;

// 메모리 풀 클래스: 미리 오브젝트를 만들어두고, 필요할 때 꺼내 쓰고 다시 넣는 방식
public class MemoryPool
{
    // 풀로 관리되는 개별 오브젝트 정보
    private class PoolItem
    {
        public GameObject gameObject;  // 실제 화면에 나타나는 유니티 오브젝트
        private bool isActive;         // 오브젝트가 현재 활성화(켜짐) 상태인지 표시

        // IsActive 프로퍼티: 활성화 상태를 설정하거나 가져올 때 사용
        public bool IsActive
        {
            set
            {
                isActive = value;
                gameObject.SetActive(isActive);  // 유니티 오브젝트의 활성화 상태도 함께 변경
            }
            get => isActive;
        }
    }

    private int increaseCount = 5;  // 오브젝트 부족할 때 추가로 생성할 개수
    private int maxCount;           // 현재 풀에 등록된 총 오브젝트 개수
    private int activeCount;        // 현재 풀에서 사용 중인(활성화된) 오브젝트 개수

    private GameObject poolObject;       // 관리할 오브젝트(프리팹)
    private List<PoolItem> poolItemList; // 풀에 저장된 모든 오브젝트 리스트

    public int MaxCount => maxCount;          // 외부에서 전체 개수를 읽을 수 있게 하는 프로퍼티
    public int ActiveCount => activeCount;    // 외부에서 활성화된 개수를 읽을 수 있게 하는 프로퍼티

    // 화면 밖, 임시로 오브젝트를 보관할 위치 (플레이어가 못 보게 멀리 보내둠)
    private Vector3 tempPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

    // 생성자: 풀 객체를 만들 때 프리팹을 받아서 초기화
    public MemoryPool(GameObject poolObject)
    {
        maxCount = 0;
        activeCount = 0;
        this.poolObject = poolObject;

        poolItemList = new List<PoolItem>();

        InstantiateObjects();  // 처음에 오브젝트들 미리 생성
    }

    // 오브젝트들을 실제로 생성해서 풀에 추가
    public void InstantiateObjects()
    {
        maxCount += increaseCount;  // 전체 개수 증가

        for (int i = 0; i < increaseCount; i++)
        {
            PoolItem poolItem = new PoolItem();

            poolItem.gameObject = GameObject.Instantiate(poolObject);  // 프리팹 복사 생성
            poolItem.gameObject.transform.position = tempPosition;     // 화면 밖으로 보내놓음
            poolItem.IsActive = false;                                // 비활성화 상태로 시작

            poolItemList.Add(poolItem);  // 리스트에 추가
        }
    }

    // 풀에 있는 모든 오브젝트를 완전히 파괴 (게임 끝날 때나 씬 전환 시 사용)
    public void DestroyObjects() 
    {
        if (poolItemList == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject.Destroy(poolItemList[i].gameObject);
        }

        poolItemList.Clear();
    }

    // 풀에서 비활성화된 오브젝트를 꺼내 활성화, 위치를 지정해 반환
    public GameObject ActivatePoolItem(Vector3 position)
    {
        if (poolItemList == null) return null;

        // 모든 오브젝트가 이미 사용 중이면 새로 추가 생성
        if (maxCount == activeCount)
        {
            InstantiateObjects();
        }

        int count = poolItemList.Count;
        for (int i = 0; i < count; i++)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.IsActive == false)  // 비활성화된 오브젝트 찾기
            {
                activeCount++;  // 사용 중 개수 증가

                poolItem.gameObject.transform.position = position;  // 위치 설정
                poolItem.IsActive = true;                          // 활성화

                return poolItem.gameObject;  // 사용 가능한 오브젝트 반환
            }
        }
        return null;   
    }

    // 특정 오브젝트를 다시 풀로 되돌리기 (비활성화 + 화면 밖으로 보내기)
    public void DeactivatePoolItem(GameObject removeObject)
    {
        if (poolItemList == null || removeObject == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; i++)
        {
            PoolItem poolItem = poolItemList[i];   

            if (poolItem.gameObject == removeObject)
            {
                activeCount--;  // 사용 중 개수 감소

                poolItem.IsActive = false;                          // 비활성화
                poolItem.gameObject.transform.position = tempPosition;  // 화면 밖으로 보내기

                return;
            }
        }
    }

    // 풀 안의 모든 오브젝트를 비활성화 (게임 리셋이나 일괄 초기화 시 사용)
    public void DeactivatePoolItem()
    {
        if (poolItemList == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; i++)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObject != null && poolItem.IsActive == true)
            {
                poolItem.IsActive = false;                          // 비활성화
                poolItem.gameObject.transform.position = tempPosition;  // 화면 밖으로 보내기
            }
        }

        activeCount = 0;  // 전체 사용 중 개수 0으로 리셋
    }
}
