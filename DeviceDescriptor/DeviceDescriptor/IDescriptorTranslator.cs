using DeviceDescriptor.Abstract.Variables;

namespace DeviceDescriptor.Abstract
{
    public interface IDescriptorTranslator<TVariable> where TVariable : BasicVariable
    {
        Task<BasicDescriptor<TVariable>?> LoadFromWebAsync(string address, string deviceId = "0", string productName = "");
    }
}
