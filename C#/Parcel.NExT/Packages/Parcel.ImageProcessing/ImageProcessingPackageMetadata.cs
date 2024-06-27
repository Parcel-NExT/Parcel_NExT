using Parcel.Foundational;
using Parcel.Processing.Utilities;

namespace Parcel.Processing
{
    public sealed class ImageProcessingPackageMetadata: ParcelPackageMetadataBase
    {
        public override string HighlevelUsageDocumentation => $$"""
            {{nameof(Stiching.StitchImagesInFolder)}}(@"Input Folder", @"OutputPath.png", new {{nameof(Stiching.StichImageParameters)}}(){
            	TextColor = MagickColors.Black,
            	CaptionBox = new Size(680, 250)
              })
            """;
    }
}
