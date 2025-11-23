namespace Loupedeck.SimHubIntegrationPlugin.Data
{
    using System;

    using Loupedeck.SimHubIntegrationPlugin.Triggers;

    public class Binding<T>
    {
        private readonly IDataTrigger _trigger;
        private T _value;

        public Binding(IDataTrigger trigger) => this._trigger = trigger;

        public T Value
        {
            get => this._value;
            set
            {
                this._value = value;
                this.changed();
            }
        }

        private void changed() =>
            // This method can be used to trigger any necessary updates or notifications
            // when the value changes. For example, you might want to notify observers
            // or update the UI.
            this._trigger.Refresh();

        public static implicit operator Binding<T>(Binding<dynamic> v) => throw new NotImplementedException();
    }
}