using DeviceDescriptor.Abstract;
using DeviceDescriptor.IoLink.Variables;
using DeviceDescriptor.IoLink.IODD1_1; // Namespace from your IODD1_1.cs
using System.Xml.Serialization;
using DeviceDescriptor.Abstract.Variables;
using static DeviceDescriptor.Abstract.Definition;


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

            var variableDefs = root.ProfileBody.DeviceFunction.VariableCollection?
                .Variable
                .OfType<VariableT>()
                .ToList();

            var datatypeMap = root.ProfileBody.DeviceFunction.DatatypeCollection?
                .Datatype
                .OfType<DatatypeT>()
                .ToDictionary(d => d.id, d => d);

            var variables = new List<Variable>();

            foreach (var varDef in variableDefs ?? Enumerable.Empty<VariableT>())
            {
                string name = varDef.id ?? "unnamed";

                int index = varDef.index != 0 ? varDef.index : -1;

                AccessType access = varDef.accessRights switch
                {
                    AccessRightsT.ro => AccessType.R,
                    AccessRightsT.wo => AccessType.W,
                    AccessRightsT.rw => AccessType.RW,
                    _ => AccessType.RW
                };

                var dtypeId = varDef.id; // Assuming 'id' contains the datatype ID.
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
                };

                // Updated constructor call to include all required parameters
                var variable = new Variable(
                    name: name,
                    index: index,
                    subindex: 0, // Assuming default value for subindex
                    access: access,
                    dataType: dataType,
                    arrayCount: 0, // Assuming default value for arrayCount
                    lengthInBits: 0, // Assuming default value for lengthInBits
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
    }
}
