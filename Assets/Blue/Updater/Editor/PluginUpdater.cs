using UnityEditor;

namespace Blue.Updater.Editor
{
    public abstract class PluginUpdater
    {
        protected static void SearchForUpdate(string currentVersion, string userName, string repoName)
        {
            BlueUpdater updater = new BlueUpdater(userName, repoName);

            if (updater.IsNewVersionAvailable(currentVersion))
            {
                if (EditorUtility.DisplayDialog(repoName, "There is a new version available", "Download", "Cancel"))
                    DownloadLatest(updater);
            }
            else
                EditorUtility.DisplayDialog(repoName, "You are up to date", "Ok");
        }

        private static void DownloadLatest(BlueUpdater updt)
        {
            updt.DownloadPackage();
        }
    }
}