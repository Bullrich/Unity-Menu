using UnityEngine;
using UnityEngine.UI;

namespace Blue.Menu.Log
{
    public class LogContainer : MonoBehaviour
    {
        public LogData Data { get; private set; }
        private MenuController _controller;
        [SerializeField] private Text _logText;

        public void Init(MenuController controller)
        {
            _controller = controller;
            _logText = GetComponentInChildren<Text>();
        }

        public void OpenLog()
        {
            _controller.ShowLogDetail(Data);
        }

        public void UpdateData(LogData logData)
        {
            Data = logData;
            _logText.text = Data.GetLogTitle();
        }
    }

    public struct LogData
    {
        public readonly string Title, Message;
        public readonly LogType LogType;
        private readonly string _color;
        private const string Null = "<color=\"#ff00ffff\"><b>NULL</b></color>";

        public LogData(string title, string message, LogType logType = LogType.Log)
        {
            Title = CheckIfNullOrEmpty(title);
            Message = message ?? Null;
            LogType = logType;

            switch (logType)
            {
                case LogType.Log:
                    _color = "#ffffff";
                    break;
                case LogType.Error:
                case LogType.Exception:
                    _color = "#ff0000";
                    break;
                case LogType.Warning:
                    _color = "#ffa500ff";
                    break;
                default:
                    _color = "#ffffff";
                    break;
            }
        }

        private static string CheckIfNullOrEmpty(string s)
        {
            const string nullString = "Null";
            const string empty = "<color=\"#800080ff\"><b>EMPTY</b></color>";

            if (s == null || string.Equals(s, nullString))
                return Null;
            else if (s.Length == 0)
                return empty;
            return s;
        }

        public string GetLogTitle()
        {
            return string.Format("<b><color={0}>{1}</color></b>", _color, LimitLength(Title, 300));
        }

        public string GetLog()
        {
            return string.Format("<b><color={0}>{1}</color></b>{2}", _color, LimitLength(Title, 150),
                LimitLength((Message.Length > 0 ? "\n" : string.Empty) + Message, 150));
        }

        private static string LimitLength(string s, int l)
        {
            if (s.Length > l)
                return s.Substring(0, l - 3) + "...";
            return s;
        }
    }
}