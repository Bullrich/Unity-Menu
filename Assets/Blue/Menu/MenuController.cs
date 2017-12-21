using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Net;

// By @Bullrich
namespace Blue.Menu
{
    [HelpURL("https://github.com/Bullrich/Unity-Menu/blob/master/README.md")]
    public class MenuController : MonoBehaviour
    {
        public string DefaultMailDirectory = "example@gmail.com";

        [Header("Do not touch below this point")] public VerticalLayoutGroup buttons;
        public Text debugger, logCounter;
        public Button buttonPrefab;

        private readonly List<LogContainer> _logs = new List<LogContainer>();
        private int logIndex = 0;

        private static MenuController _instance;

        private void Awake()
        {
            _instance = this;
            Application.logMessageReceived += HandleLog;
        }

        private void Start()
        {
            PrintMessage("Unity Menu!", "Up and running!");
            if (!Debug.isDebugBuild)
                Debug.LogWarning("This isn't a development build! You won't be able to read the stack trace!");

            const string eventSystemName = "EventSystem";
            GameObject eventSystem = GameObject.Find(eventSystemName);

            if (eventSystem == null)
            {
                GameObject _eventSystem = new GameObject(eventSystemName);
                _eventSystem.AddComponent<EventSystem>();
                _eventSystem.AddComponent<StandaloneInputModule>();
                _eventSystem.transform.position = Vector3.zero;
            }
        }

        public void CreateButton(Action method, string buttonName)
        {
            Button button = Instantiate(buttonPrefab.gameObject).GetComponent<Button>();
            button.onClick.AddListener(method.Invoke);
            button.GetComponentInChildren<Text>().text = buttonName;
            button.transform.SetParent(buttons.transform);
            button.gameObject.name = buttonName;
            button.transform.localScale = Vector3.one;
        }

        private void ShowLog(LogContainer log, int currentLog)
        {
            debugger.text = log.GetLog();
            logCounter.text = string.Format("{0}/{1}", currentLog + 1, _logs.Count);
        }

        private void PrintMessage(LogContainer log)
        {
            _logs.Add(log);
            logIndex = _logs.Count - 1;
            ShowLog(log, logIndex);
        }

        private static string MyEscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            LogContainer log = new LogContainer(logString, stackTrace, type);
            PrintMessage(log);
        }

        private void Go(int direction)
        {
            logIndex += direction;
            if (logIndex >= _logs.Count)
                logIndex = 0;
            else if (logIndex < 0)
                logIndex = _logs.Count - 1;
            ShowLog(_logs[logIndex], logIndex);
        }

        private void ClearConsoleLogs()
        {
            _logs.Clear();
            PrintMessage("Console clear!", "");
        }


        #region Buttons

        public void GoDown()
        {
            Go(1);
        }

        public void SendEmail()
        {
            string email = DefaultMailDirectory;
            string subject = MyEscapeURL(_logs[logIndex].title);
            string body = MyEscapeURL(_logs[logIndex].message);
            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        }

        public void GoUp()
        {
            Go(-1);
        }

        #endregion

        #region static methods

        private static MenuController GetInstance()
        {
            if (_instance == null)
                throw new Exception("Menu prefab does not exist on the scene or is being called before it's awake.");
            return _instance;
        }

        /// <summary>Add a button to the menu.</summary>
        /// <param name="method">A void method</param>
        /// <param name="buttonName">The name of the button</param>
        public static void AddButton(Action method, string buttonName)
        {
            if (string.IsNullOrEmpty(buttonName))
                throw new Exception("Button name can not be null or empty!");
            GetInstance().CreateButton(method, buttonName);
        }

        /// <summary>Sends a message directly to the console.</summary>
        /// <param name="title">Header of the message</param>
        /// <param name="message">Body of the message</param>
        public static void PrintMessage(string title, string message)
        {
            GetInstance().PrintMessage(new LogContainer(title, message));
        }

        /// <summary>Clear all the logs from memory.</summary>
        public static void ClearLogs()
        {
            GetInstance().ClearConsoleLogs();
        }

        #endregion

        private class LogContainer
        {
            public readonly string
                title, message;

            private readonly string _color;
            private const string NULL = "<color=\"#ff00ffff\"><b>NULL</b></color>";

            private string CheckIfNullOrEmpty(string s)
            {
                const string nullString = "Null";
                const string EMPTY = "<color=\"#800080ff\"><b>EMPTY</b></color>";

                if (s == null || string.Equals(s, nullString))
                    return NULL;
                else if (s.Length == 0)
                    return EMPTY;
                return s;
            }

            public LogContainer(string title, string message, LogType type = LogType.Log)
            {
                this.title = CheckIfNullOrEmpty(title);
                this.message = message ?? NULL;
                switch (type)
                {
                    case LogType.Log:
                        _color = "#000000ff";
                        break;
                    case LogType.Error:
                    case LogType.Exception:
                        _color = "#ff0000";
                        break;
                    case LogType.Warning:
                        _color = "#ffa500ff";
                        break;
                }
            }

            public string GetLog()
            {
                return LimitLength(string.Format("<b><color={0}>{1}</color></b>{2}", _color, title,
                    (message.Length > 0 ? "\n" : "") + message), 340);
            }

            private string LimitLength(string s, int l)
            {
                return s.Substring(0, s.Length > l ? l : s.Length);
            }
        }
    }
}