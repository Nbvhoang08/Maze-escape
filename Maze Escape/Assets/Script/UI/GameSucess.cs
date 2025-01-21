using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
public class GameSucess : UICanvas
{
    private RectTransform _panelTransform;
    public Sprite OnVolume;
    public Sprite OffVolume;

    [SerializeField] private Image buttonImage;
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
    public void NextBtn()
    {
        Time.timeScale = 1;
        SoundManager.Instance.PlayClickSound();
        StartCoroutine(NextSence());
    }
    public void LoadNextScene()
    {
        int lvIndex =  SceneManager.GetActiveScene().buildIndex + 1; // Tăng chỉ số level
        string nextSceneName = "LV" + lvIndex;

        // Kiểm tra xem scene tiếp theo có tồn tại hay không
        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            // Nếu không tồn tại scene tiếp theo, quay về Home
            SceneManager.LoadScene("Home");
            UIManager.Instance.CloseAll();
            UIManager.Instance.OpenUI<ChooseLV>();
        }
}
    IEnumerator NextSence()
    {
        yield return new WaitForSeconds(0.3f);
        LoadNextScene();
        UIManager.Instance.CloseUIDirectly<GameSucess>();
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
