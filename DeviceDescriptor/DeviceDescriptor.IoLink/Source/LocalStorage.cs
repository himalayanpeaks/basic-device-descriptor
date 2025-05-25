using DeviceDescriptor.Abstract;
using DeviceDescriptor.IoLink.Variables;
using DeviceDescriptor.IoLink.IODD1_1; // Namespace from your IODD1_1.cs
using System.Xml.Serialization;
using DeviceDescriptor.Abstract.Variables;
using static DeviceDescriptor.Abstract.Definition;
using System.Globalization;


namespace DeviceDescriptor.IoLink.Source
{
    public class LocalStorage : IDescriptorTranslator<Variable>
    {
        public Task<BasicDescriptor<Variable>?> LoadFromWebAsync(string address, string deviceId = "0", string productName = "")
        {
            if (!File.Exists(address))
                return Task.FromResult<BasicDescriptor<Variable>?>(null);

            IODevice? root;
            try
            {
                var serializer = new XmlSerializer(typeof(IODevice));
                using var stream = File.OpenRead(address);
                root = serializer.Deserialize(stream) as IODevice;
            }
            catch
            {
                return Task.FromResult<BasicDescriptor<Variable>?>(null);
            }

            if (root == null || root.ProfileBody?.DeviceFunction == null)
                return Task.FromResult<BasicDescriptor<Variable>?>(null);

            //Get all avaliable variables from the DeviceFunction
            var variableDefs = root.ProfileBody.DeviceFunction.VariableCollection?
                .Variable
                .OfType<VariableT>()
                .ToList();
            //Get all datatype definitions from the DeviceFunction
            var datatypeMap = root.ProfileBody.DeviceFunction.DatatypeCollection?
                .Datatype
                .OfType<DatatypeT>()
                .ToDictionary(d => d.id, d => d);

            var variables = new List<Variable>();
            int varCount = 0, dataRefCount = 0;
            DataType dataType = DataType.CHAR;
            int lengthinbits = 0; 
            string? minValue = null, maxValue = null, valid = null, defaultValue = null;
            int offset = 0;
            foreach (var varDef in variableDefs ?? Enumerable.Empty<VariableT>())
            {
                #region CommonAttributes
                #region required
                string name = varDef.id;
                int index = varDef.index != 0 ? varDef.index : -1;
                AccessType access = varDef.accessRights switch
                {
                    AccessRightsT.ro => AccessType.R,
                    AccessRightsT.wo => AccessType.W,
                    AccessRightsT.rw => AccessType.RW,
                    _ => AccessType.RW
                };
                #endregion
                #region optional
                bool isDynamic = varDef.dynamic;
                #endregion
                #endregion CommonAttributes


                if (varDef.Item is DatatypeT dtDef)
                {
                    //Assin type
                    dataType = dtDef?.GetType().Name switch
                    {
                        nameof(UIntegerT) => DataType.UINT,
                        nameof(IntegerT) => DataType.INT,
                        nameof(BooleanT) => DataType.BOOL,
                        nameof(Float32T) => DataType.Float32,
                        nameof(StringT) => DataType.CHAR,
                        nameof(RecordT) => DataType.Record,
                        _ => DataType.Byte
                    };
                    varCount++;
                    if (varDef is VariableCollectionTVariable v && !string.IsNullOrWhiteSpace(v.defaultValue))                    
                        defaultValue = v.defaultValue;                    
                    
                    switch (varDef.Item)
                    {                        
                        case UIntegerT u:                            
                            (minValue, maxValue) = GetMinMaxFromValueRange(u.Items);
                            valid = GetValidValues(u.Items);
                            lengthinbits = u.bitLength;
                            break;
                        case IntegerT i:
                            (minValue, maxValue) = GetMinMaxFromValueRange(i.Items);
                            valid = GetValidValues(i.Items);
                            lengthinbits = i.bitLength;
                            break;
                        case Float32T f:
                            (minValue, maxValue) = GetMinMaxFromValueRange(f.Items);
                            valid = GetValidValues(f.Items);
                            lengthinbits = 32;
                            break;
                        case StringT s:
                            lengthinbits = 8 * s.fixedLength;
                            break;
                        case BooleanT b:
                            valid = GetValidValues(b.SingleValue);                            
                            lengthinbits = 8;
                            break;
                        case RecordT r:
                            lengthinbits = r.bitLength;
                            break;
                        default:
                            // array type not handles
                            continue; // Skip this variable definition
                    }
                }
                    if(varDef.Item is DatatypeRefT refT)
                    {
                        var dtypeId = refT.datatypeId;
                        dataRefCount++;
                    }


                    var variable = new Variable(
                    name: name,
                    index: index,
                    isDynamic: isDynamic, 
                    subindex: 0, 
                    access: access,
                    dataType: DataType.UINT,
                    arrayCount: 1, //Can't handle array. Rerely used
                    lengthInBits: lengthinbits, 
                    offset: offset, 
                    defaultValue: defaultValue, 
                    minimum: minValue, 
                    maximum: maxValue, 
                    valid: valid, 
                    value: null 
                );

                variables.Add(variable);
            }

            var deviceVariables = new DeviceVariables<Variable>
            {
                SpecificVariableCollection = variables, //in progress..
                StandardVariableCollection = new List<Variable>(), 
                SystemVariableCollection = new List<Variable>(),  
                CommandCollection = new List<Variable>()          
            };

            var descriptor = new BasicDescriptor<Variable>(
                variables: deviceVariables,
                processData: null // Assuming null for ProcessData
            );

            return Task.FromResult<BasicDescriptor<Variable>?>(descriptor);
        }

        public static (string? Min, string? Max) GetMinMaxFromValueRange(AbstractValueT[]? valueRange)
        {
            if (valueRange == null)
                return (null, null);

            return ((AbstractValueT[])valueRange)[0] switch
            {
                UIntegerValueRangeT u => (u.lowerValue.ToString(), u.upperValue.ToString()),
                IntegerValueRangeT i => (i.lowerValue.ToString(), i.upperValue.ToString()),
                Float32ValueRangeT f => (f.lowerValue.ToString(), f.upperValue.ToString()),

                // Add other range types here if needed
                _ => (null, null)
            };
        }

        public static string? GetValidValues(AbstractValueT[]? valueRange)
        {
            if (valueRange == null)
                return null;

            var values = valueRange.Select(v => v switch
            {
                UIntegerValueT u => u.value.ToString(),
                IntegerValueT i => i.value.ToString(),
                Float32ValueT f => f.value.ToString(CultureInfo.InvariantCulture),
                BooleanValueT b => b.value.ToString(),
                _ => null
            }).Where(s => s != null);

            return string.Join(",", values!);
        }    
    }
}
