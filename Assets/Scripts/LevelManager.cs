using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public int currentLevel = 1;

    public void loadNextScene()
    {
        currentLevel++;
        if (currentLevel > 5)
            currentLevel = 1;

    }
}
