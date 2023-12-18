using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatSelectLevel : MonoBehaviour
{
    public bool enableLevelCheats;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (enableLevelCheats)
        {
            for (int i = (int)KeyCode.Alpha0; i <= (int)KeyCode.Alpha9; i++)
            {
                KeyCode currentKeyCode = (KeyCode)i;

                if (Input.GetKeyDown(currentKeyCode))
                {
                    int index;

                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        index = i - (int)KeyCode.Alpha0 + 10;
                    } else
                    {
                        index = i - (int)KeyCode.Alpha0;

                    }
                    Debug.Log("Switched to level index = " + index);
                    SceneManager.LoadScene(index);
                    break;
                }
            }
        }
    }
}
