using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Blue.Menu.Log
{
    public class LogQueueManager
    {
        private readonly LogContainer[] _buffer;

        public readonly int Size;
        private int _pointer;
        private bool _loop;
        private readonly VerticalLayoutGroup _logsContainer;
        private readonly MenuController _controller;
        private readonly LogContainer _logPrefab;

        public int Position
        {
            get { return _pointer; }
        }

        public int Count
        {
            get { return _loop ? Size : (_pointer + 1); }
        }

        public bool Looped
        {
            get { return _loop; }
        }

        public LogQueueManager(int maxSize, VerticalLayoutGroup container, MenuController controller,
            LogContainer logPrefab)
        {
            this.Size = maxSize;
            this._buffer = new LogContainer[this.Size];
            _logsContainer = container;
            _controller = controller;
            _logPrefab = logPrefab;
            Debug.Log("Initialized with a max size of " + maxSize);
        }

        public void AddLog(LogData data)
        {
            LogContainer container = GetNextContainer();
            container.UpdateData(data);
        }

        private LogContainer GetNextContainer()
        {
            Advance();
            LogContainer nextContainer = _buffer[_pointer];
            if (nextContainer == null)
            {
                nextContainer = CreateContainer();
                _buffer[_pointer] = nextContainer;
            }

            PutContainerAsFirst(nextContainer);
            return nextContainer;
        }

        private LogContainer CreateContainer()
        {
            var container = Object.Instantiate(_logPrefab, _logsContainer.transform)
                .GetComponent<LogContainer>();
            container.gameObject.name = "log-" + _pointer;
            container.transform.SetParent(_logsContainer.transform);
            container.Init(_controller);
            return container;
        }

        private static void PutContainerAsFirst(LogContainer container)
        {
            container.transform.SetAsFirstSibling();
            container.transform.localScale = Vector3.one;
        }

        private void Advance()
        {
            if (++_pointer >= Size)
            {
                _pointer = 0;
                _loop = true;
                Debug.Log("Looped!");
            }
        }

        public LogContainer GetLastLog()
        {
            return _buffer[_pointer];
        }

        public LogContainer[] GetWholeList()
        {
            return _buffer.Where(a => a != null).ToArray();
        }

        public void ClearList()
        {
            for (int i = 0; i < _buffer.Length; i++)
            {
                if (_buffer[i] != null)
                    Object.Destroy(_buffer[i].gameObject);
                _buffer[i] = null;
            }

            _pointer = 0;
            _loop = false;
        }
    }
}