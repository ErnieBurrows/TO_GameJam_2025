using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitCanvasHandler : MonoBehaviour
{
    public void OnStayClicked()
    {
        ExitPlatform.StayClicked();
    }

    public void OnEndClicked()
    {
        GameStateManager.Instance.GameEnd();
    }
}
