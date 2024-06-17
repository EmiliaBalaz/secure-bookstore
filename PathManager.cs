using System.Text.Json;
using System.Xml;
using System.Xml.XPath;

public class PathManager
{
    public static string GetSecurityPassword(IConfiguration configuration)
    {
        string SecurityFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        SecurityFolderPath = SecurityFolderPath + configuration["CertPasswordPath"];
        //XmlDocument projDefinition = new XmlDocument();
        //string fullProjectPath = "D:\\Projekat\\secure-online-bookstore\\secure-online-bookstore.csproj";
        //projDefinition.Load(fullProjectPath);
        //XPathNavigator navigator = projDefinition.CreateNavigator();
        //XPathNodeIterator iterator = navigator.Select(@"/Project/PropertyGroup/UserSecretsId");
        //while (iterator.MoveNext())
        //{
            //SecurityFolderPath = SecurityFolderPath + iterator.Current.Value;
        //}
        SecurityFolderPath = SecurityFolderPath + configuration["CertPasswordFile"];
        Dictionary<string, string> source = new Dictionary<string, string>();
        using(StreamReader sr = new StreamReader(SecurityFolderPath))
        {
            string json = sr.ReadToEnd();
            source = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }
        return source.Values.ToList()[0];
    }
}