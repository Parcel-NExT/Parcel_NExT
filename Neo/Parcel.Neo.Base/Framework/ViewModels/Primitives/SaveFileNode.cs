using System.IO;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Framework.ViewModels.Primitives
{
    public class SaveFileNode: StringNode
    {
        #region View Components
        public string Path
        {
            get => _value;
            set => SetField(ref _value, value);
        }
        #endregion
        
        #region Node Interface
        private readonly OutputConnector _filePathOutput = new OutputConnector(typeof(string))
        {
            Title = "File"
        }; 
        public SaveFileNode()
        {
            Title = NodeTypeName = "Save File";
            ValueOutput.IsHidden = true;
            Output.Add(_filePathOutput);
        }
        #endregion

        #region Interface
        public override OutputConnector MainOutput => _filePathOutput;
        protected override NodeExecutionResult Execute()
        {
            NodeExecutionResult result = base.Execute();
            result.Caches.Add(_filePathOutput, Path);
            return result;
        }
        #endregion
    }
}