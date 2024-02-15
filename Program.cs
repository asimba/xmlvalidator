using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace xmlvalidator
{
    class Program
    {
        static bool Warning = true;
        static void Main(string[] args)
        {
            string xsd_path = args.Length > 1 ? Path.GetFullPath(args[0]) : "";
            string xml_path = args.Length > 1 ? Path.GetFullPath(args[1]) : "";
            if (args.Length > 1 && File.Exists(xsd_path) && File.Exists(xml_path))
            {
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                try
                {
                    schemaSet.Add(null, xsd_path);
                }
                catch (System.Xml.XmlException e)
                {
                    Console.WriteLine("XSD schema error!");
                    ValidationLog(e.LineNumber, e.Message);
                    return;
                }
                XmlSchema compiledSchema = null;
                foreach (XmlSchema schema in schemaSet.Schemas())
                {
                    compiledSchema = schema;
                }
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(compiledSchema);
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessIdentityConstraints;
                settings.ValidationEventHandler += new ValidationEventHandler(
                    (object sender, ValidationEventArgs va)=>ValidationLog(va.Exception.LineNumber, va.Message)
                );
                try
                {
                    XmlReader reader = XmlReader.Create(xml_path, settings);
                    while (reader.Read()) ;
                    reader.Close();
                }
                catch (System.Xml.XmlException e)
                {
                    ValidationLog(e.LineNumber, e.Message);
                }
                if(Warning) Console.WriteLine("Ok.");
            }
            else
            {
                Console.WriteLine("XSD/XML file validator.\nUsage: {0} <XSD file path> <XML file path>", System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            }
        }
        public static void ValidationLog(int LineNumber,string Message)
        {
            Warning = false;
            Console.WriteLine("{0}: {1}", LineNumber, Message);
        }
    }
}
