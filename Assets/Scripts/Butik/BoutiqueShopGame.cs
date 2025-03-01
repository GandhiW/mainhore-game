using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BoutiqueManager : MonoBehaviour
{
    [SerializeField] private GameObject DialogKasir;
    [SerializeField] private Button RandomizeButton;
    [SerializeField] private GameObject SimpanButton;
    [SerializeField] private GameObject UseClothButton;
    [SerializeField] private GameObject[] curtains;
    [SerializeField] private GameObject[] costumeCharacters;
    [SerializeField] private SO_itemList itemDatabase;
    [SerializeField] private SpriteRenderer playerAvatarRenderer;
    [SerializeField] private Sprite gachaBtnSprite;

    private SO_Skin selectedSkin;
    private bool isRandomizing = false;
    private const int randomizationCost = 1000;
    private List<SO_Skin> allSkins;
    private SO_Skin[] costumerCharacters_data;

    private void Start()
    {
        DialogKasir.SetActive(false);
        RandomizeButton.gameObject.SetActive(false);
        SimpanButton.SetActive(false);
        UseClothButton.SetActive(false);

        RandomizeCostumeInside();
        HideAllCharacters();
        CloseAllCurtains();

        SimpanButton.GetComponent<Button>().onClick.AddListener(SaveSelectedCostume);
        UseClothButton.GetComponent<Button>().onClick.AddListener(UseSelectedCostume);

        SoundManager.Instance.PlayMusicInList("Butik");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DialogKasir.SetActive(true);
            RandomizeButton.GetComponent<Image>().sprite = gachaBtnSprite;
            RandomizeButton.gameObject.SetActive(true);
            RandomizeButton.onClick.AddListener(OnRandomizeClicked);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (RandomizeButton != null)
        {
            DialogKasir.SetActive(false);
            RandomizeButton.GetComponent<Image>().sprite = null;
            RandomizeButton.gameObject.SetActive(false);
            RandomizeButton.onClick.RemoveAllListeners();
        }
    }

    private void RandomizeCostumeInside()
    {
        allSkins = new List<SO_Skin>();
        costumerCharacters_data = new SO_Skin[3];
        foreach (SO_item item in itemDatabase.availItems)
        {
            if (item is SO_Skin skin)
                allSkins.Add(skin);
        }

        if (allSkins.Count < 3)
        {
            Debug.LogError("Jumlah skin kurang dari 3!");
            return;
        }

        List<int> chosenIndices = new List<int>();
        while (chosenIndices.Count < 3)
        {
            int randomIndex = Random.Range(0, allSkins.Count);
            if (!chosenIndices.Contains(randomIndex))
                chosenIndices.Add(randomIndex);
        }

        for (int i = 0; i < costumeCharacters.Length; i++)
        {
            SO_Skin chosenSkin = allSkins[chosenIndices[i]];
            SpriteRenderer spriteRenderer = costumeCharacters[i].GetComponentInChildren<SpriteRenderer>();
            costumerCharacters_data[i] = chosenSkin;

            if (spriteRenderer != null && chosenSkin.sprite != null)
            {
                spriteRenderer.sprite = chosenSkin.sprite;
                Debug.Log("Sprite should be changed");
            }

            costumeCharacters[i].SetActive(false);
        }
    }

    public void OnRandomizeClicked()
    {
        Debug.Log("Randomize button clicked");
        ConfirmationBehavior confirmationPanel = FindAnyObjectByType<ConfirmationBehavior>();
        RandomizeCostumeInside();

        if (confirmationPanel != null)
        {
            confirmationPanel.showConfirmDecorationPanel(
                () => RandomizeCostume(),
                () => Debug.Log("Cancel Buy")
            );
        }
        else
        {
            Debug.Log("Confirmation panel not found!");
        }
    }

    private void RandomizeCostume()
    {
        if (!isRandomizing)
        {
            if (CoinManager.Instance.canSubstractCoin(randomizationCost))
            {
                CoinManager.Instance.substractCoin(randomizationCost); // Koin langsung dikurangi
                SoundManager.Instance.PlaySFXInList("Coin berkurang");
                isRandomizing = true;
                RandomizeButton.gameObject.SetActive(false);
                DialogKasir.SetActive(false);
                StartCoroutine(RandomizeProcess());
            }
            else
            {
                Debug.Log("Koin tidak cukup untuk randomisasi!");
                NotifPanelBehavior notifPanel = FindAnyObjectByType<NotifPanelBehavior>();
                if (notifPanel != null)
                {
                    notifPanel.showCanvas();
                }
                else
                {
                    Debug.Log("Notif panel is not found");
                }
            }
        }
    }

    private IEnumerator RandomizeProcess()
    {
        yield return new WaitForSeconds(1f);
        int chosenIndex = Random.Range(0, costumeCharacters.Length);
        yield return StartCoroutine(OpenCurtain(chosenIndex));
        ShowCharacter(chosenIndex);
        SimpanButton.SetActive(true);
        UseClothButton.SetActive(true);
        selectedSkin = costumerCharacters_data[chosenIndex];
        isRandomizing = false;
    }

    private IEnumerator OpenCurtain(int index)
    {
        Animator curtainAnimator = curtains[index].GetComponent<Animator>();
        if (curtainAnimator != null)
        {
            curtainAnimator.SetTrigger("Open");
            yield return new WaitForSeconds(1.5f);
        }
    }

    private void CloseAllCurtains()
    {
        foreach (GameObject curtain in curtains)
        {
            Animator curtainAnimator = curtain.GetComponent<Animator>();
            if (curtainAnimator != null)
                curtainAnimator.SetTrigger("Close");
        }
    }

    private void ShowCharacter(int index)
    {
        HideAllCharacters();
        costumeCharacters[index].SetActive(true);
    }

    private void HideAllCharacters()
    {
        foreach (GameObject character in costumeCharacters)
            character.SetActive(false);
    }

    private void SaveSelectedCostume()
    {
        if (selectedSkin != null)
        {
            InventoryManager.Instance.AddItem(selectedSkin);
            Debug.Log("Kostum disimpan ke inventory: " + selectedSkin.name);
        }
        SimpanButton.SetActive(false);
        UseClothButton.SetActive(false);
    }

    private void UseSelectedCostume()
    {
        if (selectedSkin != null && selectedSkin.sprite != null && playerAvatarRenderer != null)
        {
            AvatarManager avatarMng = FindAnyObjectByType<AvatarManager>();
            if (avatarMng != null)
            {
                SO_Skin oldSkin = avatarMng.changeSkin(selectedSkin);
                InventoryManager.Instance.AddItem(oldSkin);
            }
            else
            {
                Debug.LogWarning("Avatar Manager not found!");
            }
            //Debug.Log("Kostum telah digunakan: " + selectedSkin.name);
        }
        SimpanButton.SetActive(false);
        UseClothButton.SetActive(false);
    }
}
