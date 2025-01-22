using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.VisualScripting;

public class GamePlay : UICanvas
{
    // Start is called before the first frame update
    public Player player;
    [SerializeField] private Text LevelName;
    void Awake()
    {
        if(player == null)
        {
            player = FindObjectOfType<Player>(true); // true để bao gồm các đối tượng không active
        }
    }
    void Update()
    {
        UpdateLevelText();
        if(player != null &&  player.gameObject.activeInHierarchy)
        {
            UpdateSlider();
        }else
        {
            player = FindObjectOfType<Player>(true); // true để bao gồm các đối tượng không active
        }
        
    }
    public void UpdateSlider()
    {
        // Tính toán tỷ lệ phần trăm
        float targetValue = (float)player.currentItemCount / player.maxItemCount;
        // Dùng DOTween để tạo hiệu ứng slide từ value cũ sang value mới
        slider.DOValue(targetValue, 0.5f); // 0.5f là thời gian hiệu ứng, bạn có thể thay đổi tùy ý
    }
    public void Pause()
    {
        Time.timeScale = 0;
        UIManager.Instance.OpenUI<GamePause>();
        SoundManager.Instance.PlayClickSound();
    }
    private void UpdateLevelText()
    {
        if (LevelName != null)
        {   
            int levelNumber = SceneManager.GetActiveScene().buildIndex;
            LevelName.text = $"Level: {levelNumber:D2}"; // Hiển thị với 2 chữ số, ví dụ: 01, 02
        }   
    }
    public Slider slider;
    
    
}
