using System;

namespace Parcel.Neo.Base.Framework
{
    public readonly struct ConnectorCache(object dataObject)
    {
        #region Data
        public object DataObject { get; } = dataObject;
        #endregion

        #region Accessor
        public Type DataType => DataObject.GetType();
        public bool IsAvailable => DataObject != null;
        #endregion
    }
}