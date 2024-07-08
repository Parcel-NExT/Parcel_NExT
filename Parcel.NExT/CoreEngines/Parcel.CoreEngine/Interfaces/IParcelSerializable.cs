using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcel.CoreEngine.Interfaces
{
    /// <summary>
    /// Represents a Parcel serializable feature.
    /// Parcel serializable features are two fold: They can be represented safely as self-contained texts (not necessarily JSON); 
    /// And they can serialize into binary data
    /// Mostly used by specific Payload objects.
    /// </summary>
    public interface IParcelSerializable
    {
    }
}
