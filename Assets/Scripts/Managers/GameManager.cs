using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
