using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CatTractorController : MonoBehaviour {
    Rigidbody2D rb;
    SpriteRenderer sp;
    float horizontal;
    float vertical;

    public float speed = 0.25f;
    public float jump = 5.0f;
    public bool isGrounded1;
    public bool isGrounded2;
    public Transform groundCheck1;
    public Transform groundCheck2;
    public LayerMask groundLayer;

    public TextMeshProUGUI coinsText;
    public int coinsRemaining;

    public ParticleSystem coinEffect;

    public TextMeshProUGUI timerText;
    float remainingTime;
    bool isIntro;
    bool isGameplay;
    bool isOutro;
    bool gameOver;
    int seconds;

    public TextMeshProUGUI instructionText;

    public AudioClip musicSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip startSound;
    AudioSource audioSource;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        remainingTime = 3;
        isIntro = true;
        isGameplay = false;
        isOutro = false;
        gameOver = false;
        int seconds = 3;

        audioSource = GetComponent<AudioSource>();

        coinsRemaining = 6;
        coinsText.text = "Cat Coins Remaining: " + coinsRemaining.ToString();

        instructionText.text = "Collect the Cat Coins in 10 seconds!\nUse W, A, and D, for movement";
    }

    void Update() {
        // If pressing W and Grounded, then jump
        if (Input.GetKeyDown(KeyCode.W)) {
            if (isGrounded1 || isGrounded2) {
                Jump();
            }
        }
        
        if (!gameOver) {
            remainingTime -= Time.deltaTime;
            seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = "Time Remaining: " + seconds.ToString();
        

            if (isIntro) {
                if (seconds <= 0) {
                    isIntro = false;
                    isGameplay = true;
                    remainingTime = 11;
                    seconds = 11;
                    instructionText.text = "";
                }
            }
            if (isGameplay) {
                if (seconds <= 0) {
                    isGameplay = false;
                    isOutro = true;
                    remainingTime = 3;
                    seconds = 3;
                    instructionText.text = "";
                    ActivateLoss();
                }
            }
            if (isOutro) {
                if (seconds <= 0) {
                    gameOver = true;
             }
            }
        } else {
            instructionText.text = "GAME OVER";
        }
    }

    void FixedUpdate() {
        //Code for movement
        if (!gameOver) {
            if (isGameplay) {
                horizontal = Input.GetAxis("Horizontal");
                vertical = Input.GetAxis("Vertical");

                transform.Translate(horizontal * speed, vertical * speed, 0);
                rb.velocity = new Vector2(speed * horizontal, rb.velocity.y);
                FlipPlayerSprite();

                // Checks if player is on the ground
                isGrounded1 = Physics2D.OverlapCircle(groundCheck1.position, 1.0f, groundLayer);
                isGrounded2 = Physics2D.OverlapCircle(groundCheck2.position, 1.0f, groundLayer);
            }
        }
    }

    // Flips player sprite depending on movement direction
    void FlipPlayerSprite() {
        if (rb.velocity.x < -0.1f) {
            sp.flipX = true;
        } else if (rb.velocity.x > 0.1f) {
            sp.flipX = false;
        }
    }

    // Activates jump, only called when able to jump
    void Jump() {
        rb.velocity = Vector2.up * jump;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // If collision with CatCoin
        if (other.gameObject.tag == "CatCoin")
        {
            PickupCoin(other);
        }
    }

    void PickupCoin(Collision2D coin) {
        coinsRemaining--;
        Destroy(coin.gameObject);
        //PlaySound(cogPickupSound); sound
        coinEffect.Play();
        coinsText.text = "Cat Coins Remaining: " + coinsRemaining.ToString();

        if (coinsRemaining <= 0) {
            ActivateWin();
        }
    }

    void ActivateWin() {
        remainingTime = 3;
        seconds = 3;
        isGameplay = false;
        isOutro = true;
        instructionText.text = "Well Done! You win!";
    }

    void ActivateLoss() {
        instructionText.text = "Oh no! You Lost!";
    }
}