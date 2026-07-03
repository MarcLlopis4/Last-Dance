using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private AnimationCurve deathPanelCurve;

    private PlayableDirector dir;
    

    void Start()
    {
       
    }

     
    void Update()
    {
        
    }
    public void DeathPanelActivation()
    {
        StartCoroutine(DeathPanelAppear());
    }
    private IEnumerator DeathPanelAppear()
    {
        deathPanel.SetActive(true);
        float timer = deathPanelCurve[deathPanelCurve.length - 1].time;
        float time = 0;
        Color color = deathPanel.GetComponent<Image>().color;

        while (time < timer) 
        { 
            float opacity = deathPanelCurve.Evaluate(time);
            deathPanel.GetComponent<Image>().color = new Color(color.r, color.g, color.b,opacity);

            yield return null;

            time += Time.deltaTime;


        }

        deathPanel.transform.GetChild(0).gameObject.SetActive(true);

            
        yield return null;
    }



    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
