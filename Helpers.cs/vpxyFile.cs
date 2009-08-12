using System.IO;
using Gibbed.Helpers;

namespace MadScience
{
    public class vpxyFile
    {
        public static MemoryStream New(keyName inputKey)
        {
            // builds a new vpxy 
            MemoryStream output = new MemoryStream();
            StreamHelpers.WriteValueU32(output, 3);
            StreamHelpers.WriteValueU32(output, 1);
            StreamHelpers.WriteValueU32(output, 0); // index3
            StreamHelpers.WriteValueU32(output, 0); // index1
            StreamHelpers.WriteValueU32(output, 1); // index2

            StreamHelpers.WriteValueU64(output, inputKey.instanceId);
            StreamHelpers.WriteValueU32(output, inputKey.typeId);
            StreamHelpers.WriteValueU32(output, inputKey.groupId);

            StreamHelpers.WriteValueU32(output, 44);
            StreamHelpers.WriteValueU32(output, 47);

            StreamHelpers.WriteStringASCII(output, "VPXY");
            StreamHelpers.WriteValueU32(output, 4);
            StreamHelpers.WriteValueU32(output, 35);
            StreamHelpers.WriteValueU32(output, 0);

            StreamHelpers.WriteValueU8(output, 0);
            StreamHelpers.WriteValueU8(output, 2);
            StreamHelpers.WriteValueU64(output, 0);
            StreamHelpers.WriteValueU64(output, 0);
            StreamHelpers.WriteValueU64(output, 0);
            StreamHelpers.WriteValueU32(output, 0);
            StreamHelpers.WriteValueU8(output, 0);

            return output;
        }

    }
}
