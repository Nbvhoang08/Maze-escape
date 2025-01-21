using UnityEngine;

public class DirButton : MonoBehaviour
{
    // Start is called before the first frame update
    public Player player;
    public Vector2 dir;
    public Vector3 StartPos;

    void Start()
    {
        player = GetComponentInParent<Player>();
    }
    
    private void OnEnable()
    {
        transform.localPosition = StartPos;
    }
    private void Update()
    {
        transform.localPosition = StartPos;
    }

    public void OnMouseDown()
    {
        if (player.CanMove)
        {
            player.dir = dir;
            player.Move(player.dir);
            player.CanMove = false; 
        }
       
    }
}
