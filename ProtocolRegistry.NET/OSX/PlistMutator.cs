using System;
using System.IO;
using Claunia.PropertyList;

namespace LingUnion.OSX
{
    public partial class ProtocolRegistry
    {
        public void PlistMutator(string app, string protocol)
        {
            FileInfo plistInfo = new FileInfo(Path.Join(app, "./Contents/Info.plist"));
            NSDictionary rootDict = (NSDictionary)PropertyListParser.Parse(plistInfo);
            rootDict["CFBundleIdentifier"] = (NSString)$"com.protocol.registry.{protocol}";
            NSDictionary cfBundleURLTypes = new NSDictionary();
            cfBundleURLTypes.Add("CFBundleURLName", $"URL : {protocol}");
            cfBundleURLTypes.Add("CFBundleURLSchemes", new[] { protocol });
            rootDict["CFBundleURLTypes"] = new NSArray(){ cfBundleURLTypes};

            PropertyListParser.SaveAsXml(rootDict, plistInfo);
        }
    }
}
