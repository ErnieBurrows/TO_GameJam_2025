using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUIController : MonoBehaviour
{
    public void OnLeaveClicked()
    {
        Debug.Log("<PauseCanvas> Leave clicked.");
        SceneManager.LoadScene("MainMenu");
    }
}
