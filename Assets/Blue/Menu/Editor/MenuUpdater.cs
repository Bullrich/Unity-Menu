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
            CURRENT_VERSION = "0.4";

        [MenuItem("Window/Blue/Menu/Search for updates")]
        public static void SearchUpdate()
        {
            SearchForUpdate(CURRENT_VERSION, USERNAME, REPONAME);
        }
    }
}