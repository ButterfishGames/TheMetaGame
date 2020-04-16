using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float respawnWait;
    public float preSpawnTime;
    public float postSpawnTime;

    public Vector3 spawnOffset;

    public GameObject enemyPrefab;
    public GameObject linkedEnemy;

    public Sprite idle;
    public Sprite spawning;

    private bool respawning;

    private SpriteRenderer sRend;

    // Start is called before the first frame update
    void Start()
    {
        respawning = false;

        sRend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!respawning && linkedEnemy == null)
        {
            StartCoroutine(Respawn());
        }
    }

    private IEnumerator Respawn()
    {
        respawning = true;

        yield return new WaitForSeconds(respawnWait);

        sRend.sprite = spawning;
        yield return new WaitForSeconds(preSpawnTime);
        linkedEnemy = Instantiate(enemyPrefab, transform.position + spawnOffset, Quaternion.identity);
        switch (GameController.singleton.equipped)
        {
            case GameController.GameMode.fighting:
                linkedEnemy.GetComponent<PFEnemy>().enabled = false;
                FGEnemy temp = linkedEnemy.GetComponent<FGEnemy>();
                temp.enabled = true;

                temp.hitstun = 0;

                Camera cam = FindObjectOfType<Camera>();
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);


                if (GeometryUtility.TestPlanesAABB(planes, linkedEnemy.GetComponent<Collider2D>().bounds))
                {
                    temp.changedInView = true;
                }
                else
                {
                    temp.changedInView = false;
                }

                Rigidbody2D tempRB = linkedEnemy.GetComponent<Rigidbody2D>();
                if (tempRB != null)
                {
                    tempRB.velocity = new Vector2(0.0f, 0.0f);
                    tempRB.gravityScale = 1;
                }

                temp.GetAnimator().SetBool("fighter", true);
                temp.GetAnimator().SetBool("platformer", false);
                break;

            case GameController.GameMode.rpg:
                linkedEnemy.GetComponent<PFEnemy>().enabled = false;
                linkedEnemy.GetComponent<NPC>().enabled = true;

                if (linkedEnemy.GetComponent<Rigidbody2D>() != null)
                {
                    linkedEnemy.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    linkedEnemy.GetComponent<Rigidbody2D>().gravityScale = 0;
                }

                linkedEnemy.transform.position = new Vector3(GameController.GridLocker(linkedEnemy.transform.position.x), GameController.GridLocker(linkedEnemy.transform.position.y), 0);

                RaycastHit2D hit;
                hit = Physics2D.BoxCast(linkedEnemy.transform.position, Vector2.one * 0.975f, 0, Vector2.zero, 0, ~((1 << LayerMask.NameToLayer("Enemy")) + (1 << LayerMask.NameToLayer("Enemy2")) + (1 << LayerMask.NameToLayer("DamageFloor"))));
                if (hit.collider != null)
                {
                    linkedEnemy.transform.position += Vector3.up;
                    linkedEnemy.transform.position = new Vector3(GameController.GridLocker(linkedEnemy.transform.position.x), GameController.GridLocker(linkedEnemy.transform.position.y), 0);
                }
                break;
        }

        yield return new WaitForSeconds(postSpawnTime);
        sRend.sprite = idle;

        respawning = false;
    }
}
