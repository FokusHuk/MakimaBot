using System.ComponentModel.DataAnnotations;

namespace MakimaBot.Model;

public abstract class ValidatableObject
{
    public IEnumerable<CompositeValidationResult> Validate()
	{
		var context = new ValidationContext(this);
		var validationResults = new List<ValidationResult>();

		if (!Validator.TryValidateObject(this, context, validationResults, validateAllProperties: true))
			return validationResults.Select(result => new CompositeValidationResult(this.GetType().Name, result));

		return ValidateCompositeProperties();
	}

	protected abstract IEnumerable<CompositeValidationResult> ValidateCompositeProperties();
}
