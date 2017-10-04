using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// By @Bullrich

namespace game
{
	public class Test : MonoBehaviour
	{
		private void Start()
		{
			Blue.Menu.MenuController.AddButton(MenuTest, "Log!");
			Blue.Menu.MenuController.AddButton(Warning, "Warning!");
			Blue.Menu.MenuController.AddButton(Error, "Error!");
			
		}


		private void MenuTest()
		{
			Debug.Log("Log!");
		}

		private void Warning(){
			Debug.LogWarning("Warning!");
		}

		private void Error(){
			Debug.LogError("Error!");
		}
	}
}