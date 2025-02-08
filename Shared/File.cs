namespace Shared;

public record class File(string Recipient, string Name, string Content) 
{
    public string Suffix => Name.Split('.').Last();

    public static File? Load(string recipient, string path) 
    {
        string name = Path.GetFileName(path);
        if (name is null) return null;
        if (System.IO.File.Exists(path) == false) return null;
        
        byte[] content = System.IO.File.ReadAllBytes(path);
        return new File(recipient, name, Convert.ToBase64String(content));
    }

    public override string ToString()
    {
        return $"/sendfile {Recipient} {Name} {Content}";
    }
}
