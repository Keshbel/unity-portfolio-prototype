using UnityEngine;
using UnityEngine.UI;

namespace ExtractionRoom.UI
{
    public sealed class EndGameView : MonoBehaviour
    {
        [SerializeField]
        private Text label;

        public void Configure(Text text)
        {
            label = text;
        }

        public void Hide()
        {
            label.gameObject.SetActive(false);
        }

        public void ShowWin()
        {
            Show("Extraction Complete");
        }

        public void ShowLose()
        {
            Show("Mission Failed");
        }

        private void Show(string message)
        {
            label.text = message;
            label.gameObject.SetActive(true);
        }
    }
}
