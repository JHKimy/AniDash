using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.VirtualTexturing;
using Unity.Collections;
using UnityEngine.Rendering.HighDefinition;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, PlayerState.PlayerObserver
{
    public PlayerState _playerState;

    public GameObject gamePanel;
    public GameObject clearPanel;
    public bool isBattle;
    public float playTime;


    public TextMeshProUGUI playTimeTxt;
    public TextMeshProUGUI bestTimeTxt;

    public Text playerHealthTxt;
    public RectTransform playerHealthBar;
    public RectTransform playerStaminaBar;

    public Text playerKeyTxt;

    public Button restartButton;


    private void Awake()
    {
        _playerState.AttachObserver(this);
        // maxScore.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore").ToString());
    }
    private void OnDestroy()
    {
        _playerState.DetachObserver(this);
    }
    void Start()
    {
        GameStart();
        restartButton.onClick.AddListener(RestartGame);

    }

    void Update()
    {
        if (isBattle)
        {
            playTime += Time.deltaTime;
        }

        // G 키로 테스트 클리어
        if (Input.GetKeyDown(KeyCode.G))
        {
            GameClear();
        }
    }

    void LateUpdate()
    {

        UpdatePlayTimeUI();
        //// 경과 시간 UI
        //int hour = (int)(playTime / 3600);
        //int min = (int)((playTime - hour * 3600) / 60);
        //int second = (int)(playTime % 60);

        //playTimeTxt.text = string.Format("{0:00}:{1:00}:{2:00}", hour, min, second);

        ////// 플레이어 UI
        ////playerKeyTxt.text = string.Format("{0:n0}", _playerState.key);
        ////playerHealthTxt.text = _playerState.health.ToString();
        ////playerHealthBar.localScale = new Vector3(_playerState.health / _playerState.maxHealth, 1, 1);
        ////playerStaminaBar.localScale = new Vector3(_playerState.stamina / _playerState.maxStamina, 1, 1);


    }
    void GameStart()
    {
        gamePanel.SetActive(true);
        clearPanel.SetActive(false);
        playTime = 0f;
        isBattle = true;

        // BestTime이 없을 때만 초기화
        if (!PlayerPrefs.HasKey("BestTime"))
        {
            PlayerPrefs.SetInt("BestTime", int.MaxValue);
        }
    }

    void GameClear()
    {
        isBattle = false;
        gamePanel.SetActive(false);
        clearPanel.SetActive(true);

        int currentTime = Mathf.FloorToInt(playTime);
        int bestTime = PlayerPrefs.GetInt("BestTime");
        
        // 최고 기록 갱신
        if (currentTime < bestTime)
        {
            bestTime = currentTime;
            PlayerPrefs.SetInt("BestTime", bestTime);
            PlayerPrefs.Save();
        }

        bestTimeTxt.text = "Best Time\n" + FormatTime(bestTime);

        Cursor.visible = true;                          // 마우스 커서 보이게
        Cursor.lockState = CursorLockMode.None;         // 마우스 커서 잠금 해제

    }


    private void UpdatePlayTimeUI()
    {
        int seconds = Mathf.FloorToInt(playTime);
        playTimeTxt.text = FormatTime(seconds);
    }

    private string FormatTime(int totalSeconds)
    {
        int hour = totalSeconds / 3600;
        int min = (totalSeconds % 3600) / 60;
        int sec = totalSeconds % 60;
        return string.Format("{0:00}:{1:00}:{2:00}", hour, min, sec);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



    public void OnPlayerStateChanged(PlayerState playerState)
    {
        playerHealthTxt.text = playerState.health.ToString();
        playerHealthBar.localScale = new Vector3(playerState.health / playerState.maxHealth, 1, 1);
        playerStaminaBar.localScale = new Vector3(playerState.stamina / playerState.maxStamina, 1, 1);
        playerKeyTxt.text = string.Format("{0:n0}", playerState.key);
    }
}
