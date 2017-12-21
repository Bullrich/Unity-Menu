using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Blue.Menu.Test
{
    public class RuntimeTest : MonoBehaviour, IPrebuildSetup
    {
        public void Setup()
        {
            Instantiate(Resources.Load("TestPrefab") as GameObject);
        }

        [UnityTest]
        public IEnumerator IsCreatingNewButton()
        {
            yield return new WaitForEndOfFrame();
            const string newButtonName = "TESTING";
            MenuController.AddButton(() => { }, newButtonName);
            yield return new WaitForEndOfFrame();

            GameObject result = GameObject.Find(newButtonName);
            Assert.IsNotNull(result);
        }

        private Text getLogViewer()
        {
            return GameObject.Find("LogMessage").GetComponent<Text>();
        }

        [UnityTest]
        public IEnumerator IsShowingLogs()
        {
            const string logMessage = "This is a log";
            Debug.Log(logMessage);
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(getLogViewer().text.Contains(logMessage));
        }

        [UnityTest]
        public IEnumerator IsShowingMessages()
        {
            const string customMessage = "This is a message";
            MenuController.PrintMessage(customMessage, "");
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(getLogViewer().text.Contains(customMessage));
        }

        [UnityTest]
        public IEnumerator IsShowlingLogsWithCorrectColor()
        {
            const string logColor = "000000ff";
            const string logMessage = "This is a log";
            Debug.Log(logMessage);
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(
                getLogViewer().text.Contains(string.Format("<b><color=#{0}>{1}</color></b>", logColor, logMessage)));
        }

        [UnityTest]
        public IEnumerator IsShowlingWarningWithCorrectColor()
        {
            const string logColor = "ffa500ff";
            const string logMessage = "This is a warning";
            Debug.LogWarning(logMessage);
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(
                getLogViewer().text.Contains(string.Format("<b><color=#{0}>{1}</color></b>", logColor, logMessage)));
        }

        [UnityTest]
        public IEnumerator IsShowlingErrorWithCorrectColor()
        {
            const string logColor = "ff0000";
            const string logMessage = "This is a error";
            LogAssert.Expect(LogType.Error, logMessage);
            Debug.LogError(logMessage);
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(
                getLogViewer().text.Contains(string.Format("<b><color=#{0}>{1}</color></b>", logColor, logMessage)));
        }

        [UnityTest]
        public IEnumerator IsAddingOnClickFunction()
        {
            yield return new WaitForEndOfFrame();
            const string newButtonName = "Change Int Button";
            const int startingExample = 0;
            int numberExample = startingExample;
            MenuController.AddButton(() => { numberExample = 4; }, newButtonName);
            yield return new WaitForEndOfFrame();
            GameObject.Find(newButtonName).GetComponent<Button>().onClick.Invoke();
            Assert.IsTrue(numberExample != startingExample);
        }
    }
}