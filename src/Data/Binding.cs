namespace Loupedeck.SimHubIntegrationPlugin.Data
{
    using System;
    using System.Collections.Generic;

    using Loupedeck.SimHubIntegrationPlugin.Triggers;

    public class Binding<T>
    {
        private readonly List<IDataTrigger> _triggers = new();
        private T _value;

        public Binding(IDataTrigger trigger)
        {
            if (trigger != null)
            {
                this._triggers.Add(trigger);
            }
        }

        public void AddTrigger(IDataTrigger trigger)
        {
            if (trigger != null && !this._triggers.Contains(trigger))
            {
                this._triggers.Add(trigger);
            }
        }

        public T Value
        {
            get => this._value;
            set
            {
                this._value = value;
                this.changed();
            }
        }

        private void changed()
        {
            // Notify all registered triggers that the value has changed
            foreach (var trigger in this._triggers)
            {
                trigger?.Refresh();
            }
        }

        public static implicit operator Binding<T>(Binding<dynamic> v) => throw new NotImplementedException();
    }
}
