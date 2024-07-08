namespace Parcel.CoreEngine.Primitives
{
    /// <summary>
    /// Multi-line string
    /// </summary>
    public struct Text
    {
        public Text() { }
        public Text(string value)
        {
            Value = value;
        }
        public string Value { get; set; }
    }
}
