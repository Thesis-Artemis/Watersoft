using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCotroller : MonoBehaviour
{
    [SerializeField] SpriteRenderer _render;
    [SerializeField] float _speed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var size = _render.size;
        size.x =2 +_speed + Time.deltaTime + Time.time;
        _render.size = size;
    }
}
