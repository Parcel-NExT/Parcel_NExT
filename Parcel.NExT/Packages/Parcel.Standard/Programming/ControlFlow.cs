namespace Parcel.Standard.Programming
{
    public static class ControlFlow
    {
        /// <summary>
        /// Provides a basic terminated conditional action.
        /// </summary>
        public static void FunctionalBranch(bool condition, Action actionA, Action actionB)
        {
            if (condition)
                actionA();
            else actionB();
        }
    }
}
