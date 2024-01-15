using UnityEngine;

/*
 * This script should be applied to all objects that form background
 * The backgrounds should be child objects of the camera
 * speed of 1 would follow camera 1:1
*/

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private float speed;
    
    private new static Transform camera;
    private float length;
    private float startPos;
    private Transform myTransform;

    private void Awake()
    {
        if (camera==null)
        {
            if (Camera.main != null) camera = Camera.main.transform;
        }

        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        myTransform = transform;
    }

    private void Update()
    {
        var _cameraPosition = camera.position;
        Vector3 _myPosition = myTransform.position;
        
        float _relativeDistance = _cameraPosition.x * (1-speed);
        float _distance = _cameraPosition.x * speed;
        
        myTransform.position = new Vector3(startPos + _distance, _myPosition.y, z: _myPosition.z);
        if (_relativeDistance > startPos + length)
        {
            startPos += length;
        }
        else if (_relativeDistance< startPos-length)
        {
            startPos -= length;
        }
    }
}
