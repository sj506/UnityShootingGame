using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private Transform left, right; 
    // 왼쪽·오른쪽 한계 지점 (씬에서 오브젝트로 지정)

    [SerializeField]
    private float moveSpeed; 
    // 움직이는 속도

    [SerializeField]
    private Vector3 moveDirection = Vector3.right;
    // 초기 이동 방향 (오른쪽)

    private new Collider2D collider2D;
    private SpriteRenderer spriteRenderer;
    private ParticleSystem dieEffect;

    private void Awake() {
        collider2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        dieEffect = GetComponentInChildren<ParticleSystem>();
        dieEffect.Stop();
    }

    void Start()
    {
        // Start: 게임 시작할 때 한 번만 실행 (여기선 비워둠)
    }

    void Update()
    {
        if (gameController.IsGameStart == false || gameController.IsGameOver == true) return;

        if (Input.GetMouseButtonDown(0)) 
        {
            moveDirection *= -1f;
            // 왼쪽 클릭(또는 모바일 터치)했을 때, 이동 방향 반전
        }

        // 현재 방향이 오른쪽인데 오른쪽 끝에 도달했거나,
        // 현재 방향이 왼쪽인데 왼쪽 끝에 도달했으면,
        // 이동 실행
        if (
            (moveDirection == Vector3.right && transform.position.x >= right.position.x) || 
            (moveDirection == Vector3.left && transform.position.x <= left.position.x)
            )
            {
                moveDirection *= -1f;
                gameController.Score += 2;
            }
        
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            // moveSpeed * Time.deltaTime: 초당 속도로 이동 (프레임 독립)
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Obstacle"))
        {
            collider2D.enabled = false;
            spriteRenderer.enabled = false;
            dieEffect.Play();
            gameController.GameOver();
        }
    }
}
