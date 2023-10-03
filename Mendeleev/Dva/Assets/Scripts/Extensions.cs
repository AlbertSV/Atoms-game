using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dva
{
    public static class AIUtility
    {
        private static readonly string c_ConfigPath = "//Resources//Config.xml";
        private static readonly string c_ElementsConfigPath = "//Resources//ElementsConfig.xml";
        private static readonly string p_NumbersList = "//Resources//PlayerConfig.xml";

        private static Dictionary<int, string> _NameIDDict = new Dictionary<int, string>();
        private static Dictionary<int, string> _SymbolIDDict = new Dictionary<int, string>();
        private static Dictionary<int, string> _MaterialIDDict = new Dictionary<int, string>();
        private static Dictionary<int, string> _CompositionIDDict = new Dictionary<int, string>();

        private static Dictionary<int, string> e_NameIDDict = new Dictionary<int, string>();
        private static Dictionary<int, string> e_SymbolIDDict = new Dictionary<int, string>();
        private static Dictionary<int, int> e_MaterialIDDict = new Dictionary<int, int>();
        private static Dictionary<int, string> e_CompositionIDDict = new Dictionary<int, string>();
        private static Dictionary<int, int> e_IsotopesNumberDict = new Dictionary<int, int>();
        private static Dictionary<int, int> e_NumberIDDict = new Dictionary<int, int>();

        private static Dictionary<int, int> p_Numbers = new Dictionary<int, int>();
        private static XDocument _fileToWrite;

        //Запускается автоматически
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Configuration()
        {
            _fileToWrite = XDocument.Load(Application.dataPath + p_NumbersList);
            var root = XDocument.Load(Application.dataPath + c_ConfigPath).Root;
            ConfigurationAtomData(root);
            root = XDocument.Load(Application.dataPath + c_ElementsConfigPath).Root;
            ConfigurationElementsData(root);
            root = XDocument.Load(Application.dataPath + p_NumbersList).Root;
            ConfigurationPlayerNumbers(root);

        }

        private static void ConfigurationAtomData(XElement root)
        {
            //Проходка по группам действий
            foreach (var element in root.Element("Atom").Elements("AtomStructure"))
            {

                //Получение значения перечисления для игрока
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
                e_MaterialIDDict.Add(atomIDInt, atomMaterialInt);
                e_CompositionIDDict.Add(atomIDInt, atomCompositionStr);
                e_NumberIDDict.Add(atomIDInt, atomNumberInt);
                e_IsotopesNumberDict.Add(atomNumberInt, atomIsotopesInt);
            }
        }

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
            _fileToWrite.Save(Application.dataPath + p_NumbersList);
        }

        /// <summary>
        /// Возвращает коллекцию для чтения с коэффициентами изменения приоритетов действий бота по действиям игрока
        /// </summary>
        public static IReadOnlyDictionary<int, string> GetAtomName => _NameIDDict;
        public static IReadOnlyDictionary<int, string> GetAtomSymbol => _SymbolIDDict;
        public static IReadOnlyDictionary<int, string> GetAtomMaterial => _MaterialIDDict;
        public static IReadOnlyDictionary<int, string> GetAtomComposition => _CompositionIDDict;

        public static IReadOnlyDictionary<int, string> GetElementName => e_NameIDDict;
        public static IReadOnlyDictionary<int, string> GetElementSymbol => e_SymbolIDDict;
        public static IReadOnlyDictionary<int, int> GetElementMaterial => e_MaterialIDDict;
        public static IReadOnlyDictionary<int, string> GetElementComposition => e_CompositionIDDict;
        public static IReadOnlyDictionary<int, int> GetElementNumber => e_NumberIDDict;
        public static IReadOnlyDictionary<int, int> GetElementIsotopes => e_IsotopesNumberDict;

        public static Dictionary<int, int> GetPlayerNumbers => p_Numbers;
    }
}
