using System;

namespace Parcel.Neo.Base.DataTypes
{
    [Serializable]
    /// <summary>
    /// This will be a subset of <seealso cref="CacheDataType"/>
    /// </summary>
    public enum DictionaryEntryType
    {
        Number,
        String,
        Boolean
    }

    // TODO: Pending taking a look at all references to CacheDataType.ParcelDataGrid and consolidate implementation requirements - at the moment it looks like to add a new cache data type it takes way too much code changes - ideally we only need to change two places: the CacheDataType enum, a two-way mapping, and the Preview window itself
    // Remark-cz: Do we have to have those? Can we just do raw types? Maybe it's provided just for the sake of front-end (that would make sense because Gospel has something similar?
    // TODO: Pending removing this or moving it entirely to the front-end aka. PreviewWindow. At the moment the GUI (XAML) depends on i because of naive graph input/output definition support - which is quite restricting because it cannot handle arbitrary graph input/output types.
    /// <remarks>
    /// This is front-end level contract and entirely for the purpose of front-ends - unless a particular type implements Parcel.CoreEngine's serialization interface, and the front-end has a way to view that serialized result, all other particular visualization types must be explicitly supported here. That's the only way front-ends can provide reasonable preview for any specific object type (e.g. data grid and images).
    /// </remarks>
    [Serializable]
    public enum CacheDataType
    {
        // Primitive
        Boolean,
        Number,
        String,
        DateTime,
        // Basic Types
        ParcelDataGrid, // Including arrays // TODO/REMARK-cz: in general, even though this is defined here, front-ends (depending on what type of front-end it is) do not necessarily need to depend on Parcel.DataGrid package
        ParcelDataGridDataColumn,
        // BitmapImage, // Not implemented
        // Advanced (Not implemented)
        Generic,
        BatchJob
    }
}