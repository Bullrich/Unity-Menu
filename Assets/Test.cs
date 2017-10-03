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
			Blue.Menu.MenuController.AddButton(MenuTest, "Test!");
			Blue.Menu.MenuController.AddButton(MenuTest, "Test!");
			Blue.Menu.MenuController.AddButton(MenuTest, "Test!");
			Blue.Menu.MenuController.AddButton(MenuTest, "Test!");
			Blue.Menu.MenuController.AddButton(MenuTest, "Test!");
		}


		private void MenuTest()
		{
			Debug.Log("Hello!");
		}
	}
}