using DeviceDescriptor.Abstract.Variables;

namespace DeviceDescriptor.Abstract
{
    public class BasicDescriptor<TVariable> where TVariable : BasicVariable
    {
        private DeviceVariables<TVariable> variables;
        private ProcessData<TVariable> processData;

        public BasicDescriptor(DeviceVariables<TVariable> variables, ProcessData<TVariable> processData)
        {
            this.variables = variables;
            this.processData = processData;
        }

        public DeviceVariables<TVariable> Variables { get => variables; set => variables = value; }
        public ProcessData<TVariable> ProcessData { get => processData; set => processData = value; }
    }
}
