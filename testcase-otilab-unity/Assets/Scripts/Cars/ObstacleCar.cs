using UnityEngine;
using TMPro;

namespace Cars
{
    public class ObstacleCar : MonoBehaviour
    {
        [SerializeField] private TextMeshPro valueText;
        private CarScriptableObject configuration;
        private MeshRenderer meshRenderer;

        public int Value => configuration?.Value ?? 0;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (valueText == null)
            {
                Debug.LogError("ObstacleCar: ValueText is not assigned!");
            }
        }
        public void Initialize(CarScriptableObject config)
        {
            configuration = config;
            if (meshRenderer != null && config.CarMaterial != null)
            {
                meshRenderer.material = config.CarMaterial;
            }
            
            // Update value text with formatted value
            if (valueText != null)
            {
                valueText.text = Helper.NumberFormatter.FormatNumber(config.Value);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<Player.Player>();
                if (player != null)
                {
                    player.HandleCollision(this);
                }
            }
        }
    }
} 