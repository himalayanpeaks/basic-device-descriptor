using DeviceDescriptor.Abstract.Variables;
using OneDriver.Framework.Module.Parameter;

namespace DeviceDescriptor.Abstract
{
    public class BasicDescriptor<TVariable> : BaseChannelParam where TVariable : BasicVariable
    {
        private DeviceVariables<TVariable> variables;
        private ProcessData<TVariable> processData;

        public BasicDescriptor(string name, DeviceVariables<TVariable> variables, ProcessData<TVariable> processData) : base(name)
        {
            this.variables = variables;
            this.processData = processData;
        }

        public DeviceVariables<TVariable> Variables { get => variables; set => variables = value; }
        public ProcessData<TVariable> ProcessData { get => processData; set => processData = value; }
    }
}
