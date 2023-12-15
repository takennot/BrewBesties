using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMeOnGameStart : MonoBehaviour
{
    //public bool enableLevelCheats;

    void Start()
    {
        SceneManager.LoadScene("PersistentObjects", LoadSceneMode.Additive);

        SceneManager.LoadScene(1);
    }

    private void Update()
    {
       
        
    }
}
