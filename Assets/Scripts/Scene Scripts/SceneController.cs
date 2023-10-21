using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    [SerializeField] private GameObject loadingCanvas;

    [SerializeField] private GameObject IceHolder;
    [SerializeField] private GameObject[] IceGameObject;

    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject blackFadeCanvas;

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        SpawnIceElementalAnimations();
    }

    public void LoadScene(string sceneName)
    {
       StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        loadingCanvas.SetActive(true);

        while(!scene.isDone)
        {
            //Animation for loading bar
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        loadingCanvas.SetActive(false);
        
        
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            blackFadeCanvas.SetActive(true);
            yield return new WaitForSeconds(1.1f);
            blackFadeCanvas.SetActive(false);
        }
    }

    private void SpawnIceElementalAnimations()
    {
        int tempIndex = Random.Range(0, IceGameObject.Length);

        Instantiate(IceGameObject[tempIndex], IceHolder.transform);
    }

    public void ToggleGameOverCanvas(bool state) => gameOverCanvas.SetActive(state);
    //public void ToggleFadeCanvas(bool state) => blackFadeCanvas.SetActive(state);
}