using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hlarm.xml
{
    public static class xml_combiner
    {
        public static string combine_xmls_from_path(string path)
        {
            string[] files = Directory.GetFiles(path, "*.xml", SearchOption.AllDirectories);

            StringBuilder result = new StringBuilder("<?xml-stylesheet type=\"text/xsl\" encoding=\"UTF-8\" href=\"iform.xsl\" version=\"1.0\"?>\n");

            result.AppendLine("<arm_xml_file>");

            foreach (string file in files)
            {
                if (!file.EndsWith(".xml"))
                    continue;

                string file_name = Path.GetFileName(file);

                string xml_source = File.ReadAllText(file);

                xml_source = xml_loader.fix_arm_xml_file(xml_source, false);   

                result.Append($"<!-- {file_name} -->\n");
                result.Append($"{xml_source}\n");
            }

            result.AppendLine("</arm_xml_file>");

            return result.ToString();
        }
    }
}
