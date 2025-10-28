using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BossSceneController : LevelController
{
    public PlayerHealthController PlayerHealthController;
    public UnityEngine.UI.Slider PlayerHP;

    public PlayerStaminaController PlayerStaminaController;
    public UnityEngine.UI.Slider PlayerStamina;

    public BossHandler bossHandler;
    public UnityEngine.UI.Slider BossHP;
    private void Start()
    {
        Scene current = SceneManager.GetSceneByBuildIndex(1);
        foreach (GameObject g in current.GetRootGameObjects())
        {
            if (g.GetComponentInChildren<BossSceneController>())
            {
                g.GetComponentInChildren<BossSceneController>().Initialize(this);
                return;
            }
        }
    }
    private void Update()
    {
        UpdateSliders();
    }

    public void UpdateSliders()
    {
        PlayerHP.value = PlayerHealthController.GetHealthPercent();
        BossHP.value = bossHandler.BossHpPercent();
        PlayerStamina.value = PlayerStaminaController.GetStaminaPercent();
    }
}
