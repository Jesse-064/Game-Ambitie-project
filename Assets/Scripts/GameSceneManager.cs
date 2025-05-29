using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public void loadScene(string scenename)
    {
        SceneManager.LoadScene(scenename);
        Time.timeScale = 1f; // Ensure time scale is reset when loading a new scene
    }
}
