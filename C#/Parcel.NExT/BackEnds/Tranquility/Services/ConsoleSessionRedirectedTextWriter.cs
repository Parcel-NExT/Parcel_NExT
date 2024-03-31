using System.Diagnostics.CodeAnalysis;
using System.Text;
using Tranquility.Sessions;

namespace Tranquility.Services
{
    internal class ConsoleSessionRedirectedTextWriter : TextWriter
    {
        #region Threadlock
        /// <summary>
        /// All routines that touches Console.SetOut() should lock on this
        /// </summary>
        /// <remarks>
        /// We added this to avoid the situation when both the main session and console session are quickly exchanging messages
        /// </remarks>
        public static object ConsoleStateChangeLock = new();
        #endregion

        #region Overrides
        public override Encoding Encoding => Encoding.UTF8;
        public override string NewLine => "\n";
        #endregion

        #region Print Redirections

        public override void Write(bool value)
        {
            ConsoleSession.BroadcastMessages(value.ToString());
        }
        public override void Write(char value)
        {
            ConsoleSession.BroadcastMessages(value.ToString());
        }
        public override void Write(char[] buffer, int index, int count)
        {
            ConsoleSession.BroadcastMessages(new string(buffer.Skip(index).Take(count).ToArray()));
        }
        public override void Write(char[]? buffer)
        {
            ConsoleSession.BroadcastMessages(new string(buffer));
        }
        public override void Write(decimal value)
        {
            ConsoleSession.BroadcastMessages(value.ToString());
        }
        public override void Write(double value)
        {
            ConsoleSession.BroadcastMessages(value.ToString());
        }
        public override void Write(float value)
        {
            ConsoleSession.BroadcastMessages(value.ToString());
        }
        public override void Write(int value)
        {
            ConsoleSession.BroadcastMessages(value.ToString());
        }
        public override void Write(long value)
        {
            ConsoleSession.BroadcastMessages(value.ToString());
        }
        public override void Write(object? value)
        {
            if (value != null)
            {
                string? message = value.ToString();
                if (message != null)
                    ConsoleSession.BroadcastMessages(message);
            }
        }
        public override void Write(ReadOnlySpan<char> buffer)
        {
            ConsoleSession.BroadcastMessages(buffer.ToString());
        }
        public override void Write([StringSyntax("CompositeFormat")] string format, object? arg0)
        {
            ConsoleSession.BroadcastMessages(string.Format(format, arg0));
        }
        public override void Write([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1)
        {
            ConsoleSession.BroadcastMessages(string.Format(format, arg0, arg1));
        }
        public override void Write([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1, object? arg2)
        {
            ConsoleSession.BroadcastMessages(string.Format(format, arg0, arg1, arg2));
        }
        public override void Write([StringSyntax("CompositeFormat")] string format, params object?[] arg)
        {
            ConsoleSession.BroadcastMessages(string.Format(format, arg));
        }
        public override void Write(string? value)
        {
            if (value != null)
                ConsoleSession.BroadcastMessages(value);
        }
        public override void Write(StringBuilder? value)
        {
            if (value != null)
                ConsoleSession.BroadcastMessages(value.ToString());
        }
        public override void Write(uint value)
        {
            ConsoleSession.BroadcastMessages(value.ToString());
        }
        public override void Write(ulong value)
        {
            ConsoleSession.BroadcastMessages(value.ToString());
        }
        public override void WriteLine()
        {
            ConsoleSession.BroadcastMessages(Environment.NewLine);
        }
        public override void WriteLine(bool value)
        {
            ConsoleSession.BroadcastMessages(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(char value)
        {
            ConsoleSession.BroadcastMessages(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(char[] buffer, int index, int count)
        {
            ConsoleSession.BroadcastMessages(new string(buffer.Skip(index).Take(count).ToArray()) + Environment.NewLine);
        }
        public override void WriteLine(char[]? buffer)
        {
            if (buffer != null)
                ConsoleSession.BroadcastMessages(new string(buffer) + Environment.NewLine);
        }
        public override void WriteLine(decimal value)
        {
            ConsoleSession.BroadcastMessages(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(double value)
        {
            ConsoleSession.BroadcastMessages(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(float value)
        {
            ConsoleSession.BroadcastMessages(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(int value)
        {
            ConsoleSession.BroadcastMessages(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(long value)
        {
            ConsoleSession.BroadcastMessages(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(object? value)
        {
            if (value != null)
            {
                string? message = value.ToString();
                if (message != null)
                    ConsoleSession.BroadcastMessages(message + Environment.NewLine);
            }
        }
        public override void WriteLine(ReadOnlySpan<char> buffer)
        {
            ConsoleSession.BroadcastMessages(buffer.ToString() + Environment.NewLine);
        }
        public override void WriteLine([StringSyntax("CompositeFormat")] string format, object? arg0)
        {
            ConsoleSession.BroadcastMessages(string.Format(format, arg0) + Environment.NewLine);
        }
        public override void WriteLine([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1)
        {
            ConsoleSession.BroadcastMessages(string.Format(format, arg0, arg1) + Environment.NewLine);
        }
        public override void WriteLine([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1, object? arg2)
        {
            ConsoleSession.BroadcastMessages(string.Format(format, arg0, arg1, arg2) + Environment.NewLine);
        }
        public override void WriteLine([StringSyntax("CompositeFormat")] string format, params object?[] arg)
        {
            ConsoleSession.BroadcastMessages(string.Format(format, arg) + Environment.NewLine);
        }
        public override void WriteLine(string? value)
        {
            if (value != null)
                ConsoleSession.BroadcastMessages(value + Environment.NewLine);
        }
        public override void WriteLine(StringBuilder? value)
        {
            if (value != null)
                ConsoleSession.BroadcastMessages(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(uint value)
        {
            ConsoleSession.BroadcastMessages(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(ulong value)
        {
            ConsoleSession.BroadcastMessages(value.ToString() + Environment.NewLine);
        }
        #endregion
    }
}
