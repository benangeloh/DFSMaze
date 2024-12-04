using System.Xml.Serialization;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public MazeGenerator mazeGenerator;
    private AudioSource audioSource;
    public AudioClip hitForward;
    public AudioClip hitLeft;
    public AudioClip hitBack;
    public AudioClip hitRight;

    private Vector3 targetPosition;
    private bool canMove = true;
    public GameObject winPopup;

    void Start()
    {
        targetPosition = transform.position;
        Vector2Int startGridPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        audioSource = GetComponent<AudioSource>();

        if (!mazeGenerator.IsCellOpen(startGridPos))
        {
            Debug.LogError("Player starting position is not valid! Please move the player to an open cell.");
        }
    }


    void Update ()
    {
        if (canMove)
        {
            HandleMovement();
        }
    }

    void HandleMovement ()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            MovePlayer(Vector3.forward);
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            MovePlayer(Vector3.back);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            MovePlayer(Vector3.left);
        }
        else if (Input.GetKey(KeyCode.RightArrow)  || Input.GetKey(KeyCode.D))
        {
            MovePlayer(Vector3.right);
        }
    }

    void MovePlayer(Vector3 direction)
    {
        Vector3 newPos = targetPosition + direction;
        Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.z));
        Vector2Int movementDirection = new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.z));

        Debug.Log($"Attempting to move to {gridPos}");

        if (gridPos == new Vector2Int(19, 20))
        {
            Debug.Log("Player reached the exit! Congratulations!");
            ShowWinPopup(); // Implement a win celebration here
        }


        if (mazeGenerator.IsCellOpen(gridPos))
        {
            Debug.Log($"Moving to {gridPos}");
            targetPosition = newPos;
            StartCoroutine(MoveToTarget());

            if (gridPos.x == mazeGenerator.width - 2 && gridPos.y == mazeGenerator.height - 1) // Adjust to exit position
            {
                ShowWinPopup();
                Debug.Log("Player reached the exit! Congratulations! LOL.");
            }
        }
        else
        {
            PlayWallHitSound(gridPos, movementDirection);
            Debug.Log($"Cannot move to {gridPos}. It's blocked!");
        }
    }


    System.Collections.IEnumerator MoveToTarget()
    {
        canMove = false;
        Debug.Log($"Moving from {transform.position} to {targetPosition}");

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Debug.Log($"Reached {targetPosition}");
        transform.position = targetPosition;
        canMove = true;
    }

    void PlayWallHitSound(Vector2Int position, Vector2Int direction)
    {
        if (!mazeGenerator.IsCellOpen(position))
        {
            if (direction == Vector2Int.up)
            {
                audioSource.PlayOneShot(hitForward);
            }
            else if (direction == Vector2Int.down)
            {
                audioSource.PlayOneShot(hitBack);
            }
            else if (direction == Vector2Int.left)
            {
                audioSource.PlayOneShot(hitLeft);
            }
            else if (direction == Vector2Int.right)
            {
                audioSource.PlayOneShot(hitRight);
            }
        }
    }

    void ShowWinPopup()
    {
        if (winPopup != null)
        {
            winPopup.SetActive(true);
            Debug.Log("Player wins!");
        }
    }

}
