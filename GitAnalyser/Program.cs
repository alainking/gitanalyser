// See https://aka.ms/new-console-template for more information

using System.Text;
using LibGit2Sharp;

Console.WriteLine(@"Enter the full path for your Git Project in the format c:\dir1\dir2");
var projectDir = Console.ReadLine();
var repo = new Repository(projectDir.EndsWith(".git") ? projectDir : projectDir + (projectDir.Contains('\\') ?@"\":@"/")+@".git");
Console.WriteLine(@"Enter the start date for the analysis (Format YYYY/MM/DD). Leave blank for the past year");
var startDateInput = Console.ReadLine();
var startDate = string.IsNullOrEmpty(startDateInput) ? DateTime.Now.AddYears(-1) : Convert.ToDateTime(startDateInput);
Console.WriteLine(@"Enter the end date for the analysis (Format YYYY/MM/DD). Leave blank for now");
var endDateInput = Console.ReadLine();
var endDate = string.IsNullOrEmpty(endDateInput) ? DateTime.Now : Convert.ToDateTime(endDateInput);
var commits = repo.Commits.Where(c=>c.Committer.When >= startDate && c.Committer.When <= endDate);
var modifiedFiles = new List<ModifiedFiles>();
foreach (var commit in commits)
{
  var tree = commit.Tree;
  if (!commit.Parents.Any())
    break;
  var parentTree = commit.Parents.First().Tree;

  var patch = repo.Diff.Compare<Patch>(parentTree, tree);

  foreach (var ptc in patch)
  {
    var modifiedFile = modifiedFiles.FirstOrDefault(f => f.Path == ptc.Path);
    if (modifiedFile == null)
    {
      modifiedFile = new ModifiedFiles(ptc.Path);
      modifiedFiles.Add(modifiedFile);
    }

    modifiedFile.Occurrence++;
    modifiedFile.LinesAdded += ptc.LinesAdded;
    modifiedFile.LinesDeleted += ptc.LinesDeleted;
  }

}

var csv = new StringBuilder();
csv.AppendLine("Filename,Folder,Extension,Count,Added,Deleted");
foreach (var file in modifiedFiles.OrderByDescending(m => m.Occurrence))
{
  var newLine = $"\"{file.Filename}\",{file.Folder},{file.Extension},{file.Occurrence},{file.LinesAdded},{file.LinesDeleted}";
  csv.AppendLine(newLine);
}
File.WriteAllText(projectDir + (projectDir.Contains('\\') ? @"\" : @"/")+ "git-analysis.csv", csv.ToString());

internal class ModifiedFiles
{
  public ModifiedFiles(string path)
  {
    Path = path;
    Folder = Path.Contains('/') ? Path[..Path.LastIndexOf('/')] : string.Empty;
    Filename = Path[(Path.LastIndexOf('/') + 1)..];
    Extension = Filename.Contains(".Designer.cs") ? Filename[Filename.IndexOf(".Designer.cs", StringComparison.Ordinal)..] : Filename[(Filename.LastIndexOf('.') + 1)..];
  }

  public string Path { get; }
  public string Folder { get; set; }
  public string Filename { get; set; }
  public string Extension { get; set; }
  public int Occurrence { get; set; }
  public int LinesAdded { get; set; }
  public int LinesDeleted { get; set; }

}