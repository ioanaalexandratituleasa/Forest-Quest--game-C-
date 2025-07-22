using UnityEngine;
using UnityEngine.SceneManagement; // Asta e important pentru a schimba scenele

public class MainMenuManager : MonoBehaviour
{
    // Această funcție va fi apelată de butonul Play
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // Numele scenei de joc
    }

    // Această funcție va fi apelată de butonul Niveluri
    public void OpenLevelSelect()
    {
        SceneManager.LoadScene("LevelSelectScene"); // Numele scenei de selecție niveluri
    }

    // Această funcție este opțională, pentru un buton de Ieșire din joc
    public void QuitGame()
    {
        Application.Quit(); // Închide aplicația
        Debug.Log("Am ieșit din joc!"); // Acest mesaj va apărea doar în editor, nu în build-ul final
    }
}