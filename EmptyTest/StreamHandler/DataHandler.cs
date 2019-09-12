using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EmptyTest.TStreamHandler
{
    public class DataHandler
    {
        public DataHandler(Type containerType, ReadOnlyDoubleDict<int, Type> types, ReadOnlyDoubleDict<int, string> names)
        {
            ContainerType = containerType ?? throw new ArgumentNullException(nameof(containerType));
            Types = types ?? throw new ArgumentNullException(nameof(types));
            Names = names ?? throw new ArgumentNullException(nameof(names));
        }

        public DataHandler(Type containerType, IDictionary<int, Type> types, IDictionary<int, string> names)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));

            if (types == null)
                throw new ArgumentNullException(nameof(types));

            ContainerType = containerType ?? throw new ArgumentNullException(nameof(containerType));

            if (types is ReadOnlyDoubleDict<int, Type> rtypes) Types = rtypes;
            else Types = new ReadOnlyDoubleDict<int, Type>(types);

            if (names is ReadOnlyDoubleDict<int, string> rnames) Names = rnames;
            else Names = new ReadOnlyDoubleDict<int, string>(names);

        }
        public Type ContainerType { get; }
        public ReadOnlyDoubleDict<int, Type> Types { get; }
        public ReadOnlyDoubleDict<int, string> Names { get; }
    }
}
