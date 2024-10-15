using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RaindowStudio.IniParser
{
    public class IniDatas
    {
        public Dictionary<string, IniSection> data;

        public int Count => data.Count;
        public bool ContainsSection(string sectionName) => data.ContainsKey(sectionName);
        public List<IniSection> Sections => data.Values.ToList();

        public IniSection this[string _key]
        {
            get { return data[_key]; }
            set { data[_key] = value; }
        }

        public void Add(IniSection _section)
        {
            data.Add(_section.name, _section);
        }

        public bool RemoveSection(string _sectionName)
        {
            return data.Remove(_sectionName);
        }

        /// <summary>
        /// Compare two iniDatas has same section and key(not contains value).
        /// </summary>
        public bool Compare(IniDatas _iniDatas)
        {
            var compareData = _iniDatas.data;
            if (data.Count != compareData.Count)
            {
                return false;
            }
            foreach (var section in data.Keys.ToList())
            {
                if (compareData.ContainsKey(section))
                {
                    foreach (var key in data[section].data.Keys.ToList())
                    {
                        if (!compareData[section].data.ContainsKey(key))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public IniDatas()
        {
            data = new Dictionary<string, IniSection>();
        }

        public IniDatas(string _filePath)
        {
            data = new Dictionary<string, IniSection>();
            string[] datas = File.ReadAllLines(_filePath);
            IniSection section = new IniSection();
            List<string> comments = new List<string>();
            for (int i = 0; i < datas.Length; ++i)
            {
                datas[i] = datas[i].Trim();
                if (datas[i] == string.Empty)
                {
                    continue;
                }
                else if (datas[i][0] == '[')
                {
                    if (section.name != string.Empty)
                    {
                        Add(section);
                    }
                    section = new IniSection(datas[i].Replace("[", "").Replace("]", ""), comments);
                    comments = new List<string>();
                }
                else if (datas[i][0] == ';')
                {
                    comments.Add(datas[i].Replace(";", ""));
                }
                else
                {
                    try
                    {
                        section.Add(new IniNode(datas[i].Split('=')[0].Trim(), datas[i].Split('=')[1].Trim(), comments));
                        comments = new List<string>();
                    }
                    catch { }
                }
            }
            Add(section);
        }

        public string Data2String
        {
            get
            {
                string dataString = string.Empty;
                foreach (var key in data.Keys.ToList())
                {
                    foreach (var comment in data[key].comments)
                    {
                        dataString += ';' + comment + '\n';
                    }
                    dataString += '[' + data[key].name + ']' + '\n';
                    foreach (var key_ in data[key].data.Keys.ToList())
                    {
                        foreach (var comment_ in data[key].data[key_].comments)
                        {
                            dataString += ';' + comment_ + '\n';
                        }
                        dataString += data[key].data[key_].key + '=' + data[key].data[key_].value + '\n';
                    }
                    dataString += '\n';
                }
                return dataString;
            }
        }

        public IniDatas Write(string _filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(_filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
            }
            File.WriteAllText(_filePath, Data2String);
            return this;
        }

        public override string ToString()
        {
            string @return = string.Empty;
            foreach (IniSection section in data.Values)
            {
                @return += $";{section}{Environment.NewLine}";
            }
            return @return;
        }
    }

    public class IniSection
    {
        public string name = string.Empty;
        public List<string> comments;
        public Dictionary<string, IniNode> data;

        public int Count => data.Count;
        public bool ContainsNode(string nodeKey) => data.ContainsKey(nodeKey);
        public List<IniNode> Nodes => data.Values.ToList();

        public IniNode this[string _key]
        {
            get { return data[_key]; }
            set { data[_key] = value; }
        }

        public void Add(IniNode _iniData)
        {
            data.Add(_iniData.key, _iniData);
        }

        public bool RemoveNode(string _nodeName)
        {
            return data.Remove(_nodeName);
        }

        public IniSection()
        {
            comments = new List<string>();
            data = new Dictionary<string, IniNode>();
        }

        public IniSection(string _sectionName, string _comment)
        {
            name = _sectionName;
            (comments = new List<string>()).Add(_comment);
            data = new Dictionary<string, IniNode>();
        }

        public IniSection(string _sectionName, List<string> _comments = default)
        {
            name = _sectionName;
            comments = _comments ?? new List<string>();
            data = new Dictionary<string, IniNode>();
        }

        public IniNode Data(string _key)
        {
            return data[_key];
        }

        public override string ToString()
        {
            string @return = string.Empty;
            foreach (var comment in comments)
            {
                @return += $";{comment}{Environment.NewLine}";
            }
            @return += $"[{name}]{Environment.NewLine}";
            foreach (IniNode node in data.Values)
            {
                @return += $"{node}{Environment.NewLine}";
            }
            return @return;
        }
    }

    public class IniNode
    {
        public string key = string.Empty;
        public string value = string.Empty;
        public List<string> comments;

        public int AsInt { get { return int.Parse(value); } }
        public float AsFloat { get { return float.Parse(value); } }
        public bool AsBool { get { return bool.Parse(value); } }

        public IniNode()
        {
            comments = new List<string>();
        }

        public IniNode(string _key, string _value, string _comment)
        {
            key = _key;
            value = _value;
            (comments = new List<string>()).Add(_comment);
        }

        public IniNode(string _key, string _value, List<string> _comments = default)
        {
            key = _key;
            value = _value;
            comments = _comments ?? new List<string>();
        }

        public override string ToString() 
        {
            string @return = string.Empty;
            foreach (var comment in comments)
            {
                @return += $";{comment}{Environment.NewLine}";
            }
            @return += key + '=' + value;
            return @return;
        }
    }
}
