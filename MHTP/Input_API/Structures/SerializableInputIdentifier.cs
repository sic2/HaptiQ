using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Input_API
{
    // Wrapper for InputIdentifier
    public class SerializableInputIdentifier
    {
        public UInt64 id;
        public List<UInt64> equivalentIDs;
        public InputIdentifier.TYPE type;
        public int dim;

        // Empty dummy constructor, used to make this class serialisable
        public SerializableInputIdentifier() { }

        public InputIdentifier getInputIdentifier()
        {
            InputIdentifier inputIdentifier = new InputIdentifier(this);
            return inputIdentifier;
        }
    }
}
