using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    bool started = false;
    public 
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!started)
        {
            started = true;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                
            }
        }
    }

    
 
}
