using FluentValidation;
using Test_Assessment.Model;

namespace Test_Assessment.Helpers
{
    public class TripModelValidator : AbstractValidator<TripModel>
    {
        public TripModelValidator()
        {
            RuleFor(x => x.PickupDatetime).NotEmpty().LessThan(x => x.DropoffDatetime).WithMessage("Pickup must be before dropoff.");
            RuleFor(x => x.PassengerCount).GreaterThanOrEqualTo(0).WithMessage("Passenger count must be at least 1.");
            RuleFor(x => x.TripDistance).GreaterThanOrEqualTo(0).WithMessage("Trip distance must be non-negative.");
            RuleFor(x => x.FareAmount).GreaterThanOrEqualTo(0).WithMessage("Fare amount must be non-negative.");
            RuleFor(x => x.TipAmount).GreaterThanOrEqualTo(0).WithMessage("Tip amount must be non-negative.");
        }
    }
}
