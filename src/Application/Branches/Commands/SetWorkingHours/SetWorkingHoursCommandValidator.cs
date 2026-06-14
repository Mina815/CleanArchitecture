namespace CleanArchitecture.Application.Branches.Commands.SetWorkingHours;

public class SetWorkingHoursCommandValidator : AbstractValidator<SetWorkingHoursCommand>
{
    public SetWorkingHoursCommandValidator()
    {
        RuleFor(v => v.BranchId).NotEmpty();

        RuleFor(v => v.WorkingHours)
            .NotEmpty()
            .Must(list => list.Select(w => w.DayOfWeek).Distinct().Count() == list.Count)
            .WithMessage("Each day of week can only appear once.");

        RuleForEach(v => v.WorkingHours).ChildRules(wh =>
        {
            wh.When(w => !w.IsClosed, () =>
            {
                wh.RuleFor(w => w.OpenTime)
                    .LessThan(w => w.CloseTime)
                    .WithMessage("Open time must be before close time.");
            });
        });
    }
}
