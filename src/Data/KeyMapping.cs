namespace Loupedeck.SimHubIntegrationPlugin.Data
{
    public enum EDataKey
    {
        LastLapTime,
        CurrentLapTime,
        FastestLapTime,
        CompactDelta,
        SessionBestLiveDeltaSeconds,
        TargetTime
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
            [EDataKey.FastestLapTime] = new EDataKeyStructure<Object>
            {
                Key = EDataKey.FastestLapTime,
                Type = typeof(TimeSpan),
                ParseFunc = (x) => TimeSpan.Parse(x)
            },
            [EDataKey.CompactDelta] = new EDataKeyStructure<Object>
            {
                Key = EDataKey.CompactDelta,
                Type = typeof(Decimal),
                ParseFunc = (x) => Decimal.Parse(x)
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
                Type = typeof(Double),
                ParseFunc = (x) => Double.Parse(x)
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