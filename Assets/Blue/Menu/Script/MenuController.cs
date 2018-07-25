using System;
using Blue.Menu.Log;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// By @Bullrich
namespace Blue.Menu
{
    [HelpURL("https://github.com/Bullrich/Unity-Menu/blob/master/README.md")]
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private string _defaultMailDirectory = "example@gmail.com";

        [SerializeField] [Tooltip("Limit of logs shown before deleting the old logs")]
        private int _logLimit = 100;

        [SerializeField] private bool _addClearLogsButton = true;

        [Header("Do not touch below this point")] [SerializeField]
        private Button _buttonPrefab;

        [SerializeField] private VerticalLayoutGroup _buttonsContainer, _logsContainer;

        [SerializeField] private LogContainer _logPrefab;
        [SerializeField] private LogDetail _logDetail;

        private LogQueueManager _logManager;

        private static MenuController _instance;

        private void Awake()
        {
            _instance = this;
            Application.logMessageReceived += HandleLog;
            _logManager = new LogQueueManager(_logLimit, _logsContainer, this, _logPrefab);
            _logDetail.Init(this);
            if (_addClearLogsButton)
                CreateButton(ClearConsoleLogs, "Clear logs");
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
                eventSystem = new GameObject(eventSystemName);
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                eventSystem.transform.position = Vector3.zero;
            }
        }

        private void CreateButton(Action method, string buttonName)
        {
            Button button = Instantiate(_buttonPrefab.gameObject).GetComponent<Button>();
            button.onClick.AddListener(method.Invoke);
            button.GetComponentInChildren<Text>().text = buttonName;
            button.transform.SetParent(_buttonsContainer.transform);
            button.transform.SetAsFirstSibling();
            button.gameObject.name = buttonName;
            button.transform.localScale = Vector3.one;
        }

        private void PrintMessage(LogData log)
        {
            _logManager.AddLog(log);
        }

        private static string MyEscapeUrl(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            PrintMessage(new LogData(logString, stackTrace, type));
        }

        private void ClearConsoleLogs()
        {
            _logManager.ClearList();
            PrintMessage("Console clear!", "");
        }

        public void ShowLogDetail(LogData data)
        {
            _logDetail.ShowDetail(data);
        }

        public void SendEmail(LogData data)
        {
            var mail = string.Format("mailto:{0}?subject={1}&body={1}%0D%0A{2}",
                _defaultMailDirectory, MyEscapeUrl(data.Title), MyEscapeUrl(data.Message));
            Application.OpenURL(mail);
        }

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
            GetInstance().PrintMessage(new LogData(title, message));
        }

        /// <summary>Sends a message directly to the console.</summary>
        /// <param name="title">Header of the message</param>
        public static void PrintMessage(string title)
        {
            GetInstance().PrintMessage(new LogData(title, String.Empty));
        }

        /// <summary>Clear all the logs from memory.</summary>
        public static void ClearLogs()
        {
            GetInstance().ClearConsoleLogs();
        }

        #endregion
    }
}