using System.Reflection;
using MelonLoader;

[assembly: AssemblyTitle(DDSS_LobbyGuard.Properties.BuildInfo.Description)]
[assembly: AssemblyDescription(DDSS_LobbyGuard.Properties.BuildInfo.Description)]
[assembly: AssemblyCompany(DDSS_LobbyGuard.Properties.BuildInfo.Company)]
[assembly: AssemblyProduct(DDSS_LobbyGuard.Properties.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + DDSS_LobbyGuard.Properties.BuildInfo.Author)]
[assembly: AssemblyTrademark(DDSS_LobbyGuard.Properties.BuildInfo.Company)]
[assembly: AssemblyVersion(DDSS_LobbyGuard.Properties.BuildInfo.Version)]
[assembly: AssemblyFileVersion(DDSS_LobbyGuard.Properties.BuildInfo.Version)]
[assembly: MelonInfo(typeof(DDSS_LobbyGuard.MelonMain), 
    DDSS_LobbyGuard.Properties.BuildInfo.Name, 
    DDSS_LobbyGuard.Properties.BuildInfo.Version,
    DDSS_LobbyGuard.Properties.BuildInfo.Author,
    DDSS_LobbyGuard.Properties.BuildInfo.DownloadLink)]
[assembly: MelonGame("StripedPandaStudios", "DDSS")]
[assembly: MelonPriority(int.MinValue)]
[assembly: VerifyLoaderVersion("0.7.1", true)]
[assembly: HarmonyDontPatchAll]