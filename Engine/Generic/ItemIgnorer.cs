using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Windows.PowerShell.ScriptAnalyzer.Generic
{
    public class ItemIgnorer
    {
        private readonly string ignoreFileName = ".pssaignore";
        private static readonly string directorySeparator = Path.DirectorySeparatorChar.ToString();
        private static readonly string commentChar = "#";
        private string invocationDirectory;
        private List<string> ignorePatterns;
        private string ignoreFilePath;

        public ItemIgnorer(string invocationDir)
        {
            if (string.IsNullOrWhiteSpace(invocationDir))
            {
                throw new ArgumentException("Invocation directory is invalid", "invocationDir");
            }
            this.invocationDirectory = invocationDir;            
            ignoreFilePath = Path.Combine(this.invocationDirectory, ignoreFileName);
            ignorePatterns = null;
            if (File.Exists(ignoreFilePath))
            {
                ignorePatterns = new List<string>(ProcessIgnoreFileContent(File.ReadAllLines(ignoreFilePath)));
            }
        }

        public static IEnumerable<string> ProcessIgnoreFileContent(IEnumerable<string> ignoreFileLines)
        {
            // read the ignore file
            // remove blank lines
            // remove lines beginnings with comments
            // remove trailing whitespaces
            var output = new List<string>();
            foreach(var line in ignoreFileLines)
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                var lineTrimmed = line.Trim();
                if (lineTrimmed.StartsWith(commentChar))
                {
                    continue;
                }
                output.Add(lineTrimmed);
            }
            return output;
        }

        public static bool Test(string path, string basePath, string ignorePattern)
        {
            if (ignorePattern.EndsWith(directorySeparator))
            {
                // check path is part of basepath                
                var relativePath = path.Substring(basePath.Length);
                if (ignorePattern.IndexOf("*") == -1)
                {
                    if (relativePath.StartsWith(ignorePattern))
                    {
                        return true;
                    }
                }
                
            }
            return false;
        }

        private bool Test(string path, string ignorePattern)
        {
            return Test(path, invocationDirectory, ignorePattern);
        }
        public bool Test(string path)
        {
            if (ignorePatterns == null)
            {
                return false;
            }            
            foreach(var ignorePattern in ignorePatterns)
            {
                if (Test(path, ignorePattern))
                {
                    return true;
                }
            }
            return false;
        }
    }    
}
