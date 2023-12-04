using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMeOnGameStart : MonoBehaviour
{

    public bool enableLevelCheats;

    void Start()
    {
        SceneManager.LoadScene("PersistentObjects", LoadSceneMode.Additive);

        SceneManager.LoadScene(1);
    }

    private void Update()
    {
        if (enableLevelCheats)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SceneManager.LoadScene(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SceneManager.LoadScene(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SceneManager.LoadScene(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SceneManager.LoadScene(5);
            } else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SceneManager.LoadScene(7);
            } else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SceneManager.LoadScene(8);
            }


        }
        
    }
}
