namespace Parcel.Neo.Base.DataTypes
{
    public enum ConnectorFlowType
    {
        Input,
        Output,
        Knot
    }

    public enum ConnectorShape
    {
        Circle, // Default; Primitive (string and number)
        Triangle, // Boolean
        Square, // Compound data
        RightTriangle, // Control Flow
        RedSquare, // Server Config
    }
}