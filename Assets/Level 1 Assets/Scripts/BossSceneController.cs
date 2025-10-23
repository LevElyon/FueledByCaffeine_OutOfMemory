using UnityEngine;
using UnityEngine.SceneManagement;

public class BossSceneController : LevelController
{
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
}
