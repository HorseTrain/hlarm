using hlarm.xml;
using System.Xml.Serialization;
using System.Xml;
using xml;
using hlarm.pseudocode.parser;
using hlarm.pseudocode.pre_language;
using hlarm.pseudocode.language;
using hlarm.generators.cs;

namespace hlarm
{
    public static class Program
    {
        static void Main()
        {
            hlarm_context working_hlarm_context = new hlarm_context();

            string[] all_xml_files = Directory.GetFiles("C:\\Users\\Raymond\\Desktop\\arm_testing\\", "*.xml", SearchOption.AllDirectories);
            string[] all_asl_files = Directory.GetFiles("C:\\Users\\Raymond\\Desktop\\arm_testing\\", "*.asl", SearchOption.AllDirectories);

            foreach (string working_file in all_xml_files)
            {
                ArmXmlFile arm_xml_file = xml_loader.read_arm_xml_from_path(working_file);

                working_hlarm_context.append_data_to_context(arm_xml_file);
            }

            working_hlarm_context.create_asl();

            foreach (string asl in all_asl_files)
            {
                working_hlarm_context.append_asl(File.ReadAllText(asl));
            }

            working_hlarm_context.build_asl();

            interpreter_generator test = new interpreter_generator(working_hlarm_context);

            File.WriteAllText("C:\\Users\\Raymond\\Desktop\\arm_testing\\dump", working_hlarm_context.create_dump());   
            File.WriteAllText("C:\\Users\\Raymond\\source\\repos\\hlarm\\debug_arm_interpreter\\interpreter.cs", test.generate_interpreter());
        }
    }
}