using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour
{
    [HideInInspector]
    public bool facingRight = true;         // For determining which way the player is currently facing.
    [HideInInspector]
    public bool jump = false;               // Condition for whether the player should jump.
    [HideInInspector]
    public bool hasTurn = false;
    protected float tilt;

    public float moveForce = 365f;          // Amount of force added to move the player left and right.
    public float maxSpeed = 5f;             // The fastest the player can travel in the x axis.
    public AudioClip[] jumpClips;           // Array of clips for when the player jumps.
    public float jumpForce = 1000f;         // Amount of force added when the player jumps.
    public AudioClip[] taunts;              // Array of clips for when the player taunts.
    public float tauntProbability = 50f;    // Chance of a taunt happening.
    public float tauntDelay = 1f;           // Delay for when the taunt should happen.


    private int tauntIndex;                 // The index of the taunts array indicating the most recent taunt.
    private Transform groundCheck;          // A position marking where to check if the player is grounded.
    private bool grounded = false;          // Whether or not the player is grounded.
    private Animator anim;					// Reference to the player's animator component.
    Transform pivot;
    List<KeyCode> actions = new List<KeyCode>();

    float increment = 0;

    void Awake()
    {
        // Setting up references.
        groundCheck = transform.Find("groundCheck");
        anim = GetComponent<Animator>();
        pivot = transform.Find("Pivot");
    }


    void Update()
    {
        UpdateKeyboardAction(KeyCode.LeftArrow);
        UpdateKeyboardAction(KeyCode.RightArrow);
        UpdateKeyboardAction(KeyCode.UpArrow);
        UpdateKeyboardAction(KeyCode.DownArrow);

        // The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        //// If the jump button is pressed and the player is grounded then the player should jump.
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
            jump = true;
    }


    void FixedUpdate()
    {
        //if (!hasTurn)
        //{
        //    return;
        //}
        // Cache the horizontal input.
        float h = Input.GetAxis("Horizontal");

        //When we move the player using the buttons(not keyboard) -> horizontal movement won't be dected and it's value will be 0
        if (h == 0)
        {
            if (actions.Contains(KeyCode.A))
            {
                if (increment > -1)
                {
                    increment += -0.1f;
                }
                h = increment;
            }
            else if (actions.Contains(KeyCode.D))
            {
                if (increment < 1)
                {
                    increment += 0.1f;
                }
                h = increment;
            }
            else
            {
                increment = 0;
            }
        }
        if (actions.Contains(KeyCode.UpArrow))
        {
            tilt += 1.0f;
        }
        else if (actions.Contains(KeyCode.DownArrow))
        {
            tilt -= 1.0f;
        }

        tilt = Mathf.Clamp(tilt, -75, 75);
        pivot.rotation = Quaternion.Euler(0, 0, facingRight ? tilt : -tilt);

        // The Speed animator parameter is set to the absolute value of the horizontal input.
        anim.SetFloat("Speed", Mathf.Abs(h));

        // If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
        if (h * GetComponent<Rigidbody2D>().velocity.x < maxSpeed)
            // ... add a force to the player.
            GetComponent<Rigidbody2D>().AddForce(Vector2.right * h * moveForce);

        // If the player's horizontal velocity is greater than the maxSpeed...
        if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > maxSpeed)
        {
            // ... set the player's velocity to the maxSpeed in the x axis.
            GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
        }
        // If the input is moving the player right and the player is facing left...
        if (h > 0 && !facingRight)
            // ... flip the player.
            Flip();
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (h < 0 && facingRight)
            // ... flip the player.
            Flip();

        // If the player should jump...
        if (jump)
        {
            // Set the Jump animator trigger parameter.
            anim.SetTrigger("Jump");

            // Play a random jump audio clip.
            int i = Random.Range(0, jumpClips.Length);
            AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);

            // Add a vertical force to the player.
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));

            // Make sure the player can't jump again until the jump conditions from Update are satisfied.
            jump = false;
        }
    }


    protected void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        //pivot.rotation = Quaternion.Euler(0, 0, facingRight ? tilt : -tilt);
    }


    public IEnumerator Taunt()
    {
        // Check the random chance of taunting.
        float tauntChance = Random.Range(0f, 100f);
        if (tauntChance > tauntProbability)
        {
            // Wait for tauntDelay number of seconds.
            yield return new WaitForSeconds(tauntDelay);

            // If there is no clip currently playing.
            if (!GetComponent<AudioSource>().isPlaying)
            {
                // Choose a random, but different taunt.
                tauntIndex = TauntRandom();

                // Play the new taunt.
                GetComponent<AudioSource>().clip = taunts[tauntIndex];
                GetComponent<AudioSource>().Play();
            }
        }
    }


    int TauntRandom()
    {
        // Choose a random index of the taunts array.
        int i = Random.Range(0, taunts.Length);

        // If it's the same as the previous taunt...
        if (i == tauntIndex)
            // ... try another random taunt.
            return TauntRandom();
        else
            // Otherwise return this index.
            return i;
    }

    #region Movement
    private void UpdateKeyboardAction(KeyCode code)
    {
        if (Input.GetKeyDown(code))
        {
            UpdateActionDown(code);
        }
        if (Input.GetKeyUp(code))
        {
            UpdateActionUp(code);
        }
    }

    void UpdateActionDown(KeyCode code)
    {
        if (!actions.Contains(code)) { actions.Add(code); }
    }

    void UpdateActionUp(KeyCode code)
    {
        if (actions.Contains(code)) { actions.Remove(code); }
    }

    public void MoveLeftDown()
    {
        UpdateActionDown(KeyCode.A);
    }

    public void MoveRightDown()
    {
        UpdateActionDown(KeyCode.D);
    }

    public void RotateDownDown()
    {
        UpdateActionDown(KeyCode.DownArrow);
    }

    public void RotateUpDown()
    {
        UpdateActionDown(KeyCode.UpArrow);
    }

    public void MoveLeftUp()
    {
        UpdateActionUp(KeyCode.A);
    }

    public void MoveRightUp()
    {
        UpdateActionUp(KeyCode.D);
    }

    public void RotateDownUp()
    {
        UpdateActionUp(KeyCode.DownArrow);
    }

    public void RotateUpUp()
    {
        UpdateActionUp(KeyCode.UpArrow);
    }
    #endregion

    public void Jump()
    {
        if (grounded)
        {
            jump = true;
        }
    }
}
