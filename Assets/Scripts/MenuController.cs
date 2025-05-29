using UnityEngine;



public class MenuController : MonoBehaviour
{
    public GameObject PauseMenu; // Reference to the menu GameObject
    public GameObject PlayerUI; // Reference to the PlayerUI GameObject
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    public void MenuToggle()
    {
        // Toggle the visibility of the menu
        PauseMenu.SetActive(!PauseMenu.activeSelf);
        PlayerUI.SetActive(!PauseMenu.activeSelf);
        Time.timeScale = PauseMenu.activeSelf ? 0f : 1f;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuToggle();
        }
    }
}
