using OneDriver.Framework.Module.Parameter;

namespace DeviceDescriptor.Abstract.Variables
{
    public class DeviceVariables<TVariable> : BaseChannelParam where TVariable : BasicVariable
    {
        private List<TVariable> specificVariableCollection;
        private List<TVariable> standardVariableCollection;
        private List<TVariable> systemVariableCollection;
        private List<TVariable> commandCollection;

        public DeviceVariables(string name) : base(name)
        {
            specificVariableCollection = new List<TVariable>();
            standardVariableCollection = new List<TVariable>();
            systemVariableCollection = new List<TVariable>();
            commandCollection = new List<TVariable>();
        }

        public List<TVariable> StandardVariableCollection
        {
            get => standardVariableCollection;
            set => SetProperty(ref standardVariableCollection, value);
        }

        public List<TVariable> SpecificVariableCollection
        {
            get => specificVariableCollection;
            set => SetProperty(ref specificVariableCollection, value);
        }

        public List<TVariable> SystemVariableCollection
        {
            get => systemVariableCollection;
            set => SetProperty(ref systemVariableCollection, value);
        }

        public List<TVariable> CommandCollection
        {
            get => commandCollection;
            set => SetProperty(ref commandCollection, value);
        }
    }
}
