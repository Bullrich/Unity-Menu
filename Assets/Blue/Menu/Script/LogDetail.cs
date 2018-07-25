using UnityEngine;
using UnityEngine.UI;

namespace Blue.Menu.Log
{
	public class LogDetail : MonoBehaviour
	{
		private LogData _data;
		private MenuController _controller;

		[SerializeField] private Text _logText;

		public void Init(MenuController controller)
		{
			_controller = controller;
			CloseDetail();
		}
		
		public void CloseDetail()
		{
			gameObject.SetActive(false);
		}

		public void ShowDetail(LogData data)
		{
			gameObject.SetActive(true);
			_data = data;
			_logText.text = data.GetLog();
		}

		public void SendEmail()
		{
			_controller.SendEmail(_data);
		}
	}
}