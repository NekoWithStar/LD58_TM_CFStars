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
        public string                 CommandName;
        public List<ParameterWrapper> Parameters = new();
        public List<Case>             Cases      = new();
        public Output                 Output;
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

    // 测试用例
    [Serializable]
    public class Case
    {
        public string               Name;
        public List<ParameterValue> ParameterValues = new List<ParameterValue>();
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