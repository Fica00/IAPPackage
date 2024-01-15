using UnityEngine;

namespace ParallaxEffectDemo
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float speed;
        
        private void Update()
        {
            Vector3 _movementDirection = Vector3.zero;
            if (Input.GetKey(KeyCode.A))
            {
                _movementDirection = Vector3.left;
            }   
            else if (Input.GetKey(KeyCode.D))
            {
                _movementDirection = Vector3.right;
            }
            
            transform.position += (_movementDirection*speed);
        }
    }

}
