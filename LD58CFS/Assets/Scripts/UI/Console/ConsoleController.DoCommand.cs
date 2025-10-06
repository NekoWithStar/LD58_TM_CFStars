#region

//文件创建者：Egg
//创建时间：10-04 10:33

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EggFramework.Util;
using LD58.Constant;
using LD58.Extension;
using LD58.Util;
using UnityEngine;

namespace LD58.UI
{
    public sealed partial class ConsoleController
    {
        private async void DoCommand()
        {
            _isExecutingCommand = true;
            var blankChar  = '░';
            var filledChar = '▓';
            Debug.Log("DoCommand: " + _command.CommandName);
            
            // 重置随机值缓存
            _command.ResetRandomCache();
            
            var paramDic = new Dictionary<string, string>();
            foreach (var commandParameter in _command.Parameters)
            {
                paramDic[commandParameter.Parameter.Name] =
                    commandParameter.Parameter.GetValue();
            }

            string hitCase = string.Empty;
            foreach (var commandCase in _command.Cases)
            {
                var hit = true;
                foreach (var param in commandCase.ParameterValues)
                {
                    if (!paramDic.ContainsKey(param.Name) ||
                        param.Accept.Count != 0 && !param.Accept.Contains(paramDic[param.Name]))
                    {
                        Debug.Log($"❌ Case '{commandCase.Name}' 不匹配: 参数 '{param.Name}' 期望 [{string.Join(", ", param.Accept)}], 实际 '{(paramDic.ContainsKey(param.Name) ? paramDic[param.Name] : "不存在")}'");
                        hit = false;
                        break;
                    }
                }

                if (hit)
                {
                    hitCase = commandCase.Name;
                    break;
                }
            }
            
            if (string.IsNullOrEmpty(hitCase))
            {
                Debug.LogWarning("没有匹配到任何case，变量将使用默认值");
            }

            foreach (var lb in _command.Output.LanguageBlocks)
            {
                // 检查语言是否匹配
                if (lb.Language == "chinese" && _settingModel.Language.Value == SystemLanguage.ChineseSimplified ||
                    lb.Language == "english" && _settingModel.Language.Value == SystemLanguage.English ||
                    string.IsNullOrEmpty(lb.Language))
                {
                    // 检查case是否匹配（如果LanguageBlock指定了case）
                    if (!string.IsNullOrEmpty(lb.Case) && lb.Case != hitCase)
                    {
                        continue; // 跳过不匹配的case
                    }
                    
                    foreach (var line in lb.LineBlocks)
                    {
                        var text = line.LineBlock.Text.Trim();
                        
                        // 先替换变量
                        text = _command.ParseVariables(text, hitCase);
                        
                        if (text.StartsWith("case"))
                        {
                            var caseParts = text.Split(":", StringSplitOptions.RemoveEmptyEntries);
                            if (caseParts[0] == hitCase)
                            {
                                text = caseParts[1];
                                // 再次替换变量（因为case后面的内容也可能包含变量）
                                text = _command.ParseVariables(text, hitCase);
                            }
                            else
                            {
                                continue;
                            }
                        }

                        switch (line.LineBlock)
                        {
                            case ClickableLineBlock clickableLineBlock:
                            {
                                var item = AddItem(text);
                                foreach (var keyValuePair in clickableLineBlock.Gain)
                                {
                                    // 对gain的值也进行变量替换
                                    var gainValue = _command.ParseVariables(keyValuePair.Value, hitCase);
                                    item.BlackBoardItems[keyValuePair.Key] = gainValue;
                                }
                                
                                // 处理延时
                                if (clickableLineBlock.Delay > 0)
                                {
                                    await UniTask.WaitForSeconds(clickableLineBlock.Delay);
                                }
                            }
                                break;
                            case NormalLineBlock normalLineBlock:
                            {
                                if (text.StartsWith("$$"))
                                {
                                    if (text.Contains("$$wait"))
                                    {
                                        var parts = text.Split(new[] { "$$wait", "(", ")" },
                                            StringSplitOptions.RemoveEmptyEntries);
                                        if (parts.Length == 1 && float.TryParse(parts[0], out var waitTime))
                                        {
                                            await UniTask.WaitForSeconds(waitTime);
                                        }

                                        continue;
                                    }
                                    else if (text.Contains("$$progress"))
                                    {
                                        // $$progress(0.128,20," /manager"," /manager         » 404 Not Found (128ms)","white","red")
                                        var parts = SplitParameters(text, "$$progress");
                                        if (parts.Count == 6 && float.TryParse(parts[0], out var duration) &&
                                            int.TryParse(parts[1], out var total))
                                        {
                                            var item = AddItem(
                                                $"    [{new string(blankChar, total)} 0%]{parts[2]}");
                                            await DOTween
                                                .To(
                                                    val =>
                                                    {
                                                        item.SetText(
                                                            $"    [{new string(filledChar, (int)(val / (100f / total)))}{new string(blankChar, total - (int)(val / (100f / total)))} {(int)val}%]{parts[2]}");
                                                        item.SetColor(ColorUtil.ParseColor(parts[4]));
                                                    }, 0, 100, duration).ToUniTask();
                                            item.SetText(
                                                $"    [{new string(filledChar, total)} 100%]{parts[3]}".ToColor(
                                                    ColorUtil.ParseColor(parts[5])));
                                        }
                                    }
                                    else if (text.Contains("$$scene"))
                                    {
                                        // $$scene(Level3) - Load a new scene
                                        var parts = text.Split(new[] { "$$scene", "(", ")" },
                                            StringSplitOptions.RemoveEmptyEntries);
                                        if (parts.Length == 1 && !string.IsNullOrEmpty(parts[0]))
                                        {
                                            var sceneName = parts[0].Trim();
                                            Debug.Log($"Loading scene: {sceneName}");
                                            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                                        }

                                        continue;
                                    }
                                }
                                else
                                {
                                    AddItem(text);
                                }
                                
                                // 处理延时
                                if (normalLineBlock.Delay > 0)
                                {
                                    await UniTask.WaitForSeconds(normalLineBlock.Delay);
                                }
                            }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }

            // switch (_command.CommandName)
            // {
            //     case CommandConstant.WEB_SCAN:
            //         Debug.Log(_command.Parameters[0].Parameter.GetValue());
            //         ConsoleItem item = null;
            //         AddItem("    ----------------------------------------");
            //         AddItem("BuBugBomb WebScan v2.3.5 - Target: www.bmail.com");
            //         AddItem("Scan ID: BSID-230518-007 | Initiated: " +
            //                 System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC");
            //         AddItem("    ----------------------------------------");
            //         await UniTask.WaitForSeconds(0.5f);
            //         AddItem("    [SCAN SUMMARY]");
            //         AddItem("    ▸ Target: www.bmail.com ");
            //         AddItem("    ▸ Protocol: HTTPS/1.1");
            //         AddItem("    ▸ Methods: GET, HEAD, OPTIONS");
            //         AddItem("    ▸ Scanned paths: 12 | Time elapsed: 8.2s");
            //         AddItem("        [PATH ENUMERATION PROGRESS]");
            //         await UniTask.WaitForSeconds(0.5f);
            //         item = AddItem("    [░░░░░░░░░░░░░░░░░░░░ 0%] /manager        ");
            //         await DOTween
            //             .To(
            //                 val =>
            //                 {
            //                     item.SetText(
            //                         $"    [{new string(filledChar, (int)(val / 5))}{new string(blankChar, 20 - (int)(val / 5))} {(int)val}%] /manager      ");
            //                 }, 0, 100, 0.128f).ToUniTask();
            //         item.SetText($"    [{new string(filledChar, 20)} 100%] /manager         » 404 Not Found (128ms)".ToRed());
            //         await UniTask.WaitForSeconds(0.3f);
            //         item = AddItem("    [░░░░░░░░░░░░░░░░░░░░ 0%] /administrator        ");
            //         await DOTween
            //             .To(
            //                 val =>
            //                 {
            //                     item.SetText(
            //                         $"    [{new string(filledChar, (int)(val / 5))}{new string(blankChar, 20 - (int)(val / 5))} {(int)val}%] /administrator      ");
            //                 }, 0, 100, 0.142f).ToUniTask();
            //         item.SetText($"    [{new string(filledChar, 20)} 100%] /administrator   » 404 Not Found (142ms)".ToRed());
            //         await UniTask.WaitForSeconds(0.3f);
            //         item = AddItem("    [░░░░░░░░░░░░░░░░░░ 0%] /wp-admin        ");
            //         await DOTween
            //             .To(
            //                 val =>
            //                 {
            //                     item.SetText(
            //                         $"    [{new string(filledChar, (int)(val / 5))}{new string(blankChar, 20 - (int)(val / 5))} {(int)val}%] /wp-admin      ");
            //                 }, 0, 100, 0.151f).ToUniTask();
            //         item.SetText($"    [{new string(filledChar, 20)} 100%] /wp-admin        » 404 Not Found (151ms)".ToRed());
            //         item = AddItem("    [░░░░░░░░░░░░░░░░░░░░ 0%] /backend        ");
            //         await DOTween
            //             .To(
            //                 val =>
            //                 {
            //                     item.SetText(
            //                         $"    [{new string(filledChar, (int)(val / 5))}{new string(blankChar, 20 - (int)(val / 5))} {(int)val}%] /backend      ");
            //                 }, 0, 100, 0.138f).ToUniTask();
            //         item.SetText($"    [{new string(filledChar, 20)} 100%] /backend         » 404 Not Found (138ms)"
            //             .ToRed());
            //         await UniTask.WaitForSeconds(0.3f);
            //
            //         item = AddItem("    [░░░░░░░░░░░░░░░░░░░░ 0%] /console        ");
            //         await DOTween
            //             .To(
            //                 val =>
            //                 {
            //                     item.SetText(
            //                         $"    [{new string(filledChar, (int)(val / 5))}{new string(blankChar, 20 - (int)(val / 5))} {(int)val}%] /console      ");
            //                 }, 0, 100, 0.212f).ToUniTask();
            //         item.SetText(
            //             $"    [{new string(filledChar, 20)} 100%] /console         » 302 Found → Location: /login (212ms)"
            //                 .ToYellow());
            //         await UniTask.WaitForSeconds(0.3f);
            //
            //         item = AddItem("    [░░░░░░░░░░░░░░░░░░░░ 0%] /control-panel        ");
            //         await DOTween
            //             .To(
            //                 val =>
            //                 {
            //                     item.SetText(
            //                         $"    [{new string(filledChar, (int)(val / 5))}{new string(blankChar, 20 - (int)(val / 5))} {(int)val}%] /control-panel      ");
            //                 }, 0, 100, 0.125f).ToUniTask();
            //         item.SetText($"    [{new string(filledChar, 20)} 100%] /control-panel   » 404 Not Found (125ms)"
            //             .ToRed());
            //         await UniTask.WaitForSeconds(0.3f);
            //         
            //         item = AddItem("    [░░░░░░░░░░░░░░░░░░░░ 0%] /admin        ");
            //         await DOTween
            //             .To(
            //                 val =>
            //                 {
            //                     item.SetText(
            //                         $"    [{new string(filledChar, (int)(val / 5))}{new string(blankChar, 20 - (int)(val / 5))} {(int)val}%] /admin      ");
            //                 }, 0, 100, 0.184f).ToUniTask();
            //         item.SetText(
            //             $"    [{new string(filledChar, 20)} 100%] /admin           » 200 OK (184ms) ◀──[CRITICAL]"
            //                 .ToGreen());
            //         await UniTask.WaitForSeconds(0.2f);
            //         AddItem("                     ├── Server: nginx/1.18.0");
            //         AddItem("                     ├── Content-Type: text/html");
            //         AddItem("                     └── X-Powered-By: PHP/7.4.33");
            //         await UniTask.WaitForSeconds(0.5f);
            //         
            //         AddItem("        [ADDITIONAL PATHS SCANNED]");
            //         AddItem("    /dashboard     » 200 OK (201ms)".ToGreen());
            //         AddItem("    /system        » 403 Forbidden (225ms)");
            //         AddItem("    /bmail-admin   » 404 Not Found (132ms)");
            //         AddItem("    /mail-manager  » 404 Not Found (129ms)");
            //         AddItem("    /kesi-console  » 404 Not Found (131ms)");
            //         await UniTask.WaitForSeconds(0.6f);
            //
            //         // 分隔线
            //         AddItem("    ----------------------------------------");
            //
            //         // 扫描结论
            //         AddItem("    [SCAN CONCLUSIONS]");
            //         AddItem("    ▸ Valid admin paths: 3");
            //         AddItem("    ▸ Security rating: Critical (Vulnerable admin portal)");
            //         AddItem("    ▸ Recommended action: Inspect /admin endpoint");
            //         await UniTask.WaitForSeconds(0.5f);
            //
            //         // 检测到的管理面板
            //         AddItem("    [DETECTED ADMIN PORTALS]");
            //         AddItem("1. https://www.bmail.com/admin");
            //         AddItem("    ├── Status: Unauthenticated access allowed");
            //         AddItem("    ├── Framework: Custom PHP application");
            //         AddItem("2. https://www.bmail.com/console");
            //         AddItem("    ├── Status: Redirects to authentication");
            //         AddItem("    ├── Framework: Spring Boot 2.7.3");
            //         AddItem("3. https://www.bmail.com/dashboard");
            //         AddItem("    ├── Status: Requires authentication");
            //         AddItem("    ├── Framework: React.js 18.2.0");
            //         await UniTask.WaitForSeconds(0.6f);
            //
            //         // 最终信息
            //         AddItem("    ----------------------------------------");
            //         AddItem("    [INFO] Recommended action: www.bmail.com/admin".ToGreen());
            //         AddItem("    [INFO] 扫描完成: www.bmail.com/admin".ToGreen());
            //         break;
            //     case CommandConstant.SEND_EMAIL:
            //
            //         break;
            // }

            ListenForCommand    = false;
            _isExecutingCommand = false;
            AddFinishLine();
        }

        private List<string> SplitParameters(string input, string header)
        {
            var  parameters      = new List<string>();
            var  currentParam    = new StringBuilder();
            bool inQuotes        = false;
            bool escapeNext      = false;
            int  bracketDepth    = 0;
            bool inProgressBlock = false;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (escapeNext)
                {
                    currentParam.Append(c);
                    escapeNext = false;
                    continue;
                }

                if (c == '\\')
                {
                    escapeNext = true;
                    currentParam.Append(c);
                    continue;
                }

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    currentParam.Append(c);
                    continue;
                }

                // 检测 $$progress 开始
                if (!inProgressBlock && i + header.Length < input.Length && input.Substring(i, header.Length) == header)
                {
                    inProgressBlock =  true;
                    i               += header.Length - 1; // 跳过 "$$progress"
                    continue;
                }

                if (inProgressBlock)
                {
                    if (c == '(' && !inQuotes)
                    {
                        bracketDepth++;
                        if (bracketDepth == 1)
                        {
                            // 第一个左括号，不添加到参数中
                            continue;
                        }
                    }
                    else if (c == ')' && !inQuotes)
                    {
                        bracketDepth--;
                        if (bracketDepth == 0)
                        {
                            // 最后一个右括号，结束解析
                            if (currentParam.Length > 0)
                            {
                                parameters.Add(currentParam.ToString().Trim());
                                currentParam.Clear();
                            }

                            inProgressBlock = false;
                            continue;
                        }
                    }

                    if (bracketDepth >= 1)
                    {
                        if (c == ',' && !inQuotes && bracketDepth == 1)
                        {
                            // 在顶层且不在引号内的逗号，分割参数
                            parameters.Add(currentParam.ToString().Trim());
                            currentParam.Clear();
                        }
                        else
                        {
                            currentParam.Append(c);
                        }
                    }
                }
            }

            // 处理最后一个参数（如果有）
            if (currentParam.Length > 0)
            {
                parameters.Add(currentParam.ToString().Trim());
            }

            for (var i = 0; i < parameters.Count; i++)
            {
                parameters[i] = parameters[i].Trim('"');
            }

            return parameters;
        }
    }
}