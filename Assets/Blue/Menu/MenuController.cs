using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// By @Bullrich
namespace Blue.Menu
{
    public class MenuController : MonoBehaviour
    {
        public string DefaultMailDirectory = "example@gmail.com";

        [Header("Do not touch below this point")] public GridLayoutGroup buttons;
        public Text debugger;
        public Button buttonPrefab;

        private string _messageTitle, _messageBody;

        private static MenuController _instance;

        private void Awake()
        {
            _instance = this;
            Application.logMessageReceived += HandleLog;
        }

        private void Start()
        {
            Debug.Log("Unity Menu!\nMade by @Bullrich");
            if (!Debug.isDebugBuild)
                Debug.LogWarning("This isn't a development build! You won't be able to read the stack trace!");

            const string _eventSystemName = "EventSystem";
            GameObject eventSystem = GameObject.Find(_eventSystemName);

            if (eventSystem == null)
            {
                GameObject _eventSystem = new GameObject(_eventSystemName);
                _eventSystem.AddComponent<EventSystem>();
                _eventSystem.AddComponent<StandaloneInputModule>();
                _eventSystem.transform.position = Vector3.zero;
            }
        }

        /// <summary>Add a button to the menu.</summary>
        /// <param name="method">A void method</param>
        /// <param name="buttonName">The name of the button</param>
        public static void AddButton(Action method, string buttonName)
        {
            if (_instance == null)
                throw new Exception("Menu prefab does not exist on the scene or is being called before it's awake.");
            _instance.CreateButton(method, buttonName);
        }

        public void CreateButton(Action method, string buttonName)
        {
            Button button = Instantiate(buttonPrefab.gameObject).GetComponent<Button>();
            button.onClick.AddListener(method.Invoke);
            button.GetComponentInChildren<Text>().text = buttonName;
            button.transform.SetParent(buttons.transform);
        }

        private void ShowLog(string log)
        {
            debugger.text = log;
        }

        public void SendEmail()
        {
            string email = DefaultMailDirectory;
            string subject = MyEscapeURL(_messageTitle);
            string body = MyEscapeURL(_messageBody);
            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        }

        private static string MyEscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            _messageTitle = logString;
            _messageBody = string.Format("{0}\n{1}", logString, stackTrace);
            string color = "#000000ff";
            if (type == LogType.Warning)
                color = "#ffa500ff";
            else if (type == LogType.Error)
                color = "#ff0000ff";
            ShowLog(string.Format("<b><color={0}>{1}</color></b>\n{2}", color, logString, stackTrace));
        }
    }
}