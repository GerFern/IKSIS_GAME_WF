using System;
using System.Collections.Generic;
using System.Text;

namespace gtk_test
{
    public class EventArgsValue<TValue>
    {
        public EventArgsValue(TValue value)
        {
            Value = value;
        }

        public TValue Value { get; }
    }
}
