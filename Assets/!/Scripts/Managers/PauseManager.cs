using UnityEngine;


/// <summary>
/// Handles game pause logic, mostly used so other components know when the game is running or not
/// </summary>
public class PauseManager : MonoBehaviour
{
    public static PauseManager instance { get; private set; }

    public bool IsPaused { get; private set; } = false;
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void PauseGame()
    {
        IsPaused = true;
    }

    public void ResumeGame()
    {
        IsPaused = false;
    }
}
