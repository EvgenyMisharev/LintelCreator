using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LintelCreator
{
    public class LintelCreatorSettings
    {
        public string SelectedLintelFamilieName { get; set; }
        public string SelectedFamilySymbolName { get; set; }
        public string SelectedOpeningHeightParameterName { get; set; }
        public string SelectedOpeningWidthParameterName { get; set; }

        public static LintelCreatorSettings GetSettings()
        {
            LintelCreatorSettings lintelCreatorSettings = null;
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "LintelCreatorSettings.xml";
            string assemblyPath = assemblyPathAll.Replace("LintelCreator.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                using (FileStream fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(LintelCreatorSettings));
                    lintelCreatorSettings = xSer.Deserialize(fs) as LintelCreatorSettings;
                    fs.Close();
                }
            }
            else
            {
                lintelCreatorSettings = new LintelCreatorSettings();
            }

            return lintelCreatorSettings;
        }

        public void Save()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "LintelCreatorSettings.xml";
            string assemblyPath = assemblyPathAll.Replace("LintelCreator.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                File.Delete(assemblyPath);
            }

            using (FileStream fs = new FileStream(assemblyPath, FileMode.Create))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(LintelCreatorSettings));
                xSer.Serialize(fs, this);
                fs.Close();
            }
        }
    }
}
