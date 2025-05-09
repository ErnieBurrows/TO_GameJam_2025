using Unity.VisualScripting;
using UnityEngine;

public class PlaySoundOnUse : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] [Range(0.1f, 1.0f)] private float volume = 1.0f;

    public void PlaySound()
    {
        if(clip == null)
        {
            Debug.LogWarning("Audio clip is not assigned.");
            return;
        }
        
        AudioSource.PlayClipAtPoint(clip, transform.position, volume);
    }
}
