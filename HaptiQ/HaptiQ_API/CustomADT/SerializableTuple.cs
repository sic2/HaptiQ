using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HaptiQ_API
{
    /// <summary>
    /// Tuple does not have a default constructor and 
    /// XMLSerialiser can only serialise objects that have default constructors, 
    /// that's why this class is created
    /// </summary>
    public class SerializableTuple<T1, T2>
    {
        // Needed for XML serialisation
        public SerializableTuple() { }

        public T1 Item1;
        public T2 Item2;

        public SerializableTuple(T1 Item1, T2 Item2)
        {
            this.Item1 = Item1;
            this.Item2 = Item2;
        }

    }
}
