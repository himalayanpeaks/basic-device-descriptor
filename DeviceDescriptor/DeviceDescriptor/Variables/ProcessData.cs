using OneDriver.Framework.Base;

namespace DeviceDescriptor.Abstract.Variables
{
    public class ProcessData<TVariable> : PropertyHandlers where TVariable : BasicVariable
    {
        private List<TVariable> processDataInCollection;
        private List<TVariable> processDataOutCollection;

        public ProcessData()
        {
            processDataInCollection = new List<TVariable>();
            processDataOutCollection = new List<TVariable>();

        }

        public List<TVariable> ProcessDataInCollection
        {
            get => processDataInCollection;
            set => SetProperty(ref processDataInCollection, value);
        }
        public List<TVariable> ProcessDataOutCollection
        {
            get => processDataOutCollection;
            set => SetProperty(ref processDataOutCollection, value);
        }
    }
}
