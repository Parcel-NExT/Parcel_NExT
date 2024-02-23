using Parcel.NExT.Interpreter.Helpers;

namespace Parcel.NExT.Interpreter.UnitTests
{
    public class MyParentType<TType>
    {
        public class MyNestedType { }
        public class MyNestedType2<TType2>
        {
            public TType ParentTypeMember;
            public TType2 MyOwnMember;
        }
    }

    public class MyOrdinaryType
    {
        public class MyNestedType { }
        public class MyNestedType2<TType>
        {
            public TType MyOwnMember;
        }
    }

    public class TypeNameFormattingTest
    {
        [Fact]
        public void ShouldHandleNestedTypeFine()
        {
            // TODO: This works, but it's not exactly what I wanted.
            // Remark-cz: I want `MyParentType<string>.MyNestedType` to show up exactly as is, not "MyParentType<TType>.MyNestedType<String>"
            Assert.Equal("MyParentType<TType>.MyNestedType<String>", typeof(MyParentType<string>.MyNestedType).GetFormattedName()); // TODO: Weirdly we cannot reflect `string` for parent TType
            Assert.Equal("MyParentType<TType>.MyNestedType2<String, Int32>", typeof(MyParentType<string>.MyNestedType2<int>).GetFormattedName());

            Assert.Equal("MyOrdinaryType.MyNestedType", typeof(MyOrdinaryType.MyNestedType).GetFormattedName());
            Assert.Equal("MyOrdinaryType.MyNestedType2<Int32>", typeof(MyOrdinaryType.MyNestedType2<int>).GetFormattedName());
        }
    }
}
