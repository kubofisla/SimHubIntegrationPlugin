namespace Loupedeck.SimHubIntegrationPlugin.Actions
{
    using System;
    using Loupedeck.SimHubIntegrationPlugin.Triggers;
    using global::SimHubIntegrationPlugin.Data;
    using Loupedeck.SimHubIntegrationPlugin.Data;

    // Inspired by SimHubIntegrationRoles.cs: inherit from PluginDynamicCommand and add dynamic command support
    public class DashboardValues : PluginDynamicCommand, IDataTrigger
    {
        public DashboardValues() : base()
        {
            this.AddParameter(EDataKey.LastLapTime.ToString(), $"{EDataKey.LastLapTime.ToString()} value", "Dashboard Values");
            SimHubData.Instance.Data.Add(EDataKey.LastLapTime, new Binding<Object>(this));

            this.AddParameter(EDataKey.SessionBestLiveDeltaSeconds.ToString(), $"{EDataKey.SessionBestLiveDeltaSeconds.ToString()} value", "Dashboard Values");
            SimHubData.Instance.Data.Add(EDataKey.SessionBestLiveDeltaSeconds, new Binding<Object>(this));
            this.AddParameter(VirtualKeys.SessionBestLiveDeltaSecondsOnly.ToString(), $"{EDataKey.SessionBestLiveDeltaSeconds.ToString()} seconds value", "Dashboard Values");
            this.AddParameter(VirtualKeys.SessionBestLiveDeltaMiliseconds.ToString(), $"{EDataKey.SessionBestLiveDeltaSeconds.ToString()} miliseconds value", "Dashboard Values");
        }

        enum VirtualKeys
        {
            SessionBestLiveDeltaSecondsOnly,
            SessionBestLiveDeltaMiliseconds
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            switch (actionParameter)
            {
                case nameof(EDataKey.LastLapTime):
                    var value = SimHubData.Instance.Data[Enum.Parse<EDataKey>(actionParameter)].Value;
                    return value?.ToString(@"mm\:ss\.fff") ?? "Last Lap Time";
                case nameof(EDataKey.SessionBestLiveDeltaSeconds):
                    var value1 = SimHubData.Instance.Data[Enum.Parse<EDataKey>(actionParameter)].Value;
                    return value1?.ToString(@"mm\:ss\.fff") ?? "Best Live Delta";
                case nameof(VirtualKeys.SessionBestLiveDeltaSecondsOnly):
                    TimeSpan seconds = SimHubData.Instance.Data[EDataKey.SessionBestLiveDeltaSeconds].Value;
                    return $"{seconds.Seconds}." ?? "Best Live Delta Seconds Only";
                case nameof(VirtualKeys.SessionBestLiveDeltaMiliseconds):
                    TimeSpan miliseconds = SimHubData.Instance.Data[EDataKey.SessionBestLiveDeltaSeconds].Value;
                    return  $"{Math.Abs(miliseconds.Milliseconds):000}" ?? "Best Live Delta Miliseconds";
                default:
                    var value2 = SimHubData.Instance.Data[Enum.Parse<EDataKey>(actionParameter)].Value;
                    return value2?.ToString() ?? $"Not loaded";
            }
        }

        public void Refresh() =>
            // Notify the Loupedeck host that the action's display needs to be updated
            // This will trigger a call to GetCommandDisplayName() and GetCommandImage()
            // for this action instance.
            this.ActionImageChanged();
    }
}