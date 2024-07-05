using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public static class JsonFileReader
{
    // public static async Task<T> ReadAsync<T>(string filePath)
    // {
    //     // var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
    //     using FileStream stream = File.OpenRead(filePath);
    //     return await JsonSerializer.DeserializeAsync<T>(stream);
    // }
    public static Dictionary ReadJsonAsDictionary(string filePath)
    {
        var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
        var content = file.GetAsText();
        var json = new Json();
        var error = json.Parse(content);
        return (Dictionary)json.Data;
    }
}