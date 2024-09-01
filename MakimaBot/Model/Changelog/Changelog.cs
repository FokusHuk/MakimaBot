using System.ComponentModel.DataAnnotations;

public class ChangelogOptions
{
    [Required]
    public IEnumerable<Changelog> Changelogs { get; set; }
}

public class Changelog
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Version { get; set; }

    [Required]
    public string Description { get; set; }
}
