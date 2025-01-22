using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Player : MonoBehaviour
{
    public Tilemap tilemap;
    public float moveSpeed = 5f;
    public Vector3Int _currentCell;
    public bool isMoving = false;
    public Animator anim;
    public SpriteRenderer sprite;
    public Vector2 dir;
    public bool CanMove;
    [SerializeField] private DirButton[] btn;
    public int currentItemCount;
    public int maxItemCount;
    public bool Pass => currentItemCount == maxItemCount;
    public bool CanTakeSword;
    
    // Thêm các biến mới để quản lý teleport
    private bool isTeleporting = false; // Kiểm tra trạng thái teleport
    private bool hasLeftTeleportZone = true; // Kiểm tra nếu Player đã rời khỏi cổng teleport
    private float teleportCooldown = 1f; // Thời gian chờ giữa các lần teleport
    private float lastTeleportTime = 0f; // Lưu thời gian teleport cuối cùng

    public void Awake()
    {
        if (tilemap == null)
        {
            tilemap = FindAnyObjectByType<Tilemap>();
        }
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        maxItemCount = GameObject.FindGameObjectsWithTag("Crow").Length;
        currentItemCount = 0;
    }

    private void Start()
    {
        CanMove = true;
        sprite = GetComponent<SpriteRenderer>();
        _currentCell = tilemap.WorldToCell(transform.position);

        // Lấy tọa độ chính giữa của ô lưới
        Vector3 cellCenterPosition = tilemap.GetCellCenterWorld(_currentCell);

        // Đặt vị trí của đối tượng vào chính giữa ô
        transform.position = cellCenterPosition;
    }

    void Update()
    {
        anim.SetBool("isMoving", isMoving);

        if (Pass && !CanTakeSword)
        {
            CanTakeSword = true;
            Subject.NotifyObservers("TakeSword");
        }

        if (CanMove)
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            Vector2 currentCell = new Vector2(_currentCell.x, _currentCell.y);
            if (Vector2.Distance(position, currentCell) <= 1)
            {
                foreach (Vector2 dir in GetValidDirections(_currentCell))
                {
                    for (int i = 0; i <= btn.Length - 1; i++)
                    {
                        if (dir == btn[i].dir)
                        {
                            btn[i].gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i <= btn.Length - 1; i++)
            {
                btn[i].gameObject.SetActive(false);
            }
        }
    }

    public void Move(Vector2 direction)
    {
        if (isMoving) return;
        StartCoroutine(MoveThroughCells(_currentCell, direction));
    }

    private bool IsValidCell(Vector3Int cell)
    {
        return tilemap.HasTile(cell);
    }

    public List<Vector2> GetValidDirections(Vector3Int currentCell)
    {
        List<Vector2> validDirections = new List<Vector2>();
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        foreach (var dir in directions)
        {
            Vector3Int adjacentCell = currentCell + new Vector3Int((int)dir.x, (int)dir.y, 0);
            if (IsValidCell(adjacentCell))
            {
                validDirections.Add(dir);
            }
        }
        return validDirections;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra cổng teleport và đảm bảo Player có thể teleport lại chỉ khi đã rời khỏi cổng
        if (collision.gameObject.CompareTag("TelePort") && !isTeleporting && hasLeftTeleportZone && Time.time - lastTeleportTime >= teleportCooldown)
        {
            isTeleporting = true; // Đánh dấu trạng thái đang teleport
            lastTeleportTime = Time.time; // Cập nhật thời gian teleport

            StopAllCoroutines(); // Dừng mọi coroutine di chuyển đang diễn ra
            SoundManager.Instance.PlayVFXSound(2);
            // Teleport Player đến điểm đến mới
            TeleportPlayer(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Crow"))
        {
            currentItemCount++;
            Destroy(collision.gameObject);
            SoundManager.Instance.PlayVFXSound(0);
        }
        else if (collision.gameObject.CompareTag("Sword"))
        {
            if (CanTakeSword)
            {
                SoundManager.Instance.PlayVFXSound(1);
                LVManager.Instance.SaveGame();
                StartCoroutine(EndGame());
            }
        }
    }

    // Hàm xử lý teleport
    private void TeleportPlayer(GameObject teleport)
    {
        // Lấy vị trí điểm đến từ đối tượng teleport (có thể gán điểm này trong Inspector)
        Transform destination = teleport.GetComponent<Teleport>().destination;

        if (destination != null)
        {
            // Cập nhật vị trí của Player đến vị trí của destination
            transform.position = destination.position;
            _currentCell = tilemap.WorldToCell(destination.position);  // Cập nhật lại ô trong Tilemap

            // Đánh dấu Player đã teleport, và cần phải di chuyển ra khỏi cổng mới có thể teleport lại
            hasLeftTeleportZone = false;
            isMoving = false;
            CanMove = true;
            // Bắt đầu coroutine để reset trạng thái teleport sau một thời gian chờ
            StartCoroutine(AllowTeleportAfterCooldown());
            
        }
    }

    // Coroutine để reset trạng thái teleport sau thời gian chờ
    private IEnumerator AllowTeleportAfterCooldown()
    {
        // Đợi một thời gian để Player không thể teleport ngay lập tức
        yield return new WaitForSeconds(teleportCooldown);

        // Cho phép teleport lại sau khi chờ xong
        hasLeftTeleportZone = true;
        isTeleporting = false; // Reset trạng thái teleport
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Nếu Player rời khỏi vùng teleport, reset trạng thái để cho phép teleport lại
        if (collision.gameObject.CompareTag("TelePort"))
        {
            hasLeftTeleportZone = true;
        }
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(0.3f);
        UIManager.Instance.OpenUI<GameSucess>();
        Time.timeScale = 0;
    }

    private IEnumerator MoveThroughCells(Vector3Int startCell, Vector2 initialDirection)
    {
        isMoving = true; // Đặt trạng thái di chuyển
        Vector3Int currentCell = startCell; // Lưu ô bắt đầu
        Vector2 currentDirection = initialDirection; // Hướng ban đầu
        bool hasMovedOnce = false; // Cờ để đảm bảo di chuyển ít nhất một bước

        // Lật hướng sprite nếu cần
        if (initialDirection == Vector2.right)
            sprite.flipX = false;
        else if (initialDirection == Vector2.left)
            sprite.flipX = true;

        while (true)
        {
            // Tìm ô tiếp theo theo hướng hiện tại
            Vector3Int nextCell = currentCell + new Vector3Int((int)currentDirection.x, (int)currentDirection.y, 0);

            // Nếu ô tiếp theo không hợp lệ
            if (!IsValidCell(nextCell))
            {
                if (hasMovedOnce) break; // Nếu đã di chuyển ít nhất 1 bước, thì dừng lại
                else yield break; // Nếu chưa di chuyển bước nào, kết thúc coroutine
            }

            // Di chuyển đến ô tiếp theo
            Vector3 startPos = transform.position;
            Vector3 endPos = tilemap.CellToWorld(nextCell) + tilemap.tileAnchor;
            float time = 0;

            while (time < 1f)
            {
                time += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(startPos, endPos, time);
                yield return null;
            }

            // Đảm bảo vị trí chính xác tại ô sau khi di chuyển
            transform.position = endPos;
            currentCell = nextCell; // Cập nhật ô hiện tại
            hasMovedOnce = true; // Đánh dấu rằng đã di chuyển ít nhất một bước

            // Kiểm tra số lượng hướng đi hợp lệ tại ô hiện tại
            List<Vector2> validDirections = GetValidDirections(currentCell);

            // Nếu gặp ngã rẽ (nhiều hơn 2 hướng hợp lệ) hoặc đường cùng (1 hướng hợp lệ)
            if (validDirections.Count != 2)
            {
                _currentCell = currentCell; // Cập nhật lại ô hiện tại
                isMoving = false; // Cho phép gọi lại Move
                CanMove = true;

                yield break; // Kết thúc coroutine
            }
        }

        // Kết thúc di chuyển khi vòng lặp hoàn thành
        _currentCell = currentCell; // Cập nhật lại ô hiện tại
        isMoving = false; // Reset trạng thái
        CanMove = true;
    }

}
