using UnityEngine;
using UnityEngine.Playables;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private ModuleManager mManager;
    [SerializeField] private PlayableDirector pDirector;
    [SerializeField] private Camera playerCam;
    [SerializeField] private Camera cinematicCam;
    [SerializeField] private GameObject dino;
    [SerializeField] private GameObject egg;
    [SerializeField] private GameObject startUI;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameplayActivation()
    {
        playerCam.enabled = true;
        cinematicCam.enabled = false;
        mManager.enabled = true;
        dino.SetActive(true);
        egg.SetActive(true);
       
    }

    public void activateCInematic()
    {
        startUI.SetActive(false);
        pDirector.Play();
    }
}
