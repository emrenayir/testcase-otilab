using UnityEngine;

namespace Cars
{
    [CreateAssetMenu(fileName = "CarConfig", menuName = "ScriptableObjects/CarConfiguration")]
    public class CarScriptableObject : ScriptableObject
    {
        [SerializeField] private int value;
        [SerializeField] private Material carMaterial;
        [SerializeField] private float speed = 10f;

        public int Value => value;
        public Material CarMaterial => carMaterial;
        public float Speed => speed;
    }
} 