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
            int lengthinbits = 0; string? minValue = null, maxValue = null, valid = null;
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
                    switch (varDef.Item)
                    {
                        case UIntegerT u:
                            (minValue, maxValue) = GetMinMaxFromValueRange(u.Items);
                            break;
                        case IntegerT i:
                            (minValue, maxValue) = GetMinMaxFromValueRange(i.Items);
                            break;
                        case Float32T f:
                            (minValue, maxValue) = GetMinMaxFromValueRange(f.Items);
                            break;
                        case StringT s:
                            // Handle StringT specific logic
                            break;
                        case BooleanT b:
                            // Handle BooleanT specific logic
                            break;
                        case RecordT r:
                            // Handle RecordT specific logic
                            break;
                        default:
                            // Handle unknown or unsupported types
                            continue; // Skip this variable definition
                    }
                }
                    if(varDef.Item is DatatypeRefT refT)
                    {
                        // If it's a reference to a datatype, we can extract the ID
                        var dtypeId = refT.datatypeId;
                        dataRefCount++;
                    }
                    //switch(varDef.Item)
                    /*var dtypeId = ((DatatypeRefT)varDef.Item).datatypeId; // Assuming 'id' contains the datatype ID.
                     var dtDef = (dtypeId != null && datatypeMap != null && datatypeMap.ContainsKey(dtypeId))
                         ? datatypeMap[dtypeId]
                         : null;

                     DataType dataType = dtDef?.GetType().Name switch
                     {
                         nameof(UIntegerT) => DataType.UINT,
                         nameof(IntegerT) => DataType.INT,
                         nameof(BooleanT) => DataType.BOOL,
                         nameof(Float32T) => DataType.Float32,
                         nameof(StringT) => DataType.CHAR,
                         nameof(RecordT) => DataType.Record,
                         _ => DataType.Byte
                     };*/

                    // Updated constructor call to include all required parameters
                    var variable = new Variable(
                    name: name,
                    index: index,
                    isDynamic: isDynamic, // Assuming default value for isDynamic
                    subindex: 0, // Assuming default value for subindex
                    access: access,
                    dataType: DataType.UINT,
                    arrayCount: 0, // Assuming default value for arrayCount
                    lengthInBits: lengthinbits, // Assuming default value for lengthInBits
                    offset: 0, // Assuming default value for offset
                    defaultValue: null, // Assuming default value for default
                    minimum: null, // Assuming default value for minimum
                    maximum: null, // Assuming default value for maximum
                    valid: null, // Assuming default value for valid
                    value: null // Added missing 'value' parameter
                );

                variables.Add(variable);
            }

            var deviceVariables = new DeviceVariables<Variable>
            {
                SpecificVariableCollection = variables,
                StandardVariableCollection = new List<Variable>(), // Assuming empty collections
                SystemVariableCollection = new List<Variable>(),   // Assuming empty collections
                CommandCollection = new List<Variable>()          // Assuming empty collections
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

        /*private void GetValues(DataType type, int lengthInBits, out string? min, out string? max, out string[]? valid)
        {
            DatatypeT? dtDef = null;
            string? dtypeId = null;
            string? minValue = null;
            string? maxValue = null;
            string? valid = null;
            int lengthInBits = 0;
            DataType dataType = DataType.Byte;

            // 1. Resolve datatype
            if (varDef.Item is DatatypeRefT refT)
            {
                dtypeId = refT.datatypeId;
                if (dtypeId != null && datatypeMap != null && datatypeMap.TryGetValue(dtypeId, out var resolved))
                    dtDef = resolved;
            }
            else if (varDef.Item is DatatypeT inlineDt)
            {
                dtDef = inlineDt;
            }

            // 2. Extract min/max/valid from known numeric types
            switch (dtDef)
            {
                case UIntegerT u:
                    lengthInBits = u.bitLength;
                    dataType = DataType.UINT;
                    if (u.Items?.OfType<UIntegerValueRangeT>().FirstOrDefault() is UIntegerValueRangeT uRange)
                    {
                        minValue = uRange.lowerValue.ToString();
                        maxValue = uRange.upperValue.ToString();
                    }
                    else if (u.Items != null)
                    {
                        valid = string.Join(",", u.Items.OfType<UIntegerSingleValueT>().Select(s => s.value.ToString()));
                    }
                    break;

                case IntegerT i:
                    lengthInBits = i.bitLength;
                    dataType = DataType.INT;
                    if (i.Items?.OfType<IntegerValueRangeT>().FirstOrDefault() is IntegerValueRangeT iRange)
                    {
                        minValue = iRange.lowerValue.ToString();
                        maxValue = iRange.upperValue.ToString();
                    }
                    else if (i.Items != null)
                    {
                        valid = string.Join(",", i.Items.OfType<IntegerSingleValueT>().Select(s => s.value.ToString()));
                    }
                    break;

                case Float32T f:
                    lengthInBits = f.bitLength;
                    dataType = DataType.Float32;
                    if (f.Items?.OfType<Float32ValueRangeT>().FirstOrDefault() is Float32ValueRangeT fRange)
                    {
                        minValue = fRange.lowerValue.ToString("G", CultureInfo.InvariantCulture);
                        maxValue = fRange.upperValue.ToString("G", CultureInfo.InvariantCulture);
                    }
                    break;
            }

        }*/
    }
}
