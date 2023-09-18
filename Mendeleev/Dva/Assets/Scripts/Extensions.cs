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

        private static List<AtomData> _atomData = new List<AtomData>();
        private static Dictionary<int, string> _NameIDDict = new Dictionary<int, string>();
        private static Dictionary<int, string> _SymbolIDDict = new Dictionary<int, string>();
        private static Dictionary<int, string> _MaterialIDDict = new Dictionary<int, string>();
        private static Dictionary<int, string> _CompositionIDDict = new Dictionary<int, string>();

        //Запускается автоматически
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Configuration()
        {
            var root = XDocument.Load(Application.dataPath + c_ConfigPath).Root;
            ConfigurationAtomData(root);

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

                //_atomData.Add(new AtomData { ID = atomIDInt, Name = atomNameStr, Symbol = atomSymbolStr, Material = atomMaterialStr, Composition = atomCompositionStr});
                _NameIDDict.Add(atomIDInt, atomNameStr);
                _SymbolIDDict.Add(atomIDInt, atomSymbolStr);
                _MaterialIDDict.Add(atomIDInt, atomMaterialStr);
                _CompositionIDDict.Add(atomIDInt, atomCompositionStr);
            }
        }

        /// <summary>
        /// Возвращает коллекцию для чтения с коэффициентами изменения приоритетов действий бота по действиям игрока
        /// </summary>
        public static IReadOnlyList<AtomData> GetAtomData => _atomData;
        public static IReadOnlyDictionary<int, string> GetAtomName => _NameIDDict;
        public static IReadOnlyDictionary<int, string> GetAtomSymbol => _SymbolIDDict;
        public static IReadOnlyDictionary<int, string> GetAtomMaterial => _MaterialIDDict;
        public static IReadOnlyDictionary<int, string> GetAtomComposition => _CompositionIDDict;
    }
}
