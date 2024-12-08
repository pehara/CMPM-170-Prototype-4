using UnityEngine;

public class RockGenerator : MonoBehaviour
{
    // Rock prefab to instantiate
    public GameObject rockPrefab;
    public GameObject trashIcon;
    public Sprite[] rockSprites;

    // Minimum and maximum number of rocks to generate
    public int minRocks = 10;
    public int maxRocks = 20;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Ensure the rock prefab is set
        if (rockPrefab != null)
        {
            // Generate a random number of rocks
            int rockCount = Random.Range(minRocks, maxRocks);

            for (int i = 0; i < rockCount; i++)
            {
                // Generate a random position within the screen bounds
                Vector3 randomPosition = new Vector3(
                    Random.Range(0, Screen.width),
                    Random.Range(0, Screen.height),
                    0);

                // Convert screen position to world position
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(randomPosition);
                worldPosition.z = -1f; 

                // Instantiate the rock at the random position
                GameObject rock = Instantiate(rockPrefab, worldPosition, Quaternion.identity);

                RockScript rockScript = rock.GetComponent<RockScript>(); if (rockScript != null)
                {
                    rockScript.trashIcon = trashIcon;
                }

                SpriteRenderer spriteRenderer = rock.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = rockSprites[Random.Range(0, rockSprites.Length)];

                // randomize rotation and scale
                rock.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                rock.transform.localScale = Vector3.one * Random.Range(0.25f, 0.75f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
