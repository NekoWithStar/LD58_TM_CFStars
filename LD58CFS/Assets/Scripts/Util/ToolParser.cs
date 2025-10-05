#region

//文件创建者：Egg
//创建时间：10-05 03:00

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LD58.Util
{
    // 主数据结构类
    [Serializable]
    public class CommandDefinition
    {
        public string CommandName;
        public List<ParameterWrapper> Parameters = new();
        public List<Case> Cases = new();
        public Output Output;

        [NonSerialized] public Dictionary<string, string> Variables = new();
        public List<OutputStage> Stages = new List<OutputStage>();
        
        // 变量映射表（用于序列化显示）
        public List<VariableMapping> VariableMappingsList = new();
        
        // 变量映射字典（运行时使用）
        [NonSerialized] 
        private Dictionary<string, VariableMapping> _variableMappingsDict;
        
        public Dictionary<string, VariableMapping> VariableMappings
        {
            get
            {
                if (_variableMappingsDict == null || _variableMappingsDict.Count == 0)
                {
                    _variableMappingsDict = new Dictionary<string, VariableMapping>();
                    foreach (var mapping in VariableMappingsList)
                    {
                        if (!string.IsNullOrEmpty(mapping.VariableName))
                        {
                            _variableMappingsDict[mapping.VariableName] = mapping;
                        }
                    }
                }
                return _variableMappingsDict;
            }
        }
        
        // 缓存随机生成的值
        [NonSerialized] private Dictionary<string, string> _cachedRandomValues = new();
        
        /// <summary>
        /// 重置缓存的随机值
        /// </summary>
        public void ResetRandomCache()
        {
            _cachedRandomValues = new Dictionary<string, string>();
        }
        
        /// <summary>
        /// 解析文本中的变量，替换为实际值
        /// </summary>
        public string ParseVariables(string text, string hitCase)
        {
            if (string.IsNullOrEmpty(text)) return text;
            
            var result = text;
            

            if (!_cachedRandomValues.ContainsKey("time"))
                _cachedRandomValues["time"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC";
            if (!_cachedRandomValues.ContainsKey("random6"))
                _cachedRandomValues["random6"] = GenerateRandomString(6, true);
            if (!_cachedRandomValues.ContainsKey("random10"))
                _cachedRandomValues["random10"] = GenerateRandomString(10, true);
            if (!_cachedRandomValues.ContainsKey("random12"))
                _cachedRandomValues["random12"] = GenerateRandomString(12, false);
            if (!_cachedRandomValues.ContainsKey("random3"))
                _cachedRandomValues["random3"] = GenerateRandomString(3, true);
            
            result = result.Replace("$time", _cachedRandomValues["time"]);
            result = result.Replace("$random6", _cachedRandomValues["random6"]);
            result = result.Replace("$random10", _cachedRandomValues["random10"]);
            result = result.Replace("$random12", _cachedRandomValues["random12"]);
            result = result.Replace("$random3", _cachedRandomValues["random3"]);
            
            // 替换参数变量
            foreach (var param in Parameters)
            {
                var paramName = param.Parameter.Name;
                var paramValue = param.Parameter.GetValue();
                result = result.Replace($"${paramName}", paramValue);
            }
            
            // 替换case中的变量
            if (!string.IsNullOrEmpty(hitCase))
            {
                var matchedCase = Cases.FirstOrDefault(c => c.Name == hitCase);
                if (matchedCase != null && matchedCase.Variables != null)
                {
                    foreach (var kvp in matchedCase.Variables)
                    {
                        result = result.Replace($"${kvp.Key}", kvp.Value);
                    }
                }
            }
            
            // 替换旧的VariableMappings
            foreach (var mapping in VariableMappings.Values)
            {
                string value = mapping.DefaultValue;
                if (!string.IsNullOrEmpty(hitCase) && mapping.CaseValues.ContainsKey(hitCase))
                {
                    value = mapping.CaseValues[hitCase];
                }
                if (!string.IsNullOrEmpty(value))
                {
                    result = result.Replace($"${mapping.VariableName}", value);
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// 生成随机字符串
        /// </summary>
        private string GenerateRandomString(int length, bool numbersOnly)
        {
            const string numbers = "0123456789";
            const string alphanumeric = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            
            var chars = numbersOnly ? numbers : alphanumeric;
            var result = new System.Text.StringBuilder(length);
            var random = new System.Random();
            
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            
            return result.ToString();
        }
    
    }

    // 变量映射类
    [Serializable]
    public class VariableMapping
    {
        public string VariableName;
        public Dictionary<string, string> CaseValues = new(); // case名称 -> 变量值
        public string DefaultValue; // 默认值
    }

    // 变量映射包装器（用于Unity序列化）
    [Serializable]
    public class VariableMappingWrapper
    {
        public string Key;
        public VariableMapping Value;
    }


    [Serializable]
    public class OutputStage
    {
        public string StageName; // "initial", "waiting", "success"
        public float DelaySeconds; // 阶段延迟
        public Output Output; // 该阶段的输出
    }

    [Serializable]
    public class ParameterWrapper
    {
        [SerializeReference, HideReferenceObjectPicker]
        public Parameter Parameter;

        public static implicit operator Parameter(ParameterWrapper wrapper)
        {
            return wrapper.Parameter;
        }

        public static implicit operator ParameterWrapper(Parameter parameter)
        {
            return new ParameterWrapper { Parameter = parameter };
        }
    }

    // 参数定义
    [Serializable]
    public class Parameter
    {
        public                 string       Name;
        public                 string       Type;
        public                 List<string> Accept = new List<string>();
        [NonSerialized] public string       Value;

        public virtual string GetValue()
        {
            return Value;
        }
    }

    [Serializable]
    public class EmailParameter : Parameter
    {
        public override string GetValue()
        {
            return $"{Value}@bmail.com";
        }
    }

    // 键值对（可序列化）
    [Serializable]
    public class SerializableKeyValuePair
    {
        public string Key;
        public string Value;
    }

    // 测试用例
    [Serializable]
    public class Case
    {
        public string               Name;
        public List<ParameterValue> ParameterValues = new List<ParameterValue>();
        
        // 用于序列化显示的变量列表
        public List<SerializableKeyValuePair> VariablesList = new List<SerializableKeyValuePair>();
        
        // 运行时使用的变量字典（不序列化）
        [NonSerialized]
        private Dictionary<string, string> _variablesDict;
        
        public Dictionary<string, string> Variables
        {
            get
            {
                if (_variablesDict == null || _variablesDict.Count == 0)
                {
                    _variablesDict = new Dictionary<string, string>();
                    foreach (var kvp in VariablesList)
                    {
                        if (!string.IsNullOrEmpty(kvp.Key))
                        {
                            _variablesDict[kvp.Key] = kvp.Value;
                        }
                    }
                }
                return _variablesDict;
            }
        }
    }

    // 参数值
    [Serializable]
    public class ParameterValue
    {
        public string       Name;
        public List<string> Accept = new List<string>();
    }

    // 输出定义
    [Serializable]
    public class Output
    {
        public List<LanguageBlock> LanguageBlocks = new();
    }

    // 语言块
    [Serializable]
    public class LanguageBlock
    {
        public string                 Language; // 可以为空（无语言标识），"chinese" 或 "english"
        public string                 Case;     // 可选，指定此语言块只在特定case下显示
        public List<LineBlockWrapper> LineBlocks = new();
    }

    [Serializable]
    public class LineBlockWrapper
    {
        [SerializeReference, HideReferenceObjectPicker]
        public LineBlock LineBlock;
    }

    // 行块基类
    [Serializable]
    public abstract class LineBlock
    {
        public string Text;
        public string Type; // "normal" 或 "clickable"
        public float Delay; // 延时（秒），0表示无延时
    }

    // 普通行块
    [Serializable]
    public class NormalLineBlock : LineBlock
    {
        public NormalLineBlock()
        {
            Type = "normal";
        }
    }

    // 可点击行块
    [Serializable]
    public class ClickableLineBlock : LineBlock
    {
       
        public List<KeyValuePair> Gain = new();

        public ClickableLineBlock()
        {
            Type = "clickable";
        }

        [Serializable]
        public struct KeyValuePair
        {
            public string Key;
            public string Value;
        }
    }

    // 文件解析器
    public class CommandFileParser
    {
        private CommandDefinition InnerParseFile(string[] lines)
        {
            var command = new CommandDefinition();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.EndsWith(":"))
                {
                    var section = line.TrimEnd(':');

                    switch (section)
                    {
                        case "commandName":
                            command.CommandName = GetNextLineValue(lines, ref i);
                            break;
                        case "params":
                            ParseParameters(command, lines, ref i);
                            break;
                        case "variables":
                            ParseVariables(command, lines, ref i);
                            break;
                        case "output":
                            ParseOutput(command, lines, ref i);
                            break;
                        default:
                            if (section.StartsWith("case"))
                            {
                                ParseCase(command, section, lines, ref i);
                            }

                            break;
                    }
                }
            }

            return command;
        }

        public CommandDefinition ParseFile(TextAsset asset)
        {
            var lines = asset.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            return InnerParseFile(lines);
        }

        public CommandDefinition ParseFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            return InnerParseFile(lines);
        }

        private void ParseParameters(CommandDefinition command, string[] lines, ref int index)
        {
            while (++index < lines.Length)
            {
                var line = lines[index].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.StartsWith("- name :"))
                {
                    var param = new Parameter();
                    param.Name = line.Replace("- name :", "").Trim();

                    // 读取type和accept
                    while (++index < lines.Length)
                    {
                        var nextLine = lines[index].Trim();
                        if (string.IsNullOrWhiteSpace(nextLine)) break;

                        if (nextLine.StartsWith("type :"))
                        {
                            param.Type = nextLine.Replace("type :", "").Trim();
                        }
                        else if (nextLine.StartsWith("accept :"))
                        {
                            var acceptStr = nextLine.Replace("accept :", "").Trim();
                            param.Accept = ParseCommaSeparatedList(acceptStr);
                        }
                        else if (nextLine.StartsWith("- name :") || nextLine.StartsWith("case") ||
                                 nextLine.StartsWith("output"))
                        {
                            index--; // 回退，让外层处理下一个参数
                            break;
                        }
                    }

                    switch (param.Type)
                    {
                        case "email":
                            param = new EmailParameter
                            {
                                Name   = param.Name,
                                Type   = param.Type,
                                Accept = param.Accept
                            };
                            break;
                    }

                    command.Parameters.Add(param);
                }
                else if (!line.StartsWith("-") && !string.IsNullOrWhiteSpace(line))
                {
                    index--; // 回退到当前节
                    break;
                }
            }
        }

        private void ParseCase(CommandDefinition command, string caseName, string[] lines, ref int index)
        {
            var testCase = new Case { Name = caseName };

            while (++index < lines.Length)
            {
                var line = lines[index].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.StartsWith("- name :"))
                {
                    var paramValue = new ParameterValue();
                    paramValue.Name = line.Replace("- name :", "").Trim();

                    // 读取accept
                    if (++index < lines.Length)
                    {
                        var acceptLine = lines[index].Trim();
                        if (acceptLine.StartsWith("accept :"))
                        {
                            var acceptStr = acceptLine.Replace("accept :", "").Trim();
                            paramValue.Accept = ParseCommaSeparatedList(acceptStr);
                        }
                        else
                        {
                            index--; // 回退，让外层处理下一个参数值
                        }
                    }

                    testCase.ParameterValues.Add(paramValue);
                }
                else if (!line.StartsWith("-") && !string.IsNullOrWhiteSpace(line) || line.StartsWith("case") ||
                         line.StartsWith("output"))
                {
                    index--; // 回退到当前节
                    break;
                }
            }

            command.Cases.Add(testCase);
        }

        private void ParseVariables(CommandDefinition command, string[] lines, ref int index)
        {
            while (++index < lines.Length)
            {
                var line = lines[index].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.StartsWith("- name :"))
                {
                    var mapping = new VariableMapping();
                    mapping.VariableName = line.Replace("- name :", "").Trim();
                    mapping.CaseValues = new Dictionary<string, string>();

                    // 读取case相关的值和默认值
                    while (++index < lines.Length)
                    {
                        var nextLine = lines[index].Trim();
                        if (string.IsNullOrWhiteSpace(nextLine)) break;

                        if (nextLine.StartsWith("case") && nextLine.Contains(":"))
                        {
                            var parts = nextLine.Split(new[] { ':' }, 2);
                            if (parts.Length == 2)
                            {
                                var caseName = parts[0].Trim();
                                var value = parts[1].Trim();
                                mapping.CaseValues[caseName] = value;
                            }
                        }
                        else if (nextLine.StartsWith("default :"))
                        {
                            mapping.DefaultValue = nextLine.Replace("default :", "").Trim();
                        }
                        else if (nextLine.StartsWith("- name :") || 
                                 (nextLine.StartsWith("case") && nextLine.EndsWith(":")) ||
                                 nextLine.StartsWith("output") || nextLine.StartsWith("params"))
                        {
                            index--; // 回退，让外层处理下一个变量或下一个section
                            break;
                        }
                    }

                    command.VariableMappingsList.Add(mapping);
                }
                else if (!line.StartsWith("-") && !string.IsNullOrWhiteSpace(line))
                {
                    index--; // 回退到当前节
                    break;
                }
            }
        }

        private void ParseOutput(CommandDefinition command, string[] lines, ref int index)
        {
            var output = new Output();
            command.Output = output;

            LanguageBlock      currentLanguageBlock  = null;
            ClickableLineBlock currentClickableBlock = null;

            // 创建初始的无语言标识的语言块
            currentLanguageBlock = new LanguageBlock { Language = null };
            output.LanguageBlocks.Add(currentLanguageBlock);

            for (; index < lines.Length; index++)
            {
                var line = lines[index].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                // 检查是否是语言标识行
                if (line == "chinese:" || line == "english:")
                {
                    string language = line.TrimEnd(':');

                    // 创建新的语言块
                    currentLanguageBlock = new LanguageBlock { Language = language };
                    output.LanguageBlocks.Add(currentLanguageBlock);
                    currentClickableBlock = null;
                    continue;
                }

                // 检查是否是可点击块开始
                if (line == "clickable:")
                {
                    currentClickableBlock = new ClickableLineBlock();
                    currentLanguageBlock.LineBlocks.Add(new LineBlockWrapper
                    {
                        LineBlock = currentClickableBlock
                    });
                    continue;
                }

                // 处理文本内容
                if (line.StartsWith("text:"))
                {
                    string text = line.Substring(5).Trim();

                    if (currentClickableBlock != null)
                    {
                        // 可点击块的文本
                        currentClickableBlock.Text = text;
                    }
                    else
                    {
                        // 普通文本行
                        var normalBlock = new NormalLineBlock { Text = text };
                        currentLanguageBlock.LineBlocks.Add(new LineBlockWrapper
                        {
                            LineBlock = normalBlock
                        });
                    }

                    continue;
                }

                // 处理增益部分
                if (line.StartsWith("gain:"))
                {
                    if (currentClickableBlock == null) continue;

                    // 解析gain的键值对
                    while (++index < lines.Length)
                    {
                        var gainLine = lines[index].Trim();
                        if (string.IsNullOrWhiteSpace(gainLine) ||
                            gainLine == "chinese:" ||
                            gainLine == "english:" ||
                            gainLine == "clickable:")
                        {
                            index--; // 回退，让外层处理下一部分
                            break;
                        }

                        if (gainLine.Contains("="))
                        {
                            var parts = gainLine.Split('=');
                            if (parts.Length == 2)
                            {
                                var key   = parts[0].Trim();
                                var value = parts[1].Trim();
                                currentClickableBlock.Gain.Add(new ClickableLineBlock.KeyValuePair
                                {
                                    Key   = key,
                                    Value = value
                                });
                            }
                        }
                        else
                        {
                            index--; // 回退，让外层处理下一部分
                            break;
                        }
                    }

                    continue;
                }

                {
                    var normalBlock = new NormalLineBlock { Text = line };
                    currentLanguageBlock.LineBlocks.Add(new LineBlockWrapper
                    {
                        LineBlock = normalBlock
                    });
                }
                
            }
        }

        private List<string> ParseCommaSeparatedList(string input)
        {
            return input.Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();
        }

        private string GetNextLineValue(string[] lines, ref int index)
        {
            if (++index < lines.Length)
            {
                return lines[index].Trim();
            }

            return string.Empty;
        }

        public static void Test()
        {
            var parser  = new CommandFileParser();
            var command = parser.ParseFile("Assets/Scripts/Test.txt");

            Debug.Log("\n参数:");
            foreach (var param in command.Parameters)
            {
                var pa = param.Parameter;
                Debug.Log($"  {pa.Name} ({pa.Type}): {string.Join(", ", pa.Accept)}");
            }
            
            Debug.Log("\n变量映射:");
            foreach (var mapping in command.VariableMappings.Values)
            {
                Debug.Log($"  {mapping.VariableName}:");
                Debug.Log($"    Default: {mapping.DefaultValue}");
                foreach (var caseValue in mapping.CaseValues)
                {
                    Debug.Log($"    {caseValue.Key}: {caseValue.Value}");
                }
            }

            Debug.Log("\n测试用例:");
            foreach (var testCase in command.Cases)
            {
                Debug.Log($"  {testCase.Name}:");
                foreach (var paramValue in testCase.ParameterValues)
                {
                    Debug.Log($"    {paramValue.Name}: {string.Join(", ", paramValue.Accept)}");
                }
            }

            Debug.Log("\n输出内容:");
            foreach (var languageBlock in command.Output.LanguageBlocks)
            {
                Debug.Log($"语言块: {(string.IsNullOrEmpty(languageBlock.Language) ? "无语言标识" : languageBlock.Language)}");

                foreach (var lineBlock in languageBlock.LineBlocks)
                {
                    if (lineBlock.LineBlock is NormalLineBlock normalBlock)
                    {
                        Debug.Log($"  普通行: {normalBlock.Text}");
                    }
                    else if (lineBlock.LineBlock is ClickableLineBlock clickableBlock)
                    {
                        Debug.Log($"  可点击行: {clickableBlock.Text}");
                        foreach (var gain in clickableBlock.Gain)
                        {
                            Debug.Log($"    增益: {gain.Key} = {gain.Value}");
                        }
                    }
                }
            }
        }
    }
}