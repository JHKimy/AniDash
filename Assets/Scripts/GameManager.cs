using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.VirtualTexturing;
using Unity.Collections;
using UnityEngine.Rendering.HighDefinition;

public class GameManager : MonoBehaviour
{
    public PlayerState _playerState;



    public GameObject gamePanel;
    public bool isBattle;
    public float playTime;
    public Text playTimeTxt;
    
    public Text playerHealthTxt;
    public RectTransform playerHealthBar;
    public RectTransform playerStaminaBar;

    
    public Text playerKeyTxt;
    public Text maxScore;


    private void Awake()
    {
        // maxScore.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore").ToString());
    }
    void GameStart()
    {

    }
    void Start()
    {
    }


    void Update()
    {
        if (isBattle)
        {
            playTime += Time.deltaTime;
        }
    }
    void LateUpdate()
    {
        // 경과 시간 UI
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);

        playTimeTxt.text = string.Format("{0:00}:{1:00}:{2:00}", hour, min, second);

        // 플레이어 UI
        playerKeyTxt.text = string.Format("{0:n0}", _playerState.key);
        playerHealthTxt.text = _playerState.health.ToString();
        playerHealthBar.localScale = new Vector3(_playerState.health / _playerState.maxHealth, 1, 1);
        playerStaminaBar.localScale = new Vector3(_playerState.stamina / _playerState.maxStamina, 1, 1);


    }
}
