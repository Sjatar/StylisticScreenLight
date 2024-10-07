using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.EventSystems;

namespace Sja_UI
{
    class Sja_Close : MonoBehaviour
    {
        private Button mainButton;
        private GameObject mainWindow;
        
        public void Start()
        {
            // Get button!
            mainButton = GetComponent(typeof(Button)) as Button;
            mainWindow = transform.parent.gameObject.transform.parent.gameObject;
            // Add listener to if button is pressed. It will run ButtonPressCheck if it is!
            mainButton.onClick.AddListener(delegate { ButtonPressCheck(); });
        }
        
        public void ButtonPressCheck()
        {
            mainWindow.SetActive(false);
        }
    }
}