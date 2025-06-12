using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SMG
{
    public class Movement : MonoBehaviour
    {
        float _speed = 5f;

        public void Move(Vector2 move)
        {
            transform.Translate(move * _speed * Time.deltaTime);
        }
    }
}

