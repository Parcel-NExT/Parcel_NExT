namespace Parcel.CoreEngine.Document
{
    public sealed class ParcelNodeInputDefinition
    {
        public ParcelNodeInputDefinition(string name, string source)
        {
            Name = name;
            Source = source;
        }

        public string Name { get; set; }
        public string Source { get; set; }
    }

    public sealed class ParcelNode
    {
        #region Constructors
        public ParcelNode(string name, string target, Dictionary<string, string>? attributes = null)
        {
            Name = name;
            Target = target;
            Attributes = attributes ?? [];
        }
        public ParcelNode(string target, Dictionary<string, string>? attributes = null)
        {
            Name = target.Split(':').Last();
            Target = target;
            Attributes = attributes ?? [];
        }
        #endregion

        #region Properties
        public string Name { get; set; }
        public string Target { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<ParcelNodeInputDefinition> Inputs { get; set; } = [];
        public List<string> Outputs { get; set; } = [];
        #endregion
    }
}
