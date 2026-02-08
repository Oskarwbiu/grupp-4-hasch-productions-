using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera cam;
    public Transform subject;

    Vector2 startPos;
    float startZ;

    Vector2 Travel => (Vector2)cam.transform.position - startPos;
    float DistanceFromSubject => (transform.position - subject.position).z;
    float ClippingPlane => (cam.transform.position.z + (DistanceFromSubject > 0 ? cam.farClipPlane : cam.nearClipPlane));
    float ParallaxFactor => Mathf.Abs(DistanceFromSubject) / ClippingPlane;
    public void Start()
    {
        startPos = transform.position;
        startZ = transform.position.z;
    }

    public void Update()
    {
        Vector2 newPos = startPos + Travel * ParallaxFactor;
        transform.position = new Vector3(newPos.x, newPos.y, startZ);
    }

}
