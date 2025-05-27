using DataType = DeviceDescriptor.Abstract.Definition.DataType;
using AccessType = DeviceDescriptor.Abstract.Definition.AccessType;
using OneDriver.Framework.Base;
using OneDriver.Framework.Module.Parameter;

namespace DeviceDescriptor.Abstract.Variables
{
    public class BasicVariable : BaseChannelParam
    {
        private AccessType _access;
        private int _arrayCount;
        private DataType _dataType;
        private string? _default;
        private int _index;
        private int _lengthInBits;
        private string? _maximum; //array of min, max
        private string? _minimum;        
        private string? _valid;
        private string? _value;
        private int _offset;

        public BasicVariable(string name) : base(name)
        {            
        }

        public BasicVariable(string name, int index, AccessType access, DataType dataType, int arrayCount,
            int lengthInBits, int offset,
            string? value, string? @default, string? minimum, string? maximum, string? valid) : base(name)
        {            
            _index = index;
            _access = access;
            _dataType = dataType;
            _arrayCount = arrayCount;
            _lengthInBits = lengthInBits;
            _value = value;
            _default = @default;
            _maximum = maximum;
            _minimum = minimum;
            _valid = valid;
            _offset = offset;
        }

        public int Offset
        {
            get => _offset;
            set => SetProperty(ref _offset, value);
        }
        public string? Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        public AccessType Access
        {
            get => _access;
            set => SetProperty(ref _access, value);
        }

        public DataType DataType
        {
            get => _dataType;
            set => SetProperty(ref _dataType, value);
        }

        public int ArrayCount
        {
            get => _arrayCount;
            set => SetProperty(ref _arrayCount, value);
        }

        public int LengthInBits
        {
            get => _lengthInBits;
            set => SetProperty(ref _lengthInBits, value);
        }

        public string? Default
        {
            get => _default;
            set => SetProperty(ref _default, value);
        }

        public string? Minimum
        {
            get => _minimum;
            set => SetProperty(ref _minimum, value);
        }

        public string? Maximum
        {
            get => _maximum;
            set => SetProperty(ref _maximum, value);
        }
        public string? Valid
        {
            get => _valid;
            set => SetProperty(ref _valid, value);
        }
    }
}
