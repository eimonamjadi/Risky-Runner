using UnityEngine.SceneManagement;
using UnityEngine;

public class Event : MonoBehaviour
{
    public void Replays() {
    	SceneManager.LoadScene("Level");
    }

    public void QuitGame() {
    	Application.Quit();
    }
}
