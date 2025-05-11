using UnityEngine;

public class PauseUIController : MonoBehaviour
{
    public void OnContinueClicked()
    {
        Debug.Log("<PauseCanvas> Continue clicked.");
    }

    public void OnLeaveClicked()
    {
        Debug.Log("<PauseCanvas> Leave clicked.");
    }
}
