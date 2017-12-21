using Blue.Updater;
using UnityEditor;

// by @Bullrich

namespace Blue.Menu.Updater
{
    public class MenuUpdater : PluginUpdater
    {
        private const string
            USERNAME = "Bullrich",
            REPONAME = "Unity-Menu",
            CUREENT_VERSION = "0.3";

        [MenuItem("Window/Blue/Menu/Search for updates")]
        public static void SearchUpdate()
        {
            SearchForUpdate(CUREENT_VERSION, USERNAME, REPONAME);
        }
    }
}