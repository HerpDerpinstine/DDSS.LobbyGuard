using MelonLoader;
using MelonLoader.Utils;
using System.IO;

namespace DDSS_LobbyGuard
{
    public class MelonMain : MelonPlugin
    {
        public static string _userDataPath { get; private set; }
        public static MelonLogger.Instance _logger { get; private set; }

        public override void OnInitializeMelon()
        {
            // Cache Logger 
            _logger = LoggerInstance;

            // Setup UserData Folder
            _userDataPath = Path.Combine(MelonEnvironment.UserDataDirectory, Properties.BuildInfo.Name);
            if (!Directory.Exists(_userDataPath))
                Directory.CreateDirectory(_userDataPath);


        }
    }
}