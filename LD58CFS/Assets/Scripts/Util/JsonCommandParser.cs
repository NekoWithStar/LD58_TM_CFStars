using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace LD58.Util
{
    /// <summary>
    /// JSON格式命令解析器
    /// </summary>
    public class JsonCommandParser
    {
        [Serializable]
        public class JsonCommandDefinition
        {
            public string commandName;
            public List<JsonParameter> parameters;
            public List<JsonVariable> variables;
            public List<JsonCase> cases;
            public JsonOutput output;
        }

        [Serializable]
        public class JsonParameter
        {
            public string name;
            public string type;
            public List<string> accept;
        }

        [Serializable]
        public class JsonVariable
        {
            public string name;
            public string defaultValue;
            public Dictionary<string, string> caseValues;
        }

        [Serializable]
        public class JsonCase
        {
            public string name;
            public Dictionary<string, List<string>> parameterValues;
            public Dictionary<string, string> variables; 
        }

        [Serializable]
        public class JsonOutput
        {
            public List<JsonLanguageBlock> languageBlocks;
        }

        [Serializable]
        public class JsonLanguageBlock
        {
            public string language;
            public List<JsonLineBlock> lineBlocks;
        }

        [Serializable]
        public class JsonLineBlock
        {
            public string type; // "normal" or "clickable"
            public string text;
            public float delay; // 延时（秒）
            public Dictionary<string, string> gain; // For clickable blocks
        }

        /// <summary>
        /// 从JSON文件解析命令定义
        /// </summary>
        public static CommandDefinition ParseFile(string filePath)
        {
            try
            {
                var jsonText = File.ReadAllText(filePath);
                var jsonDef = JsonConvert.DeserializeObject<JsonCommandDefinition>(jsonText);

                if (jsonDef == null)
                {
                    throw new Exception("JSON解析失败：返回null");
                }

                return ConvertToCommandDefinition(jsonDef);
            }
            catch (JsonException ex)
            {
                Debug.LogError($"JSON格式错误: {filePath}\n{ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"解析失败: {filePath}\n{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 将JSON定义转换为CommandDefinition
        /// </summary>
        private static CommandDefinition ConvertToCommandDefinition(JsonCommandDefinition jsonDef)
        {
            var command = new CommandDefinition
            {
                CommandName = jsonDef.commandName,
                Parameters = new List<ParameterWrapper>(),
                Cases = new List<Case>(),
                Output = new Output()
            };

            // 转换参数
            if (jsonDef.parameters != null)
            {
                foreach (var jsonParam in jsonDef.parameters)
                {
                    Parameter param = null;

                    switch (jsonParam.type)
                    {
                        case "email":
                            param = new EmailParameter
                            {
                                Name = jsonParam.name,
                                Type = jsonParam.type,
                                Accept = jsonParam.accept ?? new List<string>()
                            };
                            break;
                        default:
                            param = new Parameter
                            {
                                Name = jsonParam.name,
                                Type = jsonParam.type,
                                Accept = jsonParam.accept ?? new List<string>()
                            };
                            break;
                    }

                    command.Parameters.Add(new ParameterWrapper { Parameter = param });
                }
            }

            // 转换变量
            if (jsonDef.variables != null)
            {
                foreach (var jsonVar in jsonDef.variables)
                {
                    var mapping = new VariableMapping
                    {
                        VariableName = jsonVar.name,
                        DefaultValue = jsonVar.defaultValue ?? "",
                        CaseValues = jsonVar.caseValues ?? new Dictionary<string, string>()
                    };

                    command.VariableMappingsList.Add(mapping);
                }
            }
            // 转换Cases
            if (jsonDef.cases != null)
            {
                foreach (var jsonCase in jsonDef.cases)
                {
                    var testCase = new Case
                    {
                        Name = jsonCase.name,
                        ParameterValues = new List<ParameterValue>(),
                        VariablesList = new List<SerializableKeyValuePair>()
                    };

                    if (jsonCase.parameterValues != null)
                    {
                        foreach (var kvp in jsonCase.parameterValues)
                        {
                            testCase.ParameterValues.Add(new ParameterValue
                            {
                                Name = kvp.Key,
                                Accept = kvp.Value
                            });
                        }
                    }

                    // 转换case的变量
                    if (jsonCase.variables != null)
                    {
                        foreach (var kvp in jsonCase.variables)
                        {
                            testCase.VariablesList.Add(new SerializableKeyValuePair
                            {
                                Key = kvp.Key,
                                Value = kvp.Value
                            });
                        }
                    }

                    command.Cases.Add(testCase);
                }
            }

            // 转换Output
            if (jsonDef.output != null && jsonDef.output.languageBlocks != null)
            {
                foreach (var jsonLangBlock in jsonDef.output.languageBlocks)
                {
                    var langBlock = new LanguageBlock
                    {
                        Language = jsonLangBlock.language,
                        LineBlocks = new List<LineBlockWrapper>()
                    };

                    if (jsonLangBlock.lineBlocks != null)
                    {
                        foreach (var jsonLineBlock in jsonLangBlock.lineBlocks)
                        {
                            LineBlock lineBlock = null;

                            if (jsonLineBlock.type == "clickable")
                            {
                                var clickableBlock = new ClickableLineBlock
                                {
                                    Text = jsonLineBlock.text,
                                    Delay = jsonLineBlock.delay,
                                    Gain = new List<ClickableLineBlock.KeyValuePair>()
                                };

                                if (jsonLineBlock.gain != null)
                                {
                                    foreach (var gainKvp in jsonLineBlock.gain)
                                    {
                                        clickableBlock.Gain.Add(new ClickableLineBlock.KeyValuePair
                                        {
                                            Key = gainKvp.Key,
                                            Value = gainKvp.Value
                                        });
                                    }
                                }

                                lineBlock = clickableBlock;
                            }
                            else
                            {
                                lineBlock = new NormalLineBlock
                                {
                                    Text = jsonLineBlock.text,
                                    Delay = jsonLineBlock.delay
                                };
                            }

                            langBlock.LineBlocks.Add(new LineBlockWrapper
                            {
                                LineBlock = lineBlock
                            });
                        }
                    }

                    command.Output.LanguageBlocks.Add(langBlock);
                }
            }

            Debug.Log($"JSON解析成功: {command.CommandName}");
            Debug.Log($"  - 参数: {command.Parameters.Count}个");
            Debug.Log($"  - 变量: {command.VariableMappingsList.Count}个");
            Debug.Log($"  - Cases: {command.Cases.Count}个");

            return command;
        }
    }
}
