namespace Loupedeck.SimHubIntegrationPlugin.Data
{
    using System.Text.Json;

    public class Response(String raw)
    {
        private readonly String _raw = raw ?? throw new ArgumentNullException(nameof(raw));
        private Dictionary<String, String> _dict;

        public Dictionary<String, String> Parse()
        {
            if (this._dict != null)
            {
                return this._dict;
            }

            // Parse into JsonDocument first so we can gracefully handle
            // both string and non-string JSON values.
            using var doc = JsonDocument.Parse(this._raw);
            var root = doc.RootElement;

            var dict = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in root.EnumerateObject())
            {
                // Use ToString() so numbers, booleans etc. become their
                // textual representation, which the downstream parsers
                // (TimeSpan.Parse, Double.Parse, Decimal.Parse) expect.
                dict[property.Name] = property.Value.ToString();
            }

            this._dict = dict;
            return this._dict;
        }
    }
}