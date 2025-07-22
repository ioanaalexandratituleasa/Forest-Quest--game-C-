using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;



public class PlayerHealth : MonoBehaviour

{  public int maxLives = 3;
 private int currentLives;
    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public TextMeshProUGUI gameOverText;
    public float restartDelay = 1f;
    private bool isDead = false;

    void Awake()

    {
        currentLives = maxLives;
        Debug.Log("Awake() - currentLives setat la " + currentLives);

    }



    void Start()

    {
        UpdateHearts();
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

    }



    public void TakeDamage()

    { 
          if (isDead) return;
       BunnyController controller = GetComponent<BunnyController>();
        if (controller != null && controller.IsJumping)
        {
            Debug.Log("Ignorăm dauna — iepurele este în săritură.");
            return;
        }



        currentLives--;
        UpdateHearts();



        if (currentLives <= 0)
        {
           isDead = true;
            GameOver();

        }

    }





    void UpdateHearts()

    {
        if (heartImages == null || fullHeart == null || emptyHeart == null)
            return;



        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentLives)
               heartImages[i].sprite = fullHeart;
            else
                heartImages[i].sprite = emptyHeart;

        }

    }



    void GameOver()

    {
        Debug.Log("Game Over");
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(true);
        Invoke("RestartScene", restartDelay);

    }



    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

}