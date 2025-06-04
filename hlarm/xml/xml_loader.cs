using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using xml;

namespace hlarm.xml
{
    public static class xml_loader
    {
        public static string fix_arm_xml_file(string xml_source, bool insert_xml_file = true)
        {
            xml_source = string_tools.fix_string(xml_source);

            string[] xml_source_parts = xml_source.Split('\n');

            for (int i = 0; i < 4; ++i)
            {
                string to_replace = xml_source_parts[i];

                if (to_replace == "")
                    continue;

                xml_source = xml_source.Replace(to_replace + "\n", "");
            }

            string result = "";

            if (insert_xml_file)
            {
                result += "<arm_xml_file>\n";
            }

            result += xml_source;

            if (insert_xml_file)
            {
                result += "</arm_xml_file>\n";
            }

            result = fix_xml_pseudocode(result);

            return result;
        }

        static string fix_xml_pseudocode(string source)
        {
            XmlDocument document = new XmlDocument();

            document.PreserveWhitespace = true;
            document.LoadXml(source);

            foreach (XmlNode element in document.ChildNodes)
            {
                fix_xml_psudocode(element);
            }

            return document.OuterXml;
        }

        static string clean_text(string source)
        {
            StringBuilder working_result = new StringBuilder();

            int tower = 0;

            for (int i = 0; i < source.Length; ++i)
            {
                char working_char = source[i];

                if (working_char == '<')
                {
                    tower++;
                }
                else if (working_char == '>')
                {
                    tower--;
                } 
                else if (tower == 0)
                {
                    working_result.Append(working_char);
                }
            }

            working_result.Replace("&quot;" , "\"");
            working_result.Replace("&apos;" , "\'");
            working_result.Replace("&lt;"   , "<");
            working_result.Replace("&gt;"   , ">");
            working_result.Replace("&amp;"  , "&");

            return working_result.ToString();   
        }


        static void fix_xml_psudocode(XmlNode working_element)
        {
            if (working_element.Name == "pstext")
            {
                string working_text = working_element.InnerText;

                working_element.RemoveAll();
                working_element.InnerText = working_text;   
            }
            else
            {
                foreach (XmlNode element in working_element.ChildNodes)
                {
                    fix_xml_psudocode(element);
                }
            }
            
        }

        public static string read_arm_xml_string_from_path(string path)
        {
            string raw_file = File.ReadAllText(path);

            raw_file = fix_arm_xml_file(raw_file, true);

            return raw_file;
        }

        public static ArmXmlFile read_arm_xml_from_path(string path)
        {
            string xml_file = read_arm_xml_string_from_path(path);

            XmlSerializer serializer = new XmlSerializer(typeof(ArmXmlFile));
            StringReader reader = new StringReader(xml_file);

            ArmXmlFile csfile = (ArmXmlFile)serializer.Deserialize(reader);

            return csfile;
        }
    }
}
