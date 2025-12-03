namespace Loupedeck.SimHubIntegrationPlugin.Data
{
    public enum EDataKey
    {
        LastLapTime,
        CurrentLapTime,
        BestLapTime,
        SessionBest,
        SessionBestLiveDeltaSeconds,
        TargetTime,
        TargetTimeDelta,
        LapInvalidated

    }

    public static class EDataKeyMapping
    {
        public static readonly Dictionary<EDataKey, EDataKeyStructure<Object>> Mapping = new()
        {
            [EDataKey.LastLapTime] = new EDataKeyStructure<Object>
            {
                Key = EDataKey.LastLapTime,
                Type = typeof(TimeSpan),
                ParseFunc = (x) => TimeSpan.Parse(x)
            },
            [EDataKey.CurrentLapTime] = new EDataKeyStructure<Object>
            {
                Key = EDataKey.CurrentLapTime,
                Type = typeof(TimeSpan),
                ParseFunc = (x) => TimeSpan.Parse(x)
            },
            [EDataKey.BestLapTime] = new EDataKeyStructure<Object>
            {
                Key = EDataKey.BestLapTime,
                Type = typeof(TimeSpan),
                ParseFunc = (x) => TimeSpan.Parse(x)
            },
            [EDataKey.SessionBest] = new EDataKeyStructure<Object>
            {
                Key = EDataKey.SessionBest,
                Type = typeof(TimeSpan),
                ParseFunc = (x) => TimeSpan.Parse(x)
            },
            [EDataKey.SessionBestLiveDeltaSeconds] = new EDataKeyStructure<Object>
            {
                Key = EDataKey.SessionBestLiveDeltaSeconds,
                Type = typeof(TimeSpan),
                ParseFunc = (x) => TimeSpan.FromSeconds(Double.Parse(x))
            },
            [EDataKey.TargetTime] = new EDataKeyStructure<Object>
            {
                Key = EDataKey.TargetTime,
                Type = typeof(TimeSpan),
                ParseFunc = (x) => TimeSpan.FromSeconds(Double.Parse(x))
            },
            [EDataKey.TargetTimeDelta] = new EDataKeyStructure<Object>
            {
                Key = EDataKey.TargetTimeDelta,
                Type = typeof(TimeSpan),
                ParseFunc = (x) => TimeSpan.FromSeconds(Double.Parse(x))
            },
            [EDataKey.LapInvalidated] = new EDataKeyStructure<Object>
            {
                Key = EDataKey.LapInvalidated,
                Type = typeof(Boolean),
                ParseFunc = (x) => Boolean.Parse(x)
            }
        };
    }

    public class EDataKeyStructure<T>
    {
        public EDataKey Key { get; set; }
        public Type Type { get; set; }
        public Func<String, T> ParseFunc { get; set; }
    }
}