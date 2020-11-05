using System.IO;
using UnityEditor.Android;
using System.Text;
using System.Xml;
using UnityEngine;

public class CelerXUnityAndroidApp : IPostGenerateGradleAndroidProject
{
    public void OnPostGenerateGradleAndroidProject(string basePath)
    {
        string rootProject = Path.Combine(basePath, "..");
        modifyGradleProperties(rootProject);
        overrideRootBuildGradle(rootProject);
        overrideLauncherBuildGradle(rootProject);
        overrideUnityLibBuildGradle(rootProject);
        copyGoogleSeriviceJson(rootProject);

        var lancherModuleManifest = new AndroidManifest(GetManifestPath(rootProject));
        lancherModuleManifest.setApplicationToolReplace("android:label,android:icon,android:roundIcon");
        lancherModuleManifest.Save();

        //File.Delete(rootProject + "/unityLibrary/libs/unity-classes.jar");

        //Directory.Delete(rootProject + "/unityLibrary/src/main/jniLibs/", true);

        var libraryModuleManifest = new AndroidManifest(getLibraryManifestPath(rootProject));
        libraryModuleManifest.removeLaunchMode();
        libraryModuleManifest.Save();
    }

    public int callbackOrder { get { return 1; } }

    private void modifyGradleProperties(string rootProject)
    {
        string gradlePropertiesFile = rootProject + "/gradle.properties";
        if (File.Exists(gradlePropertiesFile))
        {
            File.Delete(gradlePropertiesFile);
        }
        StreamWriter writer = File.CreateText(gradlePropertiesFile);
        writer.WriteLine("org.gradle.jvmargs=-Xmx4096M");
        writer.WriteLine("android.useAndroidX=true");
        writer.WriteLine("android.enableJetifier=true");
        writer.WriteLine("android.injected.testOnly=false");
        writer.Flush();
        writer.Close();
    }

    private void overrideRootBuildGradle(string rootProject)
    {
        string launcherBuildGradle = "Assets/Plugins/Android/build.gradle";
        string destFile = rootProject + "/build.gradle";

        File.Copy(launcherBuildGradle, destFile, true);
    }

    private void overrideLauncherBuildGradle(string rootProject)
    { 
        string launcherBuildGradle = "Assets/Plugins/Android/launcherBuild.gradle";
        string destFile = rootProject + "/launcher/build.gradle";

        File.Copy(launcherBuildGradle, destFile, true);
    }

    private void overrideUnityLibBuildGradle(string rootProject) {
        string launcherBuildGradle = "Assets/Plugins/Android/unityLibraryBuild.gradle";
        string destFile = rootProject + "/unityLibrary/build.gradle";

        File.Copy(launcherBuildGradle, destFile, true);
    }

    private void copyGoogleSeriviceJson(string rootProject)
    {
        string destFile = rootProject + "/launcher/google-services.json";
        string[] files = Directory.GetFiles("Assets/Plugins/Android", "google-services.json", SearchOption.AllDirectories);
        if (files.Length > 0)
        {
            File.Copy(files[0], destFile);
        }
        else
        {
            Debug.Log("there is no google-service.json in Assets/Plugins/Android");
        }
    }

    private string _manifestFilePath;
    private string _libraryManifestFilePath;

    private string GetManifestPath(string rootPath)
    {
        string launcherModulePath = rootPath + "/launcher/";
        if (string.IsNullOrEmpty(_manifestFilePath))
        {
            var pathBuilder = new StringBuilder(launcherModulePath);
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("src");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("main");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("AndroidManifest.xml");
            _manifestFilePath = pathBuilder.ToString();
        }
        return _manifestFilePath;
    }

    private string getLibraryManifestPath(string rootPath)
    {
        string libraryModulePath = rootPath + "/unityLibrary/";
        if (string.IsNullOrEmpty(_libraryManifestFilePath))
        {
            var pathBuilder = new StringBuilder(libraryModulePath);
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("src");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("main");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("AndroidManifest.xml");
            _libraryManifestFilePath = pathBuilder.ToString();
        }
        return _libraryManifestFilePath;
    }

}

internal class AndroidXmlDocument : XmlDocument
{
    private string m_Path;
    protected XmlNamespaceManager nsMgr;
    public readonly string AndroidXmlNamespace = "http://schemas.android.com/tools";
    public AndroidXmlDocument(string path)
    {
        m_Path = path;
        using (var reader = new XmlTextReader(m_Path))
        {
            reader.Read();
            Load(reader);
        }
        nsMgr = new XmlNamespaceManager(NameTable);
        //nsMgr.AddNamespace("android", AndroidXmlNamespace);
    }

    public string Save()
    {
        return SaveAs(m_Path);
    }

    public string SaveAs(string path)
    {
        using (var writer = new XmlTextWriter(path, new UTF8Encoding(false)))
        {
            writer.Formatting = Formatting.Indented;
            Save(writer);
        }
        return path;
    }
}


internal class AndroidManifest : AndroidXmlDocument
{
    private readonly XmlElement ApplicationElement;
    private readonly XmlElement ActivtyElement;

    public AndroidManifest(string path) : base(path)
    {
        ApplicationElement = SelectSingleNode("/manifest/application") as XmlElement;
        ActivtyElement = SelectSingleNode("/manifest/application/activity") as XmlElement;
    }

    private XmlAttribute CreateToolAttribute(string key, string value)
    {
        XmlAttribute attr = CreateAttribute("tools", key, AndroidXmlNamespace);
        attr.Value = value;
        return attr;
    }

    internal void setApplicationToolReplace(string replaceContent)
    {
        ApplicationElement.Attributes.Append(CreateToolAttribute("replace", replaceContent));

    }

    internal void removeLaunchMode()
    {
        //ActivtyElement.Attributes.find("android:launchMode");

        ActivtyElement.SetAttribute("android:launchMode", "standard");
    }

}
