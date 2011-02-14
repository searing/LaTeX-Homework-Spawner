using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LaTeX_Homework_Spawner {
    static class HomeworkSpawner {
        // TODO - catch exceptions.
        // TODO - save to a real location.
        public static void Spawn(string template, IDictionary<string, string> strArgs, IDictionary<string, bool> boolArgs) {
            try {
                string latexString = "";
                using (StreamReader sr = new StreamReader(new FileStream(template, FileMode.Open, FileAccess.Read, FileShare.Read))) {
                    string rawTemplate = sr.ReadToEnd();
                    latexString = createLatexString(rawTemplate, strArgs, boolArgs);
                }
                if (latexString != "") {
                    using (StreamWriter sw = new StreamWriter(new FileStream(@"C:\LaTeX Homework\test.tex", FileMode.Create, FileAccess.Write, FileShare.None))) {
                        sw.Write(latexString);
                    }
                }
            } catch (IOException) {
            }
        }

        private static string createLatexString(string source, IDictionary<string, string> strArgs, IDictionary<string, bool> boolArgs) {
            string formatted = source;
            foreach (string key in boolArgs.Keys) {
                formatted = formatted.Replace("<<not." + key + ">>", boolArgs[key] ? "%%% " : "");
                formatted = formatted.Replace("<<" + key + ">>", boolArgs[key] ? "" : "%%% ");
            }
            foreach (string key in strArgs.Keys) {
                formatted = formatted.Replace("<<" + key + ">>", strArgs[key]);
            }
            return formatted;
        }
    }
}
