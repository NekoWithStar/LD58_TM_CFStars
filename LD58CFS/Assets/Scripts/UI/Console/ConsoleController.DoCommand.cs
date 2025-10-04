#region

//文件创建者：Egg
//创建时间：10-04 10:33

#endregion

using Cysharp.Threading.Tasks;
using DG.Tweening;
using LD58.Constant;
using LD58.Extension;
using UnityEngine;

namespace LD58.UI
{
    public sealed partial class ConsoleController
    {
        private async void DoCommand()
        {
            var blankChar  = '░';
            var filledChar = '▓';
            Debug.Log("DoCommand: " + _command.CommandName);
            switch (_command.CommandName)
            {
                case CommandConstant.WEB_SCAN:
                    ConsoleItem item = null;
                    AddItem("----------------------------------------");
                    AddItem("BuBugBomb WebScan v2.3.5 - Target: www.bmail.com");
                    AddItem("Scan ID: BSID-230518-007 | Initiated: " +
                            System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC");
                    AddItem("    ----------------------------------------");
                    await UniTask.WaitForSeconds(0.5f);
                    AddItem("    [SCAN SUMMARY]");
                    AddItem("    ▸ Target: www.bmail.com ");
                    AddItem("    ▸ Protocol: HTTPS/1.1");
                    AddItem("    ▸ Methods: GET, HEAD, OPTIONS");
                    AddItem("    ▸ Scanned paths: 12 | Time elapsed: 8.2s");
                    AddItem("        [PATH ENUMERATION PROGRESS]");
                    await UniTask.WaitForSeconds(0.5f);
                    item = AddItem("    [░░░░░░░░░░░░░░░░░░░░ 0%] /manager        ");
                    await DOTween
                        .To(
                            val =>
                            {
                                item.SetText(
                                    $"    [{new string(filledChar, (int)(val / 5))}{new string(blankChar, 20 - (int)(val / 5))} {(int)val}%] /manager      ");
                            }, 0, 100, 0.128f).ToUniTask();
                    item.SetText($"    [{new string(filledChar, 20)} 100%] /manager         » 404 Not Found (128ms)".ToRed());
                    await UniTask.WaitForSeconds(0.3f);
                    item = AddItem("    [░░░░░░░░░░░░░░░░░░░░ 0%] /administrator        ");
                    await DOTween
                        .To(
                            val =>
                            {
                                item.SetText(
                                    $"    [{new string(filledChar, (int)(val / 5))}{new string(blankChar, 20 - (int)(val / 5))} {(int)val}%] /administrator      ");
                            }, 0, 100, 0.142f).ToUniTask();
                    item.SetText($"    [{new string(filledChar, 20)} 100%] /administrator   » 404 Not Found (142ms)".ToRed());
                    await UniTask.WaitForSeconds(0.3f);
                    item = AddItem("    [░░░░░░░░░░░░░░░░░░ 0%] /wp-admin        ");
                    await DOTween
                        .To(
                            val =>
                            {
                                item.SetText(
                                    $"    [{new string(filledChar, (int)(val / 5))}{new string(blankChar, 20 - (int)(val / 5))} {(int)val}%] /wp-admin      ");
                            }, 0, 100, 0.151f).ToUniTask();
                    item.SetText($"    [{new string(filledChar, 20)} 100%] /wp-admin        » 404 Not Found (151ms)".ToRed());
                    item = AddItem("    [░░░░░░░░░░░░░░░░░░░░ 0%] /backend        ");
                    await DOTween
                        .To(
                            val =>
                            {
                                item.SetText(
                                    $"    [{new string(filledChar, (int)(val / 5))}{new string(blankChar, 20 - (int)(val / 5))} {(int)val}%] /backend      ");
                            }, 0, 100, 0.138f).ToUniTask();
                    item.SetText($"    [{new string(filledChar, 20)} 100%] /backend         » 404 Not Found (138ms)"
                        .ToRed());
                    await UniTask.WaitForSeconds(0.3f);

                    item = AddItem("    [░░░░░░░░░░░░░░░░░░░░ 0%] /console        ");
                    await DOTween
                        .To(
                            val =>
                            {
                                item.SetText(
                                    $"    [{new string(filledChar, (int)(val / 5))}{new string(blankChar, 20 - (int)(val / 5))} {(int)val}%] /console      ");
                            }, 0, 100, 0.212f).ToUniTask();
                    item.SetText(
                        $"    [{new string(filledChar, 20)} 100%] /console         » 302 Found → Location: /login (212ms)"
                            .ToYellow());
                    await UniTask.WaitForSeconds(0.3f);

                    item = AddItem("    [░░░░░░░░░░░░░░░░░░░░ 0%] /control-panel        ");
                    await DOTween
                        .To(
                            val =>
                            {
                                item.SetText(
                                    $"    [{new string(filledChar, (int)(val / 5))}{new string(blankChar, 20 - (int)(val / 5))} {(int)val}%] /control-panel      ");
                            }, 0, 100, 0.125f).ToUniTask();
                    item.SetText($"    [{new string(filledChar, 20)} 100%] /control-panel   » 404 Not Found (125ms)"
                        .ToRed());
                    await UniTask.WaitForSeconds(0.3f);
                    
                    item = AddItem("    [░░░░░░░░░░░░░░░░░░░░ 0%] /admin        ");
                    await DOTween
                        .To(
                            val =>
                            {
                                item.SetText(
                                    $"    [{new string(filledChar, (int)(val / 5))}{new string(blankChar, 20 - (int)(val / 5))} {(int)val}%] /admin      ");
                            }, 0, 100, 0.184f).ToUniTask();
                    item.SetText(
                        $"    [{new string(filledChar, 20)} 100%] /admin           » 200 OK (184ms) ◀──[CRITICAL]"
                            .ToGreen());
                    await UniTask.WaitForSeconds(0.2f);
                    AddItem("                     ├── Server: nginx/1.18.0");
                    AddItem("                     ├── Content-Type: text/html");
                    AddItem("                     └── X-Powered-By: PHP/7.4.33");
                    await UniTask.WaitForSeconds(0.5f);
                    
                    AddItem("        [ADDITIONAL PATHS SCANNED]");
                    AddItem("    /dashboard     » 200 OK (201ms)".ToGreen());
                    AddItem("    /system        » 403 Forbidden (225ms)");
                    AddItem("    /bmail-admin   » 404 Not Found (132ms)");
                    AddItem("    /mail-manager  » 404 Not Found (129ms)");
                    AddItem("    /kesi-console  » 404 Not Found (131ms)");
                    await UniTask.WaitForSeconds(0.6f);

                    // 分隔线
                    AddItem("    ----------------------------------------");

                    // 扫描结论
                    AddItem("    [SCAN CONCLUSIONS]");
                    AddItem("    ▸ Valid admin paths: 3");
                    AddItem("    ▸ Security rating: Critical (Vulnerable admin portal)");
                    AddItem("    ▸ Recommended action: Inspect /admin endpoint");
                    await UniTask.WaitForSeconds(0.5f);

                    // 检测到的管理面板
                    AddItem("    [DETECTED ADMIN PORTALS]");
                    AddItem("1. https://www.bmail.com/admin");
                    AddItem("    ├── Status: Unauthenticated access allowed");
                    AddItem("    ├── Framework: Custom PHP application");
                    AddItem("2. https://www.bmail.com/console");
                    AddItem("    ├── Status: Redirects to authentication");
                    AddItem("    ├── Framework: Spring Boot 2.7.3");
                    AddItem("3. https://www.bmail.com/dashboard");
                    AddItem("    ├── Status: Requires authentication");
                    AddItem("    ├── Framework: React.js 18.2.0");
                    await UniTask.WaitForSeconds(0.6f);

                    // 最终信息
                    AddItem("    ----------------------------------------");
                    AddItem("    [INFO] Recommended action: www.bmail.com/admin".ToGreen());
                    AddItem("    [INFO] 扫描完成: www.bmail.com/admin".ToGreen());
                    break;
                case CommandConstant.SEND_EMAIL:

                    break;
            }

            ListenForCommand = false;
            AddFinishLine();
        }
    }
}