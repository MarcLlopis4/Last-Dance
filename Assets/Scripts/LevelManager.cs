using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private ModuleManager mManager;
    [SerializeField] private PlayableDirector pDirector;
    [SerializeField] private Camera playerCam;
    [SerializeField] private Camera cinematicCam;
    [SerializeField] private GameObject dino;
    [SerializeField] private GameObject egg;
    [SerializeField] private GameObject startUI;
    [SerializeField] private Slider musicS;
    [SerializeField] private Slider sfxS;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] public GameObject pausePanel;
    [SerializeField] public TextMeshProUGUI distance;
   

    void Start()
    {
        AudioManager.audioInstance.PlayMusic("MenuMusic");
        musicS.value = AudioManager.audioInstance.musicsource.volume;
        sfxS.value = AudioManager.audioInstance.sfxsource.volume;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDistance()
    {
        distance.text = mManager.distance.ToString() + " M";
    }

    public void GameplayActivation()
    {
        playerCam.enabled = true;
        cinematicCam.enabled = false;
        mManager.enabled = true;
        dino.SetActive(true);
        egg.SetActive(true);
        distance.transform.parent.gameObject.SetActive(true);
        AudioManager.audioInstance.PlayMusic("Music1");
       
    }

    public void activateCInematic()
    {
        startUI.SetActive(false);
        pDirector.Play();
    }


    public void OpenPanel(GameObject panelToOpen)
    {
        panelToOpen.SetActive(true);
        if(panelToOpen.GetComponentInChildren<Slider>() != null)
        {
            panelToOpen.GetComponentInChildren<Slider>().Select();
            AudioManager.audioInstance.PlaySFX("UIClick");
        }
        else
        {
            panelToOpen.GetComponentInChildren<Button>().Select();
            AudioManager.audioInstance.PlaySFX("UIClick");
        }
    }

    public void ClosePanel(GameObject panelToClose)
    {
        panelToClose.SetActive(false);
    }
    public void OptionsBackpanel()
    {
        if(dino.activeInHierarchy== true)
        {

             optionsPanel.SetActive(false);
             pausePanel.SetActive(true);
               pausePanel.GetComponentInChildren<Button>().Select();
            AudioManager.audioInstance.PlaySFX("UIClick");
        }
        else
        {
            optionsPanel.SetActive(false);
            startUI.SetActive(true);
             startUI.GetComponentInChildren<Button>().Select();
            
        }
    }
       

    public void ReturnGameplay()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    public void ReturnMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void SetMusicVolume(Slider slider)
    {
        AudioManager.audioInstance.musicsource.volume = slider.value;
    }
    public void SetSfxVolume(Slider slider)
    {
        AudioManager.audioInstance.sfxsource.volume = slider.value;
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }

    public void PlayUISoundClick()
    {
        AudioManager.audioInstance.PlaySFX("UIClick");
    }
    public void PlayUISoundSelected()
    {
        AudioManager.audioInstance.PlaySFX("UIHoover");
    }
}
