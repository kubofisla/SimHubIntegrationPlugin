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

            this._dict = JsonSerializer.Deserialize<Dictionary<String, String>>(this._raw);
            return this._dict ?? [];
        }
    }
}