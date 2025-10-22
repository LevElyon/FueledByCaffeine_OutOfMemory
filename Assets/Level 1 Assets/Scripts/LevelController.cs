using UnityEngine;

public class LevelController : MonoBehaviour
{
    public string sceneName;
    protected LevelController levelController;

    public virtual void Initialize(LevelController aLevelCon)
    {
        levelController = aLevelCon;
    }
}
