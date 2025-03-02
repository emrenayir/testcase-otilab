using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerSave", menuName = "ScriptableObjects/PlayerSave")]
    public class PlayerSave : ScriptableObject
    {
        [SerializeField] private int topScore;
        
        public int TopScore
        {
            get => topScore;
            set
            {
                if (value > topScore)
                {
                    topScore = value;
                    SaveScore();
                }
            }
        }

        private void SaveScore()
        {
            PlayerPrefs.SetInt("TopScore", topScore);
            PlayerPrefs.Save();
        }

        private void OnEnable()
        {
            topScore = PlayerPrefs.GetInt("TopScore", 0);
        }
    }
} 