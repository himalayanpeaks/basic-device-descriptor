using DeviceDescriptor.Abstract.Variables;
using OneDriver.Framework.Module.Parameter;

namespace DeviceDescriptor.Abstract
{
    public class BasicDescriptor<TVariable> : BaseChannelParam where TVariable : BasicVariable
    {
        private DeviceVariables<TVariable> variables;
        

        public BasicDescriptor(string name, DeviceVariables<TVariable> variables) : base(name)
        {
            this.variables = variables;
            
        }

        public DeviceVariables<TVariable> Variables { get => variables; set => variables = value; }        
    }
}
