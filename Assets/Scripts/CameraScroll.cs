using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class CameraScroll : MonoBehaviour
{
    [SerializeField]
    float speed = 1.0f;
    Vector2 direction = Vector2.right;
    BoxCollider2D[] borders = new BoxCollider2D[4];
    public PathCreator cameraRail;

    float railDistance = 0.0f;

    void Start()
    {
        BoxCollider2D up = gameObject.AddComponent<BoxCollider2D>();
        BoxCollider2D right = gameObject.AddComponent<BoxCollider2D>();
        BoxCollider2D down = gameObject.AddComponent<BoxCollider2D>();
        BoxCollider2D left = gameObject.AddComponent<BoxCollider2D>();

        borders[0] = up;
        borders[1] = right;
        borders[2] = down;
        borders[3] = left;

        UpdateCollisionBorders();
    }

    // Update is called once per frame
    void Update()
    {
        railDistance += speed * Time.deltaTime;

        var cameraPos = Camera.main.gameObject.transform.position;
        //cameraPos += speed * Time.deltaTime * (Vector3) direction;
        Vector2 railPosition = cameraRail.path.GetPointAtDistance(railDistance);
        cameraPos = new Vector3(railPosition.x, railPosition.y, cameraPos.z);
        Camera.main.gameObject.transform.position = cameraPos;
        // Camera.main.orthographicSize -= Time.deltaTime * 0.1f;
        UpdateCollisionBorders();
    }

    void UpdateCollisionBorders()
    {
        Vector2 cameraPos = Camera.main.transform.position;

        Vector2 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z)) - (Vector3) cameraPos;
        Vector2 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)) - (Vector3)cameraPos;
        Vector2 topLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.transform.position.z)) - (Vector3)cameraPos;
        Vector2 bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, Camera.main.transform.position.z)) - (Vector3)cameraPos;

        float height = topRight.y - bottomRight.y;
        float width = topRight.x - topLeft.x;

        borders[0].offset = new Vector2(0, topRight.y + 1);
        borders[0].size = new Vector2(width, 2);

        borders[1].offset = new Vector2(topRight.x + 1, 0);
        borders[1].size = new Vector2(2, height);

        borders[2].offset = new Vector2(0, bottomRight.y - 1);
        borders[2].size = new Vector2(width, 2);

        borders[3].offset = new Vector2(bottomLeft.x - 1, 0);
        borders[3].size = new Vector2(2, height);

    }
}
