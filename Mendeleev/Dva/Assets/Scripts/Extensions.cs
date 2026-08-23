using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml.Linq;

namespace Dva
{
    public static class AIUtility
    {
        //read-only reference data bundled with the game; loaded via Resources so it works in builds
        private static readonly string c_ConfigResource = "Config";
        private static readonly string c_ElementsConfigResource = "ElementsConfig";
        //the player's save file: seeded from the bundled template on first run, then read/written
        //under Application.persistentDataPath, the only location writable on-device
        private static readonly string p_SaveResource = "PlayerConfig";
        private static readonly string p_SaveFileName = "PlayerConfig.xml";

        private static Dictionary<int, string> _NameIDDict = new Dictionary<int, string>();
        private static Dictionary<int, string> _SymbolIDDict = new Dictionary<int, string>();
        private static Dictionary<int, string> _MaterialIDDict = new Dictionary<int, string>();
        private static Dictionary<int, string> _CompositionIDDict = new Dictionary<int, string>();

        private static Dictionary<int, string> e_NameIDDict = new Dictionary<int, string>();
        private static Dictionary<int, string> e_SymbolIDDict = new Dictionary<int, string>();
        private static Dictionary<int, int> e_MaterialNumberDict = new Dictionary<int, int>();
        private static Dictionary<int, string> e_CompositionIDDict = new Dictionary<int, string>();
        private static Dictionary<int, int> e_IsotopesNumberDict = new Dictionary<int, int>();
        private static Dictionary<int, int> e_NumberIDDict = new Dictionary<int, int>();

        private static Dictionary<int, int> p_Numbers = new Dictionary<int, int>();
        private static XDocument _fileToWrite;

        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Configuration()
        {
            ConfigurationAtomData(LoadBundledXml(c_ConfigResource).Root);
            ConfigurationElementsData(LoadBundledXml(c_ElementsConfigResource).Root);

            _fileToWrite = LoadOrCreateSaveFile();
            ConfigurationPlayerNumbers(_fileToWrite.Root);
        }

        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, p_SaveFileName);

        //load a read-only XML file bundled with the game; Resources works in both the Editor and builds,
        //unlike a raw Application.dataPath file path which only happens to exist in the Editor
        private static XDocument LoadBundledXml(string resourceName)
        {
            return XDocument.Parse(Resources.Load<TextAsset>(resourceName).text);
        }

        //load the player's save file from persistent storage, seeding it from the bundled empty
        //template on first run - Application.dataPath isn't writable on-device (e.g. iOS)
        private static XDocument LoadOrCreateSaveFile()
        {
            string path = SaveFilePath;
            if (!File.Exists(path))
            {
                LoadBundledXml(p_SaveResource).Save(path);
            }
            return XDocument.Load(path);
        }

        //data from config file with all existing atoms
        private static void ConfigurationAtomData(XElement root)
        {
            //going through each elemtnts
            foreach (var element in root.Element("Atom").Elements("AtomStructure"))
            {

                //getting values
                var atomIDInt = int.Parse(element.Attribute("ID").Value);
                var atomNameStr = element.Attribute("Name").Value;
                var atomSymbolStr = element.Attribute("Symbol").Value;
                var atomMaterialStr = element.Attribute("Material").Value;
                var atomCompositionStr = element.Attribute("Composition").Value;

                _NameIDDict.Add(atomIDInt, atomNameStr);
                _SymbolIDDict.Add(atomIDInt, atomSymbolStr);
                _MaterialIDDict.Add(atomIDInt, atomMaterialStr);
                _CompositionIDDict.Add(atomIDInt, atomCompositionStr);
            }
        }

        //data from elementsconfig file with unique atoms
        private static void ConfigurationElementsData(XElement root)
        {
            //Проходка по группам действий
            foreach (var element in root.Element("Atom").Elements("Element"))
            {

                //Получение значения перечисления для игрока
                var atomIDInt = int.Parse(element.Attribute("ID").Value);
                var atomNameStr = element.Attribute("Name").Value;
                var atomSymbolStr = element.Attribute("Symbol").Value;
                var atomMaterialInt = int.Parse(element.Attribute("Material").Value);
                var atomCompositionStr = element.Attribute("Composition").Value;
                var atomNumberInt = int.Parse(element.Attribute("Number").Value);
                var atomIsotopesInt = int.Parse(element.Attribute("Isotopes").Value);

                e_NameIDDict.Add(atomIDInt, atomNameStr);
                e_SymbolIDDict.Add(atomIDInt, atomSymbolStr);
                e_MaterialNumberDict.Add(atomNumberInt, atomMaterialInt);
                e_CompositionIDDict.Add(atomIDInt, atomCompositionStr);
                e_NumberIDDict.Add(atomIDInt, atomNumberInt);
                e_IsotopesNumberDict.Add(atomNumberInt, atomIsotopesInt);
            }
        }

        //data from playerconfig file with atoms discovered by player
        private static void ConfigurationPlayerNumbers(XElement root)
        {
            //Проходка по группам действий
            foreach (var element in root.Element("Atom").Elements("PlayerElements"))
            {

                //Получение значения перечисления для игрока
                var playerAtomID = int.Parse(element.Attribute("ID").Value);
                var playerNumbers = int.Parse(element.Attribute("Number").Value);

                p_Numbers.Add(playerAtomID, playerNumbers);
            }
        }

        public static void RewriteXML(int number, int ID)
        {
            
            _fileToWrite.Element("Units").Element("Atom").Add(new XElement("PlayerElements", new XAttribute("ID", ID),
                new XAttribute("Number", number)));
            _fileToWrite.Save(SaveFilePath);
        }

        /// getting back dictionary with data of elements
        public static IReadOnlyDictionary<int, string> GetAtomName => _NameIDDict;
        public static IReadOnlyDictionary<int, string> GetAtomSymbol => _SymbolIDDict;
        public static IReadOnlyDictionary<int, string> GetAtomMaterial => _MaterialIDDict;
        public static IReadOnlyDictionary<int, string> GetAtomComposition => _CompositionIDDict;

        public static IReadOnlyDictionary<int, string> GetElementName => e_NameIDDict;
        public static IReadOnlyDictionary<int, string> GetElementSymbol => e_SymbolIDDict;
        public static IReadOnlyDictionary<int, int> GetElementMaterial => e_MaterialNumberDict;
        public static IReadOnlyDictionary<int, string> GetElementComposition => e_CompositionIDDict;
        public static IReadOnlyDictionary<int, int> GetElementNumber => e_NumberIDDict;
        public static IReadOnlyDictionary<int, int> GetElementIsotopes => e_IsotopesNumberDict;

        public static Dictionary<int, int> GetPlayerNumbers => p_Numbers;
    }
}
