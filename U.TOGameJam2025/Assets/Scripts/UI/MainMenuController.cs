using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnStartClicked()
    {
        SceneManager.LoadScene("UI_Test");
    }
}
