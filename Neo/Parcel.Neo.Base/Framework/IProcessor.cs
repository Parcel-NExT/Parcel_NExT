using Parcel.Neo.Base.Framework.ViewModels;

namespace Parcel.Neo.Base.Framework
{
    public interface IProcessor
    {
        /// <remarks>
        /// The concept of a "MainOutput" is needed mostly because we want to provide default Preview behavior.
        /// Conventionally, we could have just taken the first output pin as main output.
        /// </remarks>
        public OutputConnector MainOutput { get; }
        public void Evaluate();
        /// <remarks>
        /// Notice each node can have multiple outputs so it's essential that we provide cache at each output level.
        /// </remarks>
        public ConnectorCache this[OutputConnector cacheConnector] { get; }
        public bool HasCache(OutputConnector cacheConnector);
    }
}