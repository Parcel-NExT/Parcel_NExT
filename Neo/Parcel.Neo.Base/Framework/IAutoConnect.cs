using System;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework.ViewModels;

namespace Parcel.Neo.Base.Framework
{
    public interface IAutoConnect
    {
        public bool ShouldHaveAutoConnection { get; }
        public Tuple<ToolboxNodeExport, Vector2D, InputConnector>[] AutoPopulatedConnectionNodes { get; }
    }
}