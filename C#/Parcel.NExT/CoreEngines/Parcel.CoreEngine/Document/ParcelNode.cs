namespace Parcel.CoreEngine.Document
{
    public class ParcelNode
    {
        #region Constructors
        public ParcelNode(string target)
        {
            Target = target;
        }
        #endregion

        #region Properties
        public string Target { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = [];
        #endregion
    }
}
