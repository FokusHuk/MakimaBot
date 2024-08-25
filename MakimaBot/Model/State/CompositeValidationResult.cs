using System.ComponentModel.DataAnnotations;

namespace MakimaBot.Model;

public class CompositeValidationResult
{
	public CompositeValidationResult(string objectName, ValidationResult validationResult)
	{
		ObjectName = objectName;
		ValidationResult = validationResult;
	}
	
	public ValidationResult ValidationResult { get; }
	
	public string ObjectName { get; }
}
