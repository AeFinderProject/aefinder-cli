using CommandLine;

namespace AeFinder.Cli.Options;

[Verb("update", HelpText = "Update AeIndexer.")]
public class UpdateAppOptions : CommonOptions
{
    [Option(longName: "code", Required = false, HelpText = "The code file path of your AeIndexer.")]
    public string Code { get; set; }

    [Option(longName: "manifest", Required = false, HelpText = "The manifest file path of your AeIndexer.")]
    public string Manifest { get; set; }
    
    [Option(longName: "version", Required = true, HelpText = "The version to update.")]
    public string Version { get; set; }

    [Option(longName: "delete-attachments", Required = false, HelpText = "The attachment keys to delete.")]
    public IEnumerable<string> DeleteAttachmentKeys { get; set; }
    
    [Option(longName: "attachments", Required = false, HelpText = "The attachment path list.")]
    public IEnumerable<string> Attachments { get; set; }
}