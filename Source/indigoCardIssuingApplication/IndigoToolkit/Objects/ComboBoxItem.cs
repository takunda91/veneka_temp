using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndigoMigrationToolkit.Objects
{
    public sealed class ComboBoxItem
    {
        public ComboBoxItem(string text, object value)
        {
            this.Text = text;
            this.Value = value;
        }

        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
