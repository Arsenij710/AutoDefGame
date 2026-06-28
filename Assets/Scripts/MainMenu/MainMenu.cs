using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _optionsPanel;
    public void PlayGame()
    {

        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenOptions()
    {
        _mainMenuPanel.SetActive(false);
        _optionsPanel.SetActive(true);   
    }

    public void CloseOptions()
    {
        _optionsPanel.SetActive(false);  
        _mainMenuPanel.SetActive(true); 
    }
}
