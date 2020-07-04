using CGAwesome.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAwesome.PackageExporters
{
    public class BedrockExporter : IExportMinecraftPackage
    {
        public void Export(StringBuilder fillCommands, string packageName, string functionName, string newVersion, DirectoryInfo packRoot)
        {
            using (StreamWriter sw = new StreamWriter($"{packRoot.FullName}\\functions\\{functionName}.mcfunction"))
            {
                sw.Write(fillCommands.ToString());
            }
            //TODO: Open behavior_pack_manifest and update version? Implement save/import of other scripts? Function slots? /function cga1, cga2, etc?

            if (!File.Exists($"{packRoot.FullName}\\manifest.json"))
            {
                using (StreamReader sr = new StreamReader(new FileStream($"{AppDomain.CurrentDomain.BaseDirectory}\\Manifest Templates\\behavior_pack_manifest.json", FileMode.Open)))
                using (StreamWriter sw = new StreamWriter($"{packRoot.FullName}\\manifest.json"))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();

                        switch (line.Split(':')[0].Trim().Replace("\"", ""))
                        {
                            case "uuid":
                                sw.WriteLine($"\"uuid\":\"{Guid.NewGuid().ToString()}\",");
                                break;
                            case "description":
                                sw.WriteLine($"\"description\":\"{packageName}\",");
                                break;
                            case "name":
                                sw.WriteLine($"\"name\":\"{packageName}\",");
                                break;
                            default:
                                sw.WriteLine(line);
                                break;
                        }
                    }
                }
            }
            else
            {
                var templateManifestPath = $"{packRoot.FullName}\\manifest.json";
                var newManifestPath = $"{packRoot.FullName}\\newmanifest.json";
                var oldManifestPath = $"{packRoot.FullName}\\oldmanifest.json";

                if (File.Exists(oldManifestPath)) File.Delete(oldManifestPath);

                using (StreamReader sr = new StreamReader(new FileStream(templateManifestPath, FileMode.Open)))
                using (StreamWriter sw = new StreamWriter(newManifestPath))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string version = newVersion;
                        switch (line.Split(':')[0].Trim().Replace("\"", ""))
                        {
                            case "version":
                                sw.WriteLine($"\"version\": [{version}],");
                                break;
                            default:
                                sw.WriteLine(line);
                                break;
                        }
                    }
                }

                File.Replace(newManifestPath, templateManifestPath, oldManifestPath);
                File.Delete(oldManifestPath);
            }
        }
    }
}
