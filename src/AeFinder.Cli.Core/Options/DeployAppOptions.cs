using CommandLine;

namespace AeFinder.Cli.Options;

[Verb("deploy", HelpText = "Deploy AeIndexer.")]
public class DeployAppOptions : CommonOptions
{
    [Option(longName: "code", Required = true, HelpText = "The code file path of your AeIndexer.")]
    public string Code { get; set; }

    [Option(longName: "manifest", Required = true, HelpText = "The manifest file path of your AeIndexer.")]
    public string Manifest { get; set; }
    
    [Option(longName: "attachments", Required = false, HelpText = "The Attachment list path of your AeIndexer.")]
    public IEnumerable<string> Attachments { get; set; }
}