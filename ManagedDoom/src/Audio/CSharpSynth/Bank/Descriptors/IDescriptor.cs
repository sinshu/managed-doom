using System.IO;

namespace AudioSynthesis.Bank.Descriptors
{
    public interface IDescriptor
    {
        void Read(string[] description);
        int Read(BinaryReader reader);
        int Write(BinaryWriter writer);
    }
}
