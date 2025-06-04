using hlarm.xml;
using System.Xml.Serialization;
using System.Xml;
using xml;
using hlarm.pseudocode.parser;

namespace hlarm
{
    public static class Program
    {
        static void Main()
        {
            hlarm_context working_hlarm_context = new hlarm_context();

            string[] all_files = Directory.GetFiles("C:\\Users\\Raymond\\Desktop\\arm\\ISA_A64_xml_A_profile-2025-03", "*.xml", SearchOption.AllDirectories);

            foreach (string file in all_files)
            {
                if (file == "C:\\Users\\Raymond\\Desktop\\arm\\ISA_A64_xml_A_profile-2025-03\\dsb.xml")
                {
                    Console.WriteLine("Adasdasd");
                }

                ArmXmlFile arm_xml_file = xml_loader.read_arm_xml_from_path(file);

                working_hlarm_context.append_data_to_context(arm_xml_file);
            }
        }
    }
}