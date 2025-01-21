using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class GamePause : UICanvas
{
    // Start is called before the first frame update
    public Sprite OnVolume;
    public Sprite OffVolume;
    
    [SerializeField] private Image buttonImage;
    private RectTransform _panelTransform;
    private void Awake()
    {
        _panelTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        // Đặt kích thước ban đầu và vị trí giữa màn hình
        _panelTransform.localScale = Vector3.zero;
        _panelTransform.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true); // Bỏ qua Time.timeScale
    }  
    void Update()
    {
        UpdateButtonImage();
    }
    public void Resume()
    {
        Time.timeScale = 1;

        UIManager.Instance.CloseUI<GamePause>(0.2f);
            
    }
    public void HomeBtn()
    {
        
        Time.timeScale = 1;
        StartCoroutine(LoadHome());
        SoundManager.Instance.PlayClickSound();
    }
    IEnumerator LoadHome()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Home");
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<ChooseLV>();
    }
    public void SoundBtn()
    {
        SoundManager.Instance.TurnOn = !SoundManager.Instance.TurnOn;
        UpdateButtonImage();
    }
    private void UpdateButtonImage()
    {
        if (SoundManager.Instance.TurnOn)
        {
            buttonImage.sprite = OnVolume;
        }
        else
        {
            buttonImage.sprite = OffVolume;
        }
    }     
}
