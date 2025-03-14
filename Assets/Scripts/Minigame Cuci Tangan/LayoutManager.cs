using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LayoutManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentScoreTXT;
    protected int currentScore;

    [SerializeField] private TextMeshProUGUI coinGainedTXT;
    protected int coinGained;

    private Fader fader;
    [SerializeField] private float fadeDuration = 1.0f;

    [SerializeField] private GameObject FirstLayout;
    [SerializeField] private GameObject PauseLayout;
    [SerializeField] private GameObject InstructionLayout;
    [SerializeField] private GameObject Background;

    private void Start()
    {
        fader = FindObjectOfType<Fader>();
        Time.timeScale = 0;
        SoundManager.Instance.PlayMusicInList("House");
    }

    public void showCanvas(CanvasBehavior canvas)
    {
        canvas.showCanvas();
    }

    public void hideCanvas(CanvasBehavior canvas)
    {
        canvas.hideCanvas();
    }

    public void showLayout(GameObject layout)
    {
        if (layout)
        {
            layout.SetActive(true);
            StartCoroutine(fader.FadeInGameObject(layout, fadeDuration));
        }
    }

    public void hideLayout(GameObject layout)
    {
        if (layout)
        {
            layout.SetActive(false);
            StartCoroutine(fader.FadeOutGameObject(layout, fadeDuration));
        }
    }

    public void startMinigame()
    {
        hideLayout(FirstLayout);
        showLayout(PauseLayout);
        Time.timeScale = 1;
        SoundManager.Instance.PlayMusicInList("virus banyak");
    }

    public void openInstruksi()
    {
        showLayout(InstructionLayout);
        hideLayout(FirstLayout);
    }

    public void closeInstruksi()
    {
        showLayout(FirstLayout);
        hideLayout(InstructionLayout);
    }

    public void restartMinigame()
    {
        DataPersistenceManager.Instance.saveGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void returnToHomeScreen()
    {
        Time.timeScale = 1;
        DataPersistenceManager.Instance.saveGame();
        SceneManager.LoadScene("House");
    }

}
