using System;
using System.Collections.Generic;
using Parcel.Neo.Base.Framework.ViewModels;

namespace Parcel.Neo.Base.Framework
{
    public enum PropertyEditorType
    {
        TextBox,
        CheckBox,
        Code
    }
    
    public class PropertyEditor: ObservableObject
    {
        #region View Properties
        private string _name;
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }
        private PropertyEditorType _type;
        public PropertyEditorType Type
        {
            get => _type;
            set => SetField(ref _type, value);
        }

        public object Binding
        {
            get => GetBinding();
            set => SetBinding(value);
        }
        #endregion

        public readonly Func<object> GetBinding;
        public readonly Action<object> SetBinding;

        public PropertyEditor(string name, PropertyEditorType type, Func<object> getBinding, Action<object> setBinding)
        {
            _name = name;
            _type = type;
            GetBinding = getBinding;
            SetBinding = setBinding;
        }
    }
    
    public interface INodeProperty
    {
        public List<PropertyEditor> Editors { get; }
    }
}