using Parcel.CoreEngine.Helpers;

namespace Parcel.CoreEngine.UnitTests
{
    public class UserDiaryEntryWithCoercion
    {
        [Fact]
        public void ParcelShouldBeAbleToHandleInGraphDefinedArchetypes()
        {
            // We are using MiniParcel here to save some typing - it's part of core anyway
            // Remark: Apparently for the purpose of journal entry this may not be the best way to do it as application goes, but: 1) Parcel should be able to handle it, 2) This demo illustrates well the concept of Parcel nodes ARE data, 3) If specified in the GUI rather than using raw API or MiniParcel, things can look VERY different and be way more interactive, fun, and more straightforward - this would enabling developing and using Parcel graphs as application interface
            // Remark: It looks a bit verbose, but in reality this is likely generated in the GUI, because Archetypes are apparently a bit more advanced for hand use
            string outputFile = Path.GetTempFileName();
            var miniParcel = $$"""
            Archetype DiaryEntry :(string)Date :(string)StartTime :(string)EndTime :(string)Tags :(string)Description ~ Type is optional, default is string
            Array 
                ~ We need quotes because those parameters contain spaces in them
                ~ In this mimic of a journal entry, this array will simply have hundreds if not thousands of parameters or more in it - Parcel should be able to handle it without problem, i.e. there is no such a thing as "stack overflow" because those parameters are not handled in a "stack"
                "DiaryEntry(20280101, 00:00, 06:30, Sleep, )" ~ Notice without starting symbol, it defaults to an archetype/class instance constructor for objects
                "DiaryEntry(20280101, 06:30, 08:00, Morning, Getting up)"
                "DiaryEntry(20280101, 08:00, 10:00, \"Reading, Novel\", Read sci-fi novel.)" ~ The syntax here is a bit ugly, so we might consider allowing using arbitrary delimiters for "specifications that may contain space or other special characters" (not necessarily strings), as shown in the line below
                //DiaryEntry(20280101, 10:00, 12:00, 'Research, Science', Research science and "high-tech" stuff.)//
            | AppendFileLine {{outputFile}} "Format:{Date}, {StartTime}-{EndTime}, ({Tags}) {Description}"
            """;
            var result = "DOCUMENT EXECUTION RESULT";
            Assert.Equal("""
                20280101, 00:00-06:30, (Sleep)
                20280101, 06:30-08:00, (Morning) Gettiing up.
                20280101, 08:00-10:00, (Reading, Novel) Read sci-fi novel.
                20280101, 10:00-12:00, (Research, Science) Research science and "high-tech" stuff.
                """, result);
        }
    }
}