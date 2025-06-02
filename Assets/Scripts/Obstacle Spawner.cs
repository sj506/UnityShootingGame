using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private GameObject obstaclePrefab; // 생성할 장애물 프리팹을 에디터에서 지정할 수 있게 함
    [SerializeField]
    private float currentSpawnTime = 2f; // 장애물을 생성하는 시간 간격(초)
    [SerializeField]
    private float minX = -2f, maxX = 2f; // 장애물이 생성될 수 있는 가로 위치 범위
    [SerializeField]
    private float minY = -2f, maxY = 5.25f; // 장애물이 생성될 세로 위치(높이)

    private MemoryPool memoryPool;
    private float lastSpawnTime = 0f; // 마지막으로 장애물을 생성한 시간 기록

    private void Awake() 
    {
        memoryPool = new MemoryPool(obstaclePrefab);
    }

    // Start는 게임 시작 시 한 번 실행됨
    void Start()
    {
        // 초기화 작업이 필요하면 여기에 작성 가능
    }

    // Update는 매 프레임마다 실행됨 (초당 여러 번)
    void Update()
    {
        if (gameController.IsGameStart == false) return;

        // 현재 시간과 마지막 생성 시간의 차이가 지정한 시간 간격보다 크면
        if(Time.time - lastSpawnTime > currentSpawnTime)
        {
            lastSpawnTime = Time.time; // 마지막 생성 시간을 현재 시간으로 갱신
            SpawnObject(); // 장애물을 생성하는 함수 호출
        }
    }

    // 장애물을 생성하는 함수
    private void SpawnObject()
    {
        // 장애물이 생성될 위치를 랜덤으로 정함 (x는 minX와 maxX 사이, y는 maxY)
        Vector3 start = new Vector3(Random.Range(minX, maxX), maxY, 0f);
        Vector3 end = new Vector3(Random.Range(minX, maxX), minY, 0f);

        // 장애물 프리팹을 start 위치에 생성함 (회전 없이)
        GameObject clone = memoryPool.ActivatePoolItem(start);
        clone.GetComponent<Obstacle>().Setup(this, start, end);
    }

    public void DeactivateObject(GameObject clone)
    {
        memoryPool.DeactivatePoolItem(clone);
    }
}
